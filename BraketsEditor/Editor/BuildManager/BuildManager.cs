using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BraketsEngine;

namespace BraketsEditor;

public class BuildManager
{
    private static bool isDoneBuilding = true;
    private static bool isDoneRunning = true;

    private static Process runProcess;
    public static string runButtonText = "Run";

    public static float diagnosticRefreshRate = 0.5f;

    public static async void Build()
    {
        if (!isDoneBuilding) 
            return;

        Globals.EditorManager.Status = "Building...";
        isDoneBuilding = false; 
    
        Process buildProcess = new Process();
        buildProcess.StartInfo.FileName = "dotnet";
        buildProcess.StartInfo.Arguments = $"build {Globals.projectPath}";
        buildProcess.Exited += (object sender, EventArgs a) => {isDoneBuilding = true;};
        
        buildProcess.Start();
        await buildProcess.WaitForExitAsync();

        BraketsEngine.Debug.Log("Build process done!");
        Globals.EditorManager.Status = "Ready";
    }

    public static async void Run()
    {
        if (!isDoneRunning) 
        {   
            await Globals.EditorManager.bridgeServer.SendMessageToClient("stop");
            Globals.EditorManager.Status = "Stoppping...";
            await Task.Delay(100);

            runProcess?.Kill();
            
            return;
        }

        isDoneRunning = false;
        runButtonText = "Stop";

        string path = Path.GetFullPath(Globals.projectPath);

        runProcess = new Process();
        runProcess.StartInfo.FileName = "dotnet";
        runProcess.StartInfo.Arguments = $"run --project {Globals.projectPath} bridge localhost 8000 {path} {diagnosticRefreshRate}";
        runProcess.Exited += (object sender, EventArgs a) => 
        {
            isDoneRunning = true;
            runButtonText = "Run";
            BraketsEngine.Debug.Log("Run process done!");
        };

        DiagnosticsView.showGraphs = true;
        
        runProcess.Start();
        await runProcess.WaitForExitAsync();            

        DiagnosticsView.Reset();
        DiagnosticsView.showGraphs = false;
    }
}