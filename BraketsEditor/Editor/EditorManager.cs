using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using BraketsEditor;
using Microsoft.Xna.Framework.Input;

public class EditorManager
{
    public BridgeServer bridgeServer;
    public string Status;

    public void Start()
    {
        Globals.EditorManager = this;
        Globals.DEBUG_Overlay = false;

        Globals.Camera.BackgroundColor = new Color(25, 25, 25);

        DebugWindowStyle.VisualStudio();
        Globals.DEBUG_UI.AddMenuBar(GlobalMenuBar.Draw);

        DebugWindow objectsWindow = new DebugWindow("Objects", overridePos: true, overrideSize: true,
            flags:ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar
        );
        objectsWindow.OnDraw += (DebugWindow parent) => {ObjectsWindow.Draw(parent);};
        ObjectsWindow.Refresh();

        DebugWindow newObjWindow = new DebugWindow("Add new Object", overridePos: false, overrideSize: false,
            closable:true, visible:false, flags: ImGuiWindowFlags.NoCollapse
        );
        newObjWindow.OnDraw += (DebugWindow parent) => {NewObjectWindow.Draw();};

        DebugWindow toolsWindow = new DebugWindow("Tools", overridePos: true, overrideSize: true,
            flags:ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar);
        toolsWindow.OnDraw += MainToolsWindow.Draw;
        
        MainToolsWindow.AddTab(new ToolTab{
            name = "Diagnostics",
            view = DiagnosticsView.Draw
        });

        DebugWindow gamePropWin = new DebugWindow("Game Properties", overridePos: true, overrideSize: true,
            closable:true, visible:false, flags: ImGuiWindowFlags.NoCollapse
        );
        gamePropWin.OnDraw += (DebugWindow parent) => {GamePropertiesWindow.Draw(parent);};

        ObjectCreator.SetupFileWatcher();

        Globals.DEBUG_UI.AddWindow(objectsWindow);
        Globals.DEBUG_UI.AddWindow(toolsWindow);
        Globals.DEBUG_UI.AddWindow(newObjWindow);
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

    }

    internal void Stop()
    {
                
    }
}
