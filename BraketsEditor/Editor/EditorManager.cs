using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using BraketsEditor;
using Microsoft.Xna.Framework.Input;
using BraketsEditor.Editor;
using BraketsEditor.Editor.Contents.AddContentWindow;
using BraketsEditor.Editor.Contents.ContentPicker;
using NAudio.CoreAudioApi;
using System.Threading.Tasks;
using BraketsEditor.Editor.PluginsManager;

public class EditorManager
{
    public BridgeServer bridgeServer;
    public string Status = "{status}";

    public async void Start()
    {
        var loadingBox = new LoadingBox("We are loading!!!");
        loadingBox.Show();

        Globals.EditorManager = this;
        Globals.DEBUG_Overlay = false;

        Globals.Camera.BackgroundColor = new Color(25, 25, 25);

        Status = "Loading Editor...";
        Throbber.visible = true;

        WindowTheme.Dark();
        Globals.DEBUG_UI.AddMenuBar(GlobalMenuBar.Draw);

        ObjectsPanel.Create();
        ContentPanel.Create();
        NewObjectWindow.Create();
        AddContentWindow.Create();
        ContentPicker.Create();
        MainToolsWindow.Create();
        GamePropertiesWindow.Create();

        PluginAbstraction.ShowToolView("Diagnostics", DiagnosticsView.Draw, DiagnosticsView.Update);
        await Task.Delay(100);

        loadingBox.SetValue(35);

        bridgeServer = new BridgeServer(8000);
        bridgeServer.OnRecieve += OnBridgeDataRecive;
        bridgeServer.Start();
        loadingBox.SetValue(50);

        //PluginLoader.LoadPlugins();
        ObjectCreator.SetupFileWatcher();

        Status = "Ready";
        Throbber.visible = false;
        loadingBox.SetValue(100);
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

        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.F5, ImGuiInputFlags.RouteGlobal))
        {
            BuildManager.OnRunBtnClick(false);
        }
        else if (ImGui.Shortcut(ImGuiKey.F5, ImGuiInputFlags.RouteGlobal))
        {
            BuildManager.OnRunBtnClick(true);
        }
    }

    internal void OnResize()
    {
        ParticleEditor.OnAppResize();
    }

    internal async void Stop()
    {
        DiagnosticsView.ResetFull();
        await ParticleEditor.Unload();
    }
}
