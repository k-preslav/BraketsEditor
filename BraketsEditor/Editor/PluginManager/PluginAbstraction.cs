using BraketsEditor;
using BraketsEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace BraketsPluginIntegration;

public class PluginAbstraction
{
    public static DebugWindow MakeWindow(string name, Action draw, Action update,
        ImGuiWindowFlags _flags = ImGuiWindowFlags.None, bool _overridePos = false, bool _overrideSize = false,
        bool _closable = true, bool _visible = true, int _widht = 800, int _height = 600, int _posx = 100, int _posy = 100)
    {
        var window = new DebugWindow(name, overridePos: _overridePos, overrideSize: _overrideSize,
            width: _widht, height: _height, posx: _posx, posy: _posy,
            closable: _closable, visible: _visible, flags: _flags
        );

        window.OnDraw = () => { draw?.Invoke(); };
        window.OnUpdate = () => { update?.Invoke(); };

        Globals.DEBUG_UI.AddWindow(window);
        return window;
    }
    public static DebugWindow GetWindow(string name)
    {
        return Globals.DEBUG_UI.GetWindow(name);
    }

    public static void ShowToolView(string name, Action draw, Action update)
    {
        var tab = new ToolTab
        {
            name = name,
            view = draw,
            update = update
        };

        MainToolsWindow.AddTab(tab);
    }
}
