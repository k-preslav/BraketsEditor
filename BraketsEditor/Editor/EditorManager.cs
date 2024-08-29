using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using BraketsEditor;
using Microsoft.Xna.Framework.Input;
using BraketsEditor.Editor;
using BraketsEditor.Editor.Contents.AddContentWindow;
using BraketsEditor.Editor.Contents.ContentPicker;

public class EditorManager
{
    public BridgeServer bridgeServer;
    public string Status;

    public void Start()
    {
        Globals.EditorManager = this;
        Globals.DEBUG_Overlay = false;

        Globals.Camera.BackgroundColor = new Color(25, 25, 25);

        WindowTheme.Dark();
        Globals.DEBUG_UI.AddMenuBar(GlobalMenuBar.DrawAsync);

        DebugWindow objectsPanel = new DebugWindow("Objects", overridePos: true, overrideSize: true,
            flags:ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar
        );
        objectsPanel.OnDraw += (DebugWindow parent) => {ObjectsPanel.Draw(parent);};
        ObjectsPanel.Refresh();

        DebugWindow contentPanel = new DebugWindow("Content", overridePos: true, overrideSize: true,
            flags: ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar
        );
        contentPanel.OnDraw += (DebugWindow parent) => { ContentPanel.Draw(parent); };
        ContentPanel.Refresh("/");

        DebugWindow newObjWindow = new DebugWindow("Add new Object", overridePos: false, overrideSize: true, width:475, height:675,
            closable:true, topmost: true, visible:false, flags: ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize
        );
        newObjWindow.OnDraw += (DebugWindow parent) => {NewObjectWindow.Draw(parent);};

        DebugWindow newContentWindow = new DebugWindow("Add new Content", overridePos: false, overrideSize: true, width:550, height:225,
            closable: true, topmost: true, visible: false, flags: ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse
        );
        newContentWindow.OnDraw += (DebugWindow parent) => { AddContentWindow.Draw(parent); };

        DebugWindow contentPicker = new DebugWindow("Content Picker", overridePos: false, overrideSize: true, width: 580, height: 400,
            closable: true, topmost: true, visible: false, flags: ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize
        );
        contentPicker.OnDraw += (DebugWindow parent) => { ContentPicker.Draw(parent); };

        DebugWindow toolsWindow = new DebugWindow("Tools", overridePos: true, overrideSize: true,
            flags:ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar);
        toolsWindow.OnDraw += MainToolsWindow.Draw;
        
        MainToolsWindow.AddTab(new ToolTab{
            name = "Diagnostics",
            view = DiagnosticsView.Draw
        });

        DebugWindow gamePropWin = new DebugWindow("Game Properties", overridePos: true, overrideSize: true,
            closable:true, topmost: true, visible:false, flags: ImGuiWindowFlags.NoCollapse
        );
        gamePropWin.OnDraw += (DebugWindow parent) => {GamePropertiesWindow.Draw(parent);};

        ObjectCreator.SetupFileWatcher();

        Globals.DEBUG_UI.AddWindow(objectsPanel);
        Globals.DEBUG_UI.AddWindow(contentPanel);
        Globals.DEBUG_UI.AddWindow(toolsWindow);
        Globals.DEBUG_UI.AddWindow(newObjWindow);
        Globals.DEBUG_UI.AddWindow(newContentWindow);
        Globals.DEBUG_UI.AddWindow(contentPicker);
        Globals.DEBUG_UI.AddWindow(gamePropWin);

        bridgeServer = new BridgeServer(8000);
        bridgeServer.OnRecieve += OnBridgeDataRecive;
        bridgeServer.Start();

        Status = "Ready";
    }

    private void OnBridgeDataRecive(string recieveData)
    {
        string[] split = recieveData.Split("|");

        DiagnosticsView.currentFps = float.Parse(split[0]);
        DiagnosticsView.currentMemory = float.Parse(split[1]);
        DiagnosticsView.currentSpriteCount = float.Parse(split[2]);
        DiagnosticsView.currentGC = float.Parse(split[3]);
        DiagnosticsView.currentThreadsCount = float.Parse(split[4]);
        DiagnosticsView.currentDt = float.Parse(split[5]);
    }

    public void Update(float dt)            
    {
        // Shortcuts
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.N, ImGuiInputFlags.RouteGlobal)) Globals.DEBUG_UI.GetWindow("Add new Object").Visible = true;
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.R, ImGuiInputFlags.RouteGlobal))
        {
            ObjectsPanel.Refresh();
            ContentPanel.Refresh("last");
        }
    }

    internal void Stop()
    {
                
    }
}
