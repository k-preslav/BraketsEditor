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
    private static List<float> fpsValues = new List<float>();
    private static List<float> memoryValues = new List<float>();
    private static List<float> spritesCounts = new List<float>();
    private const int MaxValues = 350; // Limit the length of each list

    public static bool showGraphs = false;
    public static bool hasLaunched = false;
    public static float currentFps = 0;
    public static float currentMemory = 0;
    public static float currentSpriteCount = 0;

    static string[] refreshIntervalOptions = {
        "Fast, Lower Performance (0.25s)", 
        "Balanced (0.50s)", 
        "Slow, Better Performance (1s)", 
        "None, Best Performance"
    };
    static int _refreshIntervalSelectedIndex = 1;

    public static void Draw()
    {
        if (currentFps <= 0 && currentMemory <= 0) hasLaunched = false;
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
        memoryValues.Add(currentMemory);
        spritesCounts.Add(currentSpriteCount);

        if (fpsValues.Count > MaxValues) fpsValues.RemoveAt(0);
        if (memoryValues.Count > MaxValues) memoryValues.RemoveAt(0);
        if (spritesCounts.Count > MaxValues) spritesCounts.RemoveAt(0);

        float[] fpsArray = fpsValues.ToArray();
        float[] memoryArray = memoryValues.ToArray();
        float[] spritesArray = spritesCounts.ToArray();

        float fpsMaxScale = fpsValues.Max() * 2;
        ImGui.PlotLines($"FPS {currentFps.ToString("0.00")}", ref fpsArray[0], fpsArray.Length, 0, null, 0f, fpsMaxScale, new System.Numerics.Vector2(0, 40));
    
        float spriteCountScale = spritesCounts.Max() * 2;
        ImGui.PlotLines($"Sprites {currentSpriteCount}", ref spritesArray[0], spritesArray.Length, 0, null, 0, spriteCountScale, new System.Numerics.Vector2(0, 40));

        float memoryMaxScale = memoryValues.Max() * 2;
        ImGui.PlotLines($"Memory {currentMemory} (MB)", ref memoryArray[0], memoryArray.Length, 0, null, 0, memoryMaxScale, new System.Numerics.Vector2(0, 80));
    }

    public static void Reset()
    {
        currentFps = 0;
        currentMemory = 0;
        fpsValues.Clear();
        memoryValues.Clear();
    }
}
