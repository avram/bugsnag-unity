using System;
using UnityEngine;

namespace BugsnagUnity
{
  public class BugsnagBehaviour : MonoBehaviour
  {
    /// <summary>
    /// Exposed in the Unity Editor to configure this behaviour
    /// </summary>
    public string BugsnagApiKey = "";

    /// <summary>
    /// Exposed in the Unity Editor to configure this behaviour
    /// </summary>
    public bool AutoNotify = true;

    public LogType NotifyLevel = LogType.Exception;

    public int MaximumBreadcrumbs = 25;

    public int UniqueLogsPerSecond = 5;

    public bool AutoCaptureSessions = true;

    public string Notify = Bugsnag.NotifyEndpoint;

    public string Sessions = Bugsnag.SessionsEndpoint;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// We use this to pull the fields that have been set in the
    /// Unity editor and pass them to the Bugsnag client.
    /// </summary>
    void Awake()
    {
      Bugsnag.Init(BugsnagApiKey);
      Bugsnag.Client.Configuration.Endpoint = new Uri(Notify);
      Bugsnag.Client.Configuration.SessionEndpoint = new Uri(Sessions);
      Bugsnag.Client.Configuration.AutoNotify = AutoNotify;
      Bugsnag.Client.Configuration.AutoCaptureSessions = AutoCaptureSessions;
      Bugsnag.Client.Configuration.UniqueLogsTimePeriod = TimeSpan.FromSeconds(UniqueLogsPerSecond);
      Bugsnag.Client.Configuration.NotifyLevel = NotifyLevel;
      Bugsnag.Client.Configuration.ReleaseStage = Debug.isDebugBuild ? "development" : "production";
      Bugsnag.Client.Configuration.MaximumBreadcrumbs = MaximumBreadcrumbs;
    }

    /// <summary>
    /// OnApplicationFocus is called when the application loses or gains focus.
    /// Alt-tabbing or Cmd-tabbing can take focus away from the Unity
    /// application to another desktop application. This causes the GameObjects
    /// to receive an OnApplicationFocus call with the argument set to false.
    /// When the user switches back to the Unity application, the GameObjects
    /// receive an OnApplicationFocus call with the argument set to true.
    /// </summary>
    /// <param name="hasFocus"></param>
    void OnApplicationFocus(bool hasFocus)
    {
      Bugsnag.Client.SetApplicationState(hasFocus);
    }

    void OnApplicationPause(bool paused)
    {
      var hasFocus = !paused;
      Bugsnag.Client.SetApplicationState(hasFocus);
    }

    void OnValidate()
    {
      Uri temp;

      if (!Uri.TryCreate(Notify, UriKind.Absolute, out temp))
      {
        Notify = Bugsnag.NotifyEndpoint;
      }

      if (!Uri.TryCreate(Sessions, UriKind.Absolute, out temp))
      {
        Sessions = Bugsnag.SessionsEndpoint;
      }
    }
  }
}
