using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BraketsPluginIntegration;
using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace BraketsEditor;

public struct ToolTab
{
    public string name;
    public Action view;
    public Action update;
}

public class MainToolsWindow
{
    public static List<ToolTab> tabs = new List<ToolTab>();

    static DebugWindow parent;

    public static void Create()
    {
        parent = PluginAbstraction.MakeWindow("Tools", () =>
        {
            MainToolsWindow.Draw();
        }, () => { }, _flags: ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus, 
                      _overridePos: true, _overrideSize: true);
    }

    public static void Draw()
    {
        parent.Pos = new Vector2(Globals.DEBUG_UI.GetWindow("Objects").Size.X, Globals.DEBUG_UI_MENUBAR_SIZE_Y);
        parent.Size = new Vector2(Globals.APP_Width - parent.Pos.X, Globals.APP_Height - Globals.DEBUG_UI_MENUBAR_SIZE_Y);
        
        if (ImGui.BeginTabBar("ToolsTabBar", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
        {
            foreach (var tab in tabs.ToList())
            {
                if (ImGui.BeginTabItem(tab.name))
                {
                    tab.view.Invoke();
                    ImGui.EndTabItem();
                }
            }
        }
        ImGui.EndTabBar();
    }
    public static void Update()
    {
        foreach (var tab in tabs.ToList())
        {
            tab.update?.Invoke();
        }
    }

    public static async void AddTab(ToolTab newTab)
    {
        foreach (var tab in tabs.ToList())
        {
            if (tab.name == newTab.name)
                tabs.Remove(tab);
        }

        await Task.Delay(100);
        tabs.Add(newTab);
    }
}