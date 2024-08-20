using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace BraketsEditor;
public class DiagnosticsView
{
    // TODO: Add a way to view the last report and also download it as a spreadsheet

    private static List<float> fpsValues = new List<float>();
    private static List<float> memoryValues = new List<float>();
    private static List<float> dtValues = new List<float>();
    private static List<float> spritesCounts = new List<float>();
    private static List<float> GC_Calls = new List<float>();
    private static List<float> threadsCounts = new List<float>();
    private static List<string> logMessages = new List<string>();
    private const int MaxValues = 1500; // Limit the length of each list

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

    public static void Draw()
    {
        if (currentDt <= 0 && currentMemory <= 0) hasLaunched = false;
        else hasLaunched = true;

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
            ImGui.Text("No diagnostic data.");
            Globals.EditorManager.Status = "Ready";
            
            return;
        }
        else if (showGraphs && !hasLaunched)
        {
            ImGui.Text("Starting application...");
            Globals.EditorManager.Status = "Starting...";

            return;
        }

        Globals.EditorManager.Status = "Application Running...";
        
        fpsValues.Add(currentFps);
        dtValues.Add(currentDt);
        memoryValues.Add(currentMemory);
        spritesCounts.Add(currentSpriteCount);
        GC_Calls.Add(currentGC);
        threadsCounts.Add(currentThreadsCount);

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
        ImGui.BeginChild("ConsoleMessages", new Vector2(0, -10).ToNumerics(), ImGuiChildFlags.Border, ImGuiWindowFlags.HorizontalScrollbar);
        
        // LOG
        bool autoScroll = false;
        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
        {
            autoScroll = true;
        }

        if (logMessages.Count > MaxValues) logMessages.Clear();

        foreach (var message in logMessages.ToList())
        {
            if (message.ToLower().Contains("error"))
                ImGui.TextColored(new Vector4(0.8f, 0, 0, 1).ToNumerics(), message);
            else if (message.ToLower().Contains("fatal"))
                ImGui.TextColored(new Vector4(1, 0, 0, 1).ToNumerics(), message);
            else if (message.ToLower().Contains("warning"))
                ImGui.TextColored(new Vector4(1, 1, 0, 1).ToNumerics(), message);
            else
                ImGui.TextColored(new Vector4(0.95f, 0.95f, 0.95f, 1).ToNumerics(), message);
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

    public static void AddMessageToLog(string msg) 
    {
        logMessages.Add(msg);
    }
}
