using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Main : MonoBehaviour {
#if UNITY_EDITOR
  public static void CreateScene() {
    var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, UnityEditor.SceneManagement.NewSceneMode.Single);
    UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
    var obj = new GameObject("Bugsnag");
    var bugsnag = obj.AddComponent<BugsnagUnity.BugsnagBehaviour>();
    bugsnag.BugsnagApiKey = System.Environment.GetEnvironmentVariable("BUGSNAG_APIKEY");
    obj.AddComponent<Main>();
    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/MainScene.unity");
    var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
    scenes.Add(new EditorBuildSettingsScene("Assets/MainScene.unity", true));
    EditorBuildSettings.scenes = scenes.ToArray();
  }

  public static void MacOS() {
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes = new[] { "Assets/MainScene.unity" };
    buildPlayerOptions.locationPathName = "../mazerunner.app";
    buildPlayerOptions.target = BuildTarget.StandaloneOSX;
    buildPlayerOptions.options = BuildOptions.None;
    BuildPipeline.BuildPlayer(buildPlayerOptions);
  }

  public static void Android() {
    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.X86;
    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.bugsnag.mazerunner");
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes = new[] { "Assets/MainScene.unity" };
    buildPlayerOptions.locationPathName = "../mazerunner.apk";
    buildPlayerOptions.target = BuildTarget.Android;
    buildPlayerOptions.options = BuildOptions.None;
    BuildPipeline.BuildPlayer(buildPlayerOptions);
  }
#endif

  bool sent = false;

  void Update() {
    // only send one crash
    if (!sent) {
      sent = true;
      BugsnagUnity.Bugsnag.Client.Configuration.Endpoint =
        new System.Uri(System.Environment.GetEnvironmentVariable("MAZE_ENDPOINT"));
      BugsnagUnity.Bugsnag.Client.Breadcrumbs.Leave("bleeb");
      BugsnagUnity.Bugsnag.Client.Notify(new System.Exception("blorb"), report => {
        report.Event.User.Name = "blarb";
      });
      // wait for 5 seconds before exiting the application
      StartCoroutine(WaitForBugsnag());
    }
  }

  IEnumerator WaitForBugsnag() {
    yield return new WaitForSeconds(5);
    Application.Quit();
  }
}
