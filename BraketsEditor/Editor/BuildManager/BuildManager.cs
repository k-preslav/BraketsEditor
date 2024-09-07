using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BraketsEngine;

namespace BraketsEditor;

public class BuildManager
{
    public static bool isDoneBuilding = true;
    public static bool isDoneRunning = true;

    public static bool isRunningDebug = true;

    private static Process runProcess;
    public static string runButtonText = "Run";

    public static float diagnosticRefreshRate = 0.5f;

    public static float runningTimer = 0;

    static Thread buildAndRun;

    public static async Task Build()
    {
        if (!isDoneBuilding) 
            return;

        isDoneBuilding = false;

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
            
            BraketsEngine.Debug.Log("Build process done!");
            Globals.EditorManager.Status = "Ready";
            
            isDoneBuilding = true;
        };
        
        buildProcess.Start();

        Globals.EditorManager.Status = "Building...";
        runButtonText = "...";

        await buildProcess.WaitForExitAsync();
    }

    public static async void RunDebug()
    {
        if (!isDoneRunning) 
        {
            await Globals.EditorManager.gameDebug_bridgeServer.SendMessageToClient("stop");
            Globals.EditorManager.Status = "Stopping...";
            await Task.Delay(100);

            runProcess?.Kill();
            
            return;
        }

        isRunningDebug = true;
        isDoneRunning = false;

        DiagnosticsView.ResetFull();

        string path = Path.GetFullPath(Globals.projectPath);

        runProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {Globals.projectPath} bridge localhost {Globals.EditorManager.gameDebug_bridgeServer.PORT} {path} {diagnosticRefreshRate}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        runProcess.OutputDataReceived += (sender, args) =>
        {
            runButtonText = "Stop";

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
            }
        };

        runProcess.Exited += (object sender, EventArgs a) =>
        {
            DiagnosticsView.Reset();
            DiagnosticsView.showGraphs = false;

            DiagnosticsView.areDiagnosticsAvailable = true;

            isDoneRunning = true;
            runButtonText = "Run";

            Throbber.visible = false;
            Globals.EditorManager.Status = "Ready";
            BraketsEngine.Debug.Log("Run process done!");
        };

        DiagnosticsView.showGraphs = true;

        await Task.Delay(10);
        runningTimer = 0;

        runProcess.Start();

        runProcess.BeginOutputReadLine();
        runProcess.BeginErrorReadLine();

        await runProcess.WaitForExitAsync();            
    }

    public static async void Run()
    {
        if (!isDoneRunning)
        {
            runProcess?.Kill();
            return;
        }

        DiagnosticsView.ResetFull();
        runningTimer = 0;

        Throbber.visible = true;
        Globals.EditorManager.Status = "Starting Run Service...";

        isDoneRunning = false;
        isRunningDebug = false;

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
                }
            };

            runProcess.Exited += (sender, e) =>
            {
                isDoneRunning = true;
                runButtonText = "Run";

                Throbber.visible = false;
                Globals.EditorManager.Status = "Ready";
                BraketsEngine.Debug.Log("Run process done!");
            };

            runProcess.Start();
            runProcess.BeginOutputReadLine();
            runProcess.BeginErrorReadLine();
            await runProcess.WaitForExitAsync();
        }
    }

    public static void OnRunBtnClick(bool debug)
    {
        Task.Run(async () =>
        {
            Throbber.visible = true;

            BraketsEngine.Debug.Log("Building application...");
            await Build();

            Thread.Sleep(100);

            BraketsEngine.Debug.Log("Running application in DebugMode...");
            if (debug) RunDebug();
            else Run();
        });
    }
}