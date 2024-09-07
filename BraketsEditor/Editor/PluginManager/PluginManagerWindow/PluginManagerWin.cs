using BraketsEngine;
using BraketsPluginIntegration;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor;

public class PluginManagerWin
{
    static DebugWindow win;

    public static void Create()
    {
        win = PluginAbstraction.MakeWindow("Plugin Manager", Draw, Update,
            _overrideSize: true, _widht: 400, _height: 600, _flags: ImGuiNET.ImGuiWindowFlags.NoResize
        );
    }

    static void Draw()
    {
        var plugins = Globals.EditorManager.pluginLoader.Plugins;
        if (plugins.Count == 0)
        {
            ImGui.Text("No plugins are loaded.");
            return;
        }

        ImGui.SeparatorText("Loaded plugins");
        foreach (var plugin in plugins.ToList())
        {
            ImGui.Text(plugin.Window.Name);
        }
    }

    static void Update()
    {

    }
}
