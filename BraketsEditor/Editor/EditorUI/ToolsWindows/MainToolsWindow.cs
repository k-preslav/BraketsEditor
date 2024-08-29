using System;
using System.Collections.Generic;
using BraketsEditor.Editor;
using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace BraketsEditor;

public struct ToolTab
{
    public string name;
    public Action view;
}

public class MainToolsWindow
{
    public static List<ToolTab> tabs = new List<ToolTab>();

    public static void Draw(DebugWindow parent)
    {
        parent.Pos = new Vector2(Globals.DEBUG_UI.GetWindow("Objects").Size.X, Globals.DEBUG_UI_MENUBAR_SIZE_Y);
        parent.Size = new Vector2(Globals.APP_Width - parent.Pos.X, Globals.APP_Height - Globals.DEBUG_UI_MENUBAR_SIZE_Y);
        
        if (ImGui.BeginTabBar("ToolsTabBar", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
        {
            foreach (var tab in tabs)
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

    public static void AddTab(ToolTab newTab)
    {
        foreach (var tab in tabs)
        {
            if (tab.name == newTab.name)
                return;
        }

        tabs.Add(newTab);
    }
}