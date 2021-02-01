using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace TG.Build
{
	public class TGBuildPostProcessor : IPostprocessBuildWithReport
	{
		public int callbackOrder => 1;

		public void OnPostprocessBuild(BuildReport report)
		{
#if UNITY_IOS
			BuildTarget buildTarget = report.summary.platform;
			string path = report.summary.outputPath;

			if (buildTarget != BuildTarget.iOS)
				return;

			string projPath = PBXProject.GetPBXProjectPath(path);

			// buid settings
			PBXProject proj = new PBXProject();
			proj.ReadFromFile(projPath);
			var targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
			proj.SetBuildProperty(targetGuid, "VALIDATE_WORKSPACE", "True");
			proj.SetBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");

			// capabilities
			ProjectCapabilityManager manager = new ProjectCapabilityManager(
				projPath,
				"Entitlements.entitlements",
				PBXProject.GetUnityTargetName()
				);

			manager.AddSignInWithApple();
			manager.AddPushNotifications(false);
			
			// save setup
			manager.WriteToFile();
			proj.WriteToFile(projPath);
#endif
		}
	}
}