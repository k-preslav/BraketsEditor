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

    public static void AddMenuToMenuBar(string name) => Globals.GlobalMenuBar.AddMenu(name);
    public static void AddSubMenuToMenuBar(string name, string menu, Action<object> click) => Globals.GlobalMenuBar.AddSubMenu(name, menu, click);
    public static void AddControlButton(string tag, string imageName, Action<object> click) => Globals.GlobalMenuBar.AddControlButton(tag, imageName, click);
    public static UIImage FindControlButton(string tag) => Globals.GlobalMenuBar.GetControlButton(tag);
}
