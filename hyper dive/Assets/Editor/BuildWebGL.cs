using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Builds the browser version of Hanuman Dive into <repo>/WebBuild, using the
/// portrait page template in Assets/WebGLTemplates/HanumanDive.
/// Menu: Tools > Hanuman Dive > Build WebGL. Headless:
///   Unity -batchmode -quit -projectPath "hyper dive" -buildTarget WebGL -executeMethod BuildWebGL.Build
/// Publish by copying WebBuild/ to the gh-pages branch (see README).
/// </summary>
public static class BuildWebGL
{
    [MenuItem("Tools/Hanuman Dive/Build WebGL")]
    public static void Build()
    {
        PlayerSettings.productName = "Hanuman Dive";
        PlayerSettings.WebGL.template = "PROJECT:HanumanDive";
        // GitHub Pages can't send Content-Encoding headers for pre-compressed files,
        // so ship gzip with the built-in JavaScript decompression fallback.
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        PlayerSettings.runInBackground = false;

        // Startup splash: Hanuman instead of the old Hyper Dive logo (Unity Personal keeps "Made with Unity").
        var hanuman = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/A-Z_Materials/hanuman/hanuman_player.png");
        PlayerSettings.SplashScreen.show = true;
        PlayerSettings.SplashScreen.showUnityLogo = true;
        PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.AllSequential;
        PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
        PlayerSettings.SplashScreen.backgroundColor = new Color(0.10f, 0.05f, 0.02f);
        PlayerSettings.SplashScreen.background = null;
        PlayerSettings.SplashScreen.logos = hanuman != null
            ? new[] { PlayerSettings.SplashScreenLogo.Create(1.5f, hanuman) }
            : new PlayerSettings.SplashScreenLogo[0];

        string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../WebBuild"));
        if (Directory.Exists(output)) Directory.Delete(output, true);

        var options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = output,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        Debug.Log("HANUMAN_WEBGL " + report.summary.result + " size=" + report.summary.totalSize / (1024 * 1024) + "MB time=" +
                  report.summary.totalTime + " -> " + output);
        if (Application.isBatchMode) EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
