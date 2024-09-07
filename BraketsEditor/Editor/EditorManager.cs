using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using BraketsEditor;
using System.Threading.Tasks;
using BraketsPluginIntegration;

public class EditorManager
{
    public BridgeServer gameDebug_bridgeServer;
    public string Status = "{status}";

    public PluginLoader pluginLoader;

    public async Task Start()
    {
        Globals.EditorManager = this;
        Globals.DEBUG_Overlay = false;

        Globals.Camera.BackgroundColor = new Color(25, 25, 25);

        unsafe {

            ImGui.GetIO().NativePtr->IniFilename = null;
            ImGui.GetIO().NativePtr->IniSavingRate = 0;
        }

        Status = "Loading Editor...";
        Throbber.visible = true;

        var loadingBox = new LoadingBox("Loading editor...");
        loadingBox.Show();

        WindowTheme.Dark();
        Globals.DEBUG_UI.AddMenuBar(GlobalMenuBar.Draw);

        loadingBox.SetMessage("Creating Panels and Windows...");
        loadingBox.SetValue(10);
        ObjectsPanel.Create();
        ContentPanel.Create();
        NewObjectWindow.Create();
        AddContentWindow.Create();
        ContentPicker.Create();
        MainToolsWindow.Create();
        GamePropertiesWindow.Create();
        await Task.Delay(50);

        loadingBox.SetMessage("Loading Game Properties...");
        loadingBox.SetValue(35);
        GamePropertiesWindow.LoadProperties();
        await Task.Delay(50);

        loadingBox.SetValue(40);
        PluginAbstraction.ShowToolView("Diagnostics", DiagnosticsView.Draw, DiagnosticsView.Update);
        await Task.Delay(50);

        loadingBox.SetValue(45);
        loadingBox.SetMessage("Constructing Game_Debug Bridge...");

        gameDebug_bridgeServer = new BridgeServer("Game_Debug");
        gameDebug_bridgeServer.OnRecieve += OnBridgeDataRecive;
        gameDebug_bridgeServer.Start();
        Globals.GameDebugBridgeServer = gameDebug_bridgeServer;
        await Task.Delay(100);

        ObjectCreator.SetupFileWatcher();

        loadingBox.SetValue(50);
        loadingBox.SetMessage($"Loading plugins...");
        await Task.Delay(100);

        pluginLoader = new PluginLoader();
        await pluginLoader.LoadPlugins(loadingBox);
        pluginLoader.CallPInit();

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
        await gameDebug_bridgeServer.SendMessageToClient("stop");
        gameDebug_bridgeServer.Stop();

        DiagnosticsView.ResetFull();
        await ParticleEditor.Unload();
    }
}
