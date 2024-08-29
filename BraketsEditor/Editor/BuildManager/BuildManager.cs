using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BraketsEditor.Editor;
using BraketsEngine;

namespace BraketsEditor;

public class BuildManager
{
    public static bool isDoneBuilding = true;
    public static bool isDoneRunning = true;

    private static Process runProcess;
    public static string runButtonText = "Run";

    public static float diagnosticRefreshRate = 0.5f;

    public static async Task Build()
    {
        if (!isDoneBuilding) 
            return;

        isDoneBuilding = false;
        bool isSuccess = true;

        Process buildProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build {Globals.projectPath}",
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };
        buildProcess.Exited += (object sender, EventArgs a) => {
            isDoneBuilding = true;
            
            if (isSuccess)
            {
                BraketsEngine.Debug.Log("Build process done!");
                Globals.EditorManager.Status = "Ready";
            }
            else
            {
                BraketsEngine.Debug.Log("Build process failed!");
                Globals.EditorManager.Status = "Build Failed";
            }
        };

        buildProcess.OutputDataReceived += (sender, args) =>
        {
            isSuccess = false;

            BraketsEngine.Debug.Warning("Failed to build project:\n " + args.Data);
            new MessageBox($"Build failed!\n {args.Data}");
        };
        
        buildProcess.Start();
        buildProcess.BeginErrorReadLine();

        Globals.EditorManager.Status = "Building...";
        runButtonText = "...";

        await buildProcess.WaitForExitAsync();
    }

    public static async void RunDebug()
    {
        if (!isDoneRunning) 
        {
            await Globals.EditorManager.bridgeServer.SendMessageToClient("stop");
            Globals.EditorManager.Status = "Stopping...";
            await Task.Delay(100);

            runProcess?.Kill();
            
            return;
        }

        isDoneRunning = false;

        DiagnosticsView.ResetFull();

        string path = Path.GetFullPath(Globals.projectPath);

        runProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {Globals.projectPath} bridge localhost 8000 {path} {diagnosticRefreshRate}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        runProcess.OutputDataReceived += (sender, args) =>
        {
            runButtonText = "Stop";
            Globals.EditorManager.Status = "Starting Debugger...";

            if (args.Data != null)
            {
                DiagnosticsView.AddMessageToLog(args.Data);
            }
        };

        runProcess.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                DiagnosticsView.AddMessageToLog(args.Data);
                BraketsEngine.Debug.Error(args.Data);

                // TODO: Show a message that the build/run has failed and show the error
            }
        };

        runProcess.Exited += (object sender, EventArgs a) =>
        {
            Globals.EditorManager.Status = "Ready";
            Throbber.visible = false;

            DiagnosticsView.Reset();
            DiagnosticsView.showGraphs = false;

            isDoneRunning = true;
            runButtonText = "Run";

            BraketsEngine.Debug.Log("Run process done!");
        };

        DiagnosticsView.showGraphs = true;

        await Task.Delay(100);      
        runProcess.Start();

        runProcess.BeginOutputReadLine();
        runProcess.BeginErrorReadLine();

        await runProcess.WaitForExitAsync();            
    }

    public static async void Run()
    {
        Globals.EditorManager.Status = "Starting...";
        Throbber.visible = true;

        using (var runProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {Globals.projectPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Globals.projectPath
            }
        })
        {
            runProcess.OutputDataReceived += (sender, args) =>
            {
                runButtonText = "Stop";
                Globals.EditorManager.Status = "Application Running...";
                Throbber.visible = false;

                if (args.Data != null)
                {
                    DiagnosticsView.AddMessageToLog(args.Data);
                }
            };

            runProcess.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    DiagnosticsView.AddMessageToLog(args.Data);
                    BraketsEngine.Debug.Error(args.Data);

                    // TODO: Show a message that the build/run has failed and show the error
                }
            };

            runProcess.Exited += (sender, e) =>
            {
                Globals.EditorManager.Status = "Ready";
                Throbber.visible = false;

                isDoneRunning = true;
                runButtonText = "Run";

                BraketsEngine.Debug.Log("Run process done!");
            };

            runProcess.Start();
            runProcess.BeginOutputReadLine();
            runProcess.BeginErrorReadLine();
            await runProcess.WaitForExitAsync();
        }
    }

}