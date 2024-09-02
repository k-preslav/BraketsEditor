using BraketsEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor.Editor;

public class PluginAbstraction
{
    public static DebugWindow MakeWindow(string name, Action<DebugWindow> draw, Action update, 
        ImGuiWindowFlags _flags = ImGuiWindowFlags.None, bool _overridePos=false, bool _overrideSize=false, 
        bool _closable=true, bool _visible=true, int _widht=800, int _height=600)
    {
        var window = new DebugWindow(name, overridePos:_overridePos, overrideSize:_overrideSize, 
            width: _widht, height: _height, 
            closable:_closable, visible:_visible, flags:_flags
        );

        window.OnDraw = (DebugWindow window) => { draw.Invoke(window); };
        window.OnUpdate = () => { update.Invoke(); };
    
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
