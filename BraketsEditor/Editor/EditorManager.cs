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

        Status = "Loading Editor...";
        Throbber.visible = true;

        var loadingBox = new LoadingBox("Loading editor...");
        loadingBox.Show();

        WindowTheme.Dark();
        new GlobalMenuBar().Create();

        Globals.GlobalMenuBar.AddMenu("File");
        Globals.GlobalMenuBar.AddSubMenu("New...", "File", (sender) =>
        {
            if (Globals.IS_DEV_BUILD)
            {
                Debug.Warning("Cannot create new project! This is a DEV_BUILD!");
                new MessageBox("Operation could not be completed!\nRunning a DEV_BUILD!").Show();
                return;
            }
            ProjectManager.NewProject();
        });
        Globals.GlobalMenuBar.AddSubMenu("Open...", "File", (sender) =>
        {
            if (Globals.IS_DEV_BUILD)
            {
                Debug.Warning("Cannot open a project! This is a DEV_BUILD!");
                new MessageBox("Operation could not be completed!\nRunning a DEV_BUILD!").Show();
                return;
            }

            ProjectManager.OpenProject();
        });
        Globals.GlobalMenuBar.AddSubMenu("Open 'Content' in File Explorer", "File", (sender) =>
        {
            OpenInExplorer.OpenContentFolder();
        });
        Globals.GlobalMenuBar.AddSubMenu("Open 'Game' in File Explorer", "File", (sender) =>
        {
            OpenInExplorer.OpenGameFolder();
        });
        Globals.GlobalMenuBar.AddSubMenu("Exit", "File", (sender) =>
        {
            Globals.ENGINE_Main.Exit();
        });

        Globals.GlobalMenuBar.AddMenu("Edit");
        Globals.GlobalMenuBar.AddSubMenu("Game Properties", "Edit", (sender) =>
        {
            GamePropertiesWindow.LoadProperties();
            Globals.DEBUG_UI.GetWindow("Game Properties").Visible = true;
        });
        Globals.GlobalMenuBar.AddSubMenu("Preferences", "Edit", (sender) =>
        {
            Debug.Log("Open game preferences");
        });

        Globals.GlobalMenuBar.AddMenu("Addons");
        Globals.GlobalMenuBar.AddSubMenu("Extension Manager", "Addons", (sender) =>
        {
            PluginManagerWin.Create();
        });

        Globals.GlobalMenuBar.AddControlButton("runControl", "ui/run/run", (sender) =>
        {
            if (BuildManager.isDoneBuilding && BuildManager.isDoneRunning)
            {
                BuildManager.OnRunBtnClick(false);
            }
            else
            {
                Throbber.visible = false;
                BuildManager.runButtonText = "Run";

                BuildManager.Run(); // If it is performed while running, it will stop current run
            }
        });
        Globals.GlobalMenuBar.AddControlButton("runDebugControl", "ui/run/runDebug", (sender) =>
        {
            if (BuildManager.isDoneBuilding && BuildManager.isDoneRunning)
            {
                BuildManager.OnRunBtnClick(true);
                ((UIImage)sender).SetImage("ui/run/stop");
            }
            else
            {
                Throbber.visible = false;
                BuildManager.runButtonText = "Run";
                ((UIImage)sender).SetImage("ui/run/runDebug");
                BuildManager.RunDebug(); // If it is performed while running, it will stop current run
            }
        });

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

        if (BuildManager.isRunningDebug)
        {
            BuildManager.runningTimer += dt;
        }

        Globals.GlobalMenuBar.Update();
    }

    internal void OnResize()
    {
        ParticleEditor.OnAppResize();
        Globals.GlobalMenuBar?.OnAppResize();
    }

    internal async void Stop()
    {
        await gameDebug_bridgeServer.SendMessageToClient("stop");
        gameDebug_bridgeServer.Stop();

        DiagnosticsView.ResetFull();
        await ParticleEditor.Unload();
    }
}
