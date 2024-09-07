using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace BraketsEditor;

public class GlobalMenuBar
{
    public static void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            ImGui.SetNextFrameWantCaptureKeyboard(false);
            ImGui.SetNextFrameWantCaptureMouse(false);

            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New..."))
                {
                    if (Globals.IS_DEV_BUILD)
                    {
                        Debug.Warning("Cannot create new project! This is a DEV_BUILD!");
                        new MessageBox("Operation could not be completed!\nRunning a DEV_BUILD!").Show();
                        return;
                    }
                    ProjectManager.NewProject();
                }
                if (ImGui.MenuItem("Open..."))
                {
                    if (Globals.IS_DEV_BUILD)
                    {
                        Debug.Warning("Cannot open a project! This is a DEV_BUILD!");
                        new MessageBox("Operation could not be completed!\nRunning a DEV_BUILD!").Show();
                        return;
                    }

                    ProjectManager.OpenProject();
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Open 'Content' in File Explorer"))
                {
                    OpenInExplorer.OpenContentFolder();
                }
                if (ImGui.MenuItem("Open 'Game' in File Explorer"))
                {
                    OpenInExplorer.OpenGameFolder();
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Exit"))
                {
                    Globals.ENGINE_Main.Exit();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Game Properties"))
                {
                    Globals.DEBUG_UI.GetWindow("Game Properties").Visible = true;
                    GamePropertiesWindow.LoadProperties();
                }
                if (ImGui.MenuItem("Preferences"))
                {
                    Debug.Log("Change Editor Preferences");
                }
                ImGui.Separator();
                if (ImGui.BeginMenu("Style"))
                {
                    if (ImGui.MenuItem("Visual Studio")) WindowTheme.Dark();
                    if (ImGui.MenuItem("Light")) WindowTheme.Light();

                    if (ImGui.Checkbox("Rounded", ref WindowTheme.rounded))
                        WindowTheme.Refresh();

                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Plugins"))
            {
                if (ImGui.MenuItem("Plugin Manager"))
                {
                    PluginManagerWin.Create();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Tools"))
            {
                if (ImGui.MenuItem("Diagnostics"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Diagnostics",
                        view = DiagnosticsView.Draw,
                        update = DiagnosticsView.Update
                    });
                }
                if (ImGui.MenuItem("Level Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Level Editor",
                        view = LevelEditor.Draw,
                        update = LevelEditor.Update
                    });
                }
                if (ImGui.MenuItem("Tilemap Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Tilemap Editor",
                        view = DiagnosticsView.Draw
                    });
                }
                if (ImGui.MenuItem("Particle Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Particle Editor",
                        view = ParticleEditor.Draw,
                        update = ParticleEditor.Update
                    });
                    ParticleEditor.Init(type: "new");
                }
                ImGui.EndMenu();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5, 0).ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 0).ToNumerics());

            ImGui.SetCursorPosX(275);
            ImGui.Text("|"); ImGui.Text(Globals.projectName);

            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - 300) / 2);

            if (!BuildManager.isDoneRunning) ImGui.BeginDisabled();
            if (ImGui.ImageButton("###runButton", ResourceManager.GetImGuiTexture("ui/run/run"), new Vector2(20).ToNumerics()))
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
            }
            if (!BuildManager.isDoneRunning) ImGui.EndDisabled();


            if (!BuildManager.isDoneRunning && !BuildManager.isRunningDebug) ImGui.BeginDisabled();
            if (ImGui.ImageButton("###runDebugButton", ResourceManager.GetImGuiTexture(!BuildManager.isDoneRunning ? "ui/run/stop" : "ui/run/runDebug"), new Vector2(20).ToNumerics()))
            {
                if (BuildManager.isDoneBuilding && BuildManager.isDoneRunning)
                {
                    BuildManager.OnRunBtnClick(true);
                }
                else
                {
                    Throbber.visible = false;
                    BuildManager.runButtonText = "Run";
                    BuildManager.RunDebug(); // If it is performed while running, it will stop current run
                }
            }
            ImGui.EndDisabled();
            BuildManager.runningTimer += Globals.DEBUG_DT;
            ImGui.PopStyleVar(2);

            if (Globals.IS_DEV_BUILD)
            {
                ImGui.SetCursorPosX((ImGui.GetWindowSize().X - 500));
                ImGui.TextColored(Color.Gray.ToVector4().ToNumerics(), "DEV_BUILD");
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5, 0).ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 0).ToNumerics());
            ImGui.SetCursorPosX(ImGui.GetWindowSize().X - ImGui.CalcTextSize(Globals.EditorManager.Status).X - 10);
            Throbber.Draw(ImGui.GetWindowSize().X - ImGui.CalcTextSize(Globals.EditorManager.Status).X - 35, 18);
            ImGui.Text(Globals.EditorManager.Status);
            ImGui.PopStyleVar(2);

            Globals.DEBUG_UI_MENUBAR_SIZE_X = (int)ImGui.GetWindowSize().X;
            Globals.DEBUG_UI_MENUBAR_SIZE_Y = (int)ImGui.GetWindowSize().Y;

            ImGui.EndMainMenuBar();
        }
    }
}