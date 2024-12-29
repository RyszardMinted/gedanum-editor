using System.Diagnostics;
using System.Globalization;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
using Debug = UnityEngine.Debug;

public class BuildStepLogger
{
    [MenuItem("Build/Log Build Steps")]
    public static void LogBuildSteps()
    {
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = "Builds/WebGL",
            target = BuildTarget.WebGL
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

        var logFilePath = "BuildData.csv";
        var path = Path.GetFullPath(logFilePath);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
           
            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                writer.WriteLine("Step,Duration (s)");
                foreach (var step in report.steps)
                {
                    try
                    {
                        var durationInSeconds = step.duration.TotalSeconds;

                        var stepName = string.IsNullOrWhiteSpace(step.name) ? "Unknown Step" : step.name;
                        var formattedDuration = durationInSeconds.ToString("F2", CultureInfo.InvariantCulture);

                        writer.WriteLine($"{stepName},{formattedDuration}");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Error processing step: {step.name}. Exception: {ex.Message}");
                    }
                }

                writer.WriteLine();
                writer.WriteLine("Asset,Size (KB)");
                foreach (var file in report.GetFiles())
                {
                    writer.WriteLine($"{file.path},{file.size / 1024f:F2}");
                }
            }

            Debug.Log($"Build data exported to {logFilePath}");
        }
        else
        {
            Debug.LogWarning("Build failed. Data export skipped.");
        }
        
        Debug.Log($"File path to log {path} {logFilePath}");
        
        
        RunPythonScript(Path.GetDirectoryName(path));
    }
    
    private static void RunPythonScript(string path)
    {
        Debug.Log("Run python at location: " + path);
        var pythonPath = "python3";
        var scriptPath = path+"/BuildScripts/buildStats.py";

        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = scriptPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = Process.Start(startInfo);
        if (process == null) return;
        
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Debug.Log($"Python Output: {e.Data}");
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Debug.LogError($"Python Error: {e.Data}");
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }
}