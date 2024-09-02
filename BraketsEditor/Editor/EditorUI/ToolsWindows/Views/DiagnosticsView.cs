using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BraketsEditor.Editor;
using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BraketsEditor;
public class DiagnosticsView
{
    private static List<float> fpsValues = new List<float>();
    private static List<float> memoryValues = new List<float>();
    private static List<float> dtValues = new List<float>();
    private static List<float> spritesCounts = new List<float>();
    private static List<float> GC_Calls = new List<float>();
    private static List<float> threadsCounts = new List<float>();
    private static List<string> logMessages = new List<string>();
    private const int MaxValues = 1500; // Limit the length of each list

    private static List<float> fpsValuesFull = new List<float>();
    private static List<float> memoryValuesFull = new List<float>();
    private static List<float> dtValuesFull = new List<float>();
    private static List<float> spritesCountsFull = new List<float>();
    private static List<float> GC_CallsFull = new List<float>();
    private static List<float> threadsCountsFull = new List<float>();
    private static List<string> logMessagesFull = new List<string>();

    public static bool showGraphs = false;
    public static bool hasLaunched = false;
    public static float currentDt = 0;
    internal static float currentFps = 0;
    public static float currentMemory = 0;
    public static float currentSpriteCount = 0;
    public static float currentGC = 0;
    public static float currentThreadsCount = 0;

    static string[] refreshIntervalOptions = {
        "Fast, Lower Performance (0.25s)", 
        "Balanced (0.50s)", 
        "Slow, Better Performance (1s)", 
        "None, Best Performance"
    };
    static int _refreshIntervalSelectedIndex = 1;

    internal static bool areDiagnosticsAvailable = false;
    static bool areDiagnosticsDownloaded = false;
    static string dwnldDataBText = "Download data";

    public static void Draw()
    {
        if (ImGui.Combo("Refresh Interval", ref _refreshIntervalSelectedIndex, refreshIntervalOptions, refreshIntervalOptions.Length))
        {
            float n = BuildManager.diagnosticRefreshRate;

            if (_refreshIntervalSelectedIndex == 0) n = 0.25f;
            else if (_refreshIntervalSelectedIndex == 1) n = 0.5f;
            else if (_refreshIntervalSelectedIndex == 2) n = 1f;
            else if (_refreshIntervalSelectedIndex == 3) n = 0f;
            
            BuildManager.diagnosticRefreshRate = n;
        }

        if (!showGraphs && !hasLaunched)
        {
            ImGui.NewLine();

            if (fpsValuesFull.Count == 0 || !areDiagnosticsAvailable)
                ImGui.Text("No diagnostic data.");
            else
            {
                if (areDiagnosticsAvailable)
                {
                    ImGui.Text("See lates diagnostics:");
                    ImGui.Spacing();

                    if (!areDiagnosticsDownloaded)
                    {
                        if (ImGui.Button(dwnldDataBText, new System.Numerics.Vector2(150, 35)))
                        {
                            new Task(async() =>
                            {
                                dwnldDataBText = "...";
                                Throbber.visible = true;
                                Globals.EditorManager.Status = "Saving diagnostics...";

                                await DownloadGraphs();
                                await DownloadLog();
                                
                                dwnldDataBText = "Save data";
                                Throbber.visible = false;
                                Globals.EditorManager.Status = "Ready";

                                areDiagnosticsDownloaded = true;
                            }).RunSynchronously();
                        }
                        ImGui.SameLine();
                        ImGui.Spacing();
                        ImGui.SameLine();
                    }

                    if (ImGui.Button("Open Diagnostics Folder", new System.Numerics.Vector2(200, 35)))
                    {
                        OpenInExplorer.OpenDebugDataFolder();
                    }
                }
            }

            DrawLog(full: true);
            return;
        }
        else if (showGraphs && !hasLaunched)
        {
            ImGui.NewLine();
            ImGui.Text("Starting application...");    

            return;
        }

        fpsValues.Add(currentFps);
        fpsValuesFull.Add(currentFps);

        dtValues.Add(currentDt);
        dtValuesFull.Add(currentDt);
        
        memoryValues.Add(currentMemory);
        memoryValuesFull.Add(currentMemory);

        spritesCounts.Add(currentSpriteCount);
        spritesCountsFull.Add(currentSpriteCount);
        
        GC_Calls.Add(currentGC);
        GC_CallsFull.Add(currentGC);
        
        threadsCounts.Add(currentThreadsCount);
        threadsCountsFull.Add(currentThreadsCount);

        if (fpsValues.Count > MaxValues) fpsValues.RemoveAt(0);
        if (memoryValues.Count > MaxValues) memoryValues.RemoveAt(0);
        if (dtValues.Count > MaxValues) dtValues.RemoveAt(0);
        if (spritesCounts.Count > MaxValues) spritesCounts.RemoveAt(0); 
        if (GC_Calls.Count > MaxValues) GC_Calls.RemoveAt(0);
        if (threadsCounts.Count > MaxValues) threadsCounts.RemoveAt(0);

        float[] fpsArray = fpsValues.ToArray();
        float[] dtArray = dtValues.ToArray();
        float[] memoryArray = memoryValues.ToArray();
        float[] spritesArray = spritesCounts.ToArray();
        float[] gcArray = GC_Calls.ToArray();
        float[] threadsArray = threadsCounts.ToArray();

        float fpsMaxScale = fpsValues.Max() * 2;
        float dtMaxScale = dtValues.Max() * 2;
        float memoryMaxScale = memoryValues.Max() * 2;
        float spriteCountScale = spritesCounts.Max() * 2;
        float gcMaxScale = GC_Calls.Max() * 1.25f;
        float threadsMaxScale = threadsCounts.Max() * 1.25f;

        ImGui.NewLine();    
        ImGui.SetNextItemWidth(250);
        ImGui.PlotLines($"FPS {currentFps.ToString("0.00")}", ref fpsArray[0], fpsArray.Length, 0, null, 0f, fpsMaxScale, new System.Numerics.Vector2(0, 50));
    
        ImGui.SetNextItemWidth(250);
        ImGui.PlotLines($"DT {currentDt.ToString("0.000")} (s)", ref dtArray[0], dtArray.Length, 0, null, 0, dtMaxScale, new System.Numerics.Vector2(0, 50));
        ImGui.NewLine();    

        ImGui.SetNextItemWidth(250);
        ImGui.PlotLines($"Memory {currentMemory} (MB)", ref memoryArray[0], memoryArray.Length, 0, null, 0, memoryMaxScale, new System.Numerics.Vector2(0, 50));    
    
        ImGui.SetNextItemWidth(250);
        ImGui.PlotLines($"Sprites {currentSpriteCount}", ref spritesArray[0], spritesArray.Length, 0, null, 0, spriteCountScale, new System.Numerics.Vector2(0, 50));
        ImGui.NewLine();    

        ImGui.SetNextItemWidth(350);
        ImGui.PlotHistogram($"GC Calls {currentGC}", ref gcArray[0], gcArray.Length, 0, null, 0, gcMaxScale, new System.Numerics.Vector2(0, 50));

        ImGui.SetNextItemWidth(350);
        ImGui.PlotHistogram($"Active Threads {currentThreadsCount}", ref threadsArray[0], threadsArray.Length, 0, null, 0, threadsMaxScale, new System.Numerics.Vector2(0, 50));

        ImGui.NewLine();

        DrawLog(full:false);
    }

    public static void Update()
    {
        if (currentDt <= 0 && currentMemory <= 0) hasLaunched = false;
        else hasLaunched = true;

        if (!showGraphs && !hasLaunched)
        {
            return;
        }

        if (showGraphs && !hasLaunched)
        {
            Globals.EditorManager.Status = "Starting Run Service...";
            Throbber.visible = true;
            
            return;
        }

        Throbber.visible = false;

        int hours = (int)TimeSpan.FromSeconds(BuildManager.runningTimer).TotalHours;
        int minutes = (int)TimeSpan.FromSeconds(BuildManager.runningTimer).TotalMinutes;
        int seconds = (int)TimeSpan.FromSeconds(BuildManager.runningTimer).TotalSeconds;

        Globals.EditorManager.Status = $"Application Running... " +
            $"{(hours > 0 ? ($"{hours:00}:") : "")}" +
            $"{minutes:00}:" +
            $"{seconds:00}";
    }

    static void DrawLog(bool full)
    {
        if (full)
            ImGui.SetCursorPosY(ImGui.GetWindowSize().Y / 3);

        ImGui.Spacing();
        ImGui.SeparatorText("Console");
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 10);

        string copyTexture = WindowTheme.currentTheme == "dark" ? "ui/copy/copy-white" : "ui/copy/copy-black";
        if (ImGui.ImageButton("###copy-log_button", ResourceManager.GetImGuiTexture(copyTexture), new Vector2(16).ToNumerics()))
        {
            string fullLog = "";
            foreach (var log in logMessagesFull)
                fullLog += (log + "\n");

            TextCopy.ClipboardService.SetText(fullLog);
        }

        ImGui.BeginChild("ConsoleMessages", new Vector2(0, -1).ToNumerics(), ImGuiChildFlags.Border, ImGuiWindowFlags.HorizontalScrollbar);

        bool autoScroll = false;
        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
        {
            autoScroll = true;
        }

        if (logMessages.Count > MaxValues) logMessages = logMessages.Take(1000).ToList();

        try
        {
            foreach (var message in full ? logMessagesFull.Take(1000).ToList() : logMessages.ToList())
            {
                if (message.ToLower().Contains("error"))
                    ImGui.TextColored(new Vector4(0.8f, 0, 0, 1).ToNumerics(), message);
                else if (message.ToLower().Contains("fatal"))
                    ImGui.TextColored(new Vector4(1, 0, 0, 1).ToNumerics(), message);
                else if (message.ToLower().Contains("warning"))
                    ImGui.TextColored(new Vector4(1, 1, 0, 1).ToNumerics(), message);
                else
                    ImGui.TextColored(
                        WindowTheme.currentTheme == "dark" ? Color.LightCyan.ToVector4().ToNumerics()
                        : Color.Black.ToVector4().ToNumerics(), message
                    );
            }
        }
        catch (Exception ex)
        {
            BraketsEngine.Debug.Error($"Failed to draw debug messages in console! \nEX: {ex.Message}");
        }

        if (autoScroll)
        {
            ImGui.SetScrollHereY(1.0f);
        }

        ImGui.EndChild();
    }

    public static void Reset()
    {
        currentFps = 0;
        currentDt = 0;
        currentMemory = 0;
        currentGC = 0;
        currentSpriteCount = 0;
        currentThreadsCount = 0;

        fpsValues.Clear();
        dtValues.Clear();
        memoryValues.Clear();
        GC_Calls.Clear();
        spritesCounts.Clear();
        threadsCounts.Clear();
        logMessages.Clear();
    }

    public static void ResetFull()
    {
        Reset();

        fpsValuesFull.Clear();
        dtValuesFull.Clear();
        memoryValuesFull.Clear();
        GC_CallsFull.Clear();
        spritesCountsFull.Clear();
        threadsCountsFull.Clear();
        logMessagesFull.Clear();

        areDiagnosticsDownloaded = false;
        areDiagnosticsAvailable = false;

        string graphPath = $"{Globals.projectPath}/DEBUG_DATA/graphs.csv";
        string logPath = $"{Globals.projectPath}/DEBUG_DATA/log.txt";
        if (File.Exists(graphPath)) File.Delete(graphPath);
        if (File.Exists(logPath)) File.Delete(logPath);
    }

    public static void AddMessageToLog(string msg) 
    {
        logMessages.Add(msg);
        logMessagesFull.Add($"[{DateTime.Now}] {msg}");
    }

    private static async Task DownloadGraphs()
    {
        string directory = $"{Globals.projectPath}/DEBUG_DATA";
        string path = $"{directory}/graphs.csv";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("FPS,DT,Memory,Sprites,GC Calls,Threads");
            for (int i = 0; i < fpsValuesFull.Count; i++)
            {
                string line = $"{fpsValuesFull[i]},{dtValuesFull[i]},{memoryValuesFull[i]},{spritesCountsFull[i]},{GC_CallsFull[i]},{threadsCountsFull[i]}";
                await writer.WriteLineAsync(line);
            }
        }

        BraketsEngine.Debug.Log($"Graphs saved as CSV to path: {path}");
    }
    private static async Task DownloadLog()
    {
        string directory = $"{Globals.projectPath}/DEBUG_DATA";
        string path = $"{directory}/log.txt";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string data = "";
        foreach (var line in logMessagesFull)
        {
            data += $"{line}\n";
        }
        await File.WriteAllTextAsync(path, data);

        BraketsEngine.Debug.Log($"Logs saved as TXT to path: {path}");
    }
}
