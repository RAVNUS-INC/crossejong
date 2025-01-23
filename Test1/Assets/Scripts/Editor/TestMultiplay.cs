using UnityEditor;


public class TestMultiplay
{
    [MenuItem("Test/Test Multiplay Android")]
    private static void TestMultiplayWin64()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        for (int n = 0; n < 1; n++)
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = GetScenesPath();
            options.locationPathName = string.Format("Build/Android/{0}/Test.apk", n);
            options.target = BuildTarget.Android;
            options.options = BuildOptions.AutoRunPlayer;
            BuildPipeline.BuildPlayer(options);

        }
    }

    private static string[] GetScenesPath()
    {
        EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;
        string[] scenes_path = new string[scenes.Length];
        for (int n = 0; n < scenes.Length; n++)
        {
            scenes_path[n] = scenes[n].path;
        }

        return scenes_path;
    }
}