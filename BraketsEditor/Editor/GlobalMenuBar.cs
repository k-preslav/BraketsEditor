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
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New..."))
                {
                    // Handle new menu item click
                }
                if (ImGui.MenuItem("Open..."))
                {
                    // Handle open menu item click
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
                if (ImGui.MenuItem("Application Properties"))
                {
                    Debug.Log("Change Application Props");
                }
                if (ImGui.MenuItem("Preferences"))
                {
                    Debug.Log("Change Editor Preferences");
                }
                ImGui.Separator();
                if (ImGui.BeginMenu("Style"))
                {
                    if (ImGui.MenuItem("Visual Studio")) DebugWindowStyle.VisualStudio();
                    if (ImGui.MenuItem("Classic Valve")) DebugWindowStyle.ClassicValve();
                    
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Tools"))
            {
                if (ImGui.MenuItem("Diagnostics"))
                {
                    MainToolsWindow.AddTab(new ToolTab{
                        name = "Diagnostics",
                        view = DiagnosticsView.Draw
                    });
                }
                if (ImGui.MenuItem("Level Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab{
                        name = "Level Editor",
                        view = DiagnosticsView.Draw
                    });
                }
                if (ImGui.MenuItem("Tilemap Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab{
                        name = "Tilemap Editor",
                        view = DiagnosticsView.Draw
                    });
                }
                if (ImGui.MenuItem("Particle Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab{
                        name = "Particle Editor",
                        view = DiagnosticsView.Draw
                    });
                }
                ImGui.EndMenu();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5, 0).ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 0).ToNumerics());
            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - 300) / 2);
            if (ImGui.Button("Build", new Vector2(55, Globals.DEBUG_UI_MENUBAR_SIZE_Y - 1).ToNumerics()))
            {
                Debug.Log("Building application...");
                BuildManager.Build();
            }
            if (ImGui.Button(BuildManager.runButtonText, new Vector2(55, Globals.DEBUG_UI_MENUBAR_SIZE_Y - 1).ToNumerics()))
            {
                Debug.Log("Running application...");
                BuildManager.Run();
            }
            ImGui.PopStyleVar(2);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5, 0).ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 0).ToNumerics());
            ImGui.SetCursorPosX(ImGui.GetWindowSize().X - ImGui.CalcTextSize(Globals.EditorManager.Status).X - 10);
            ImGui.Text(Globals.EditorManager.Status);
            ImGui.PopStyleVar(2);
            
            Globals.DEBUG_UI_MENUBAR_SIZE_X = (int)ImGui.GetWindowSize().X;
            Globals.DEBUG_UI_MENUBAR_SIZE_Y = (int)ImGui.GetWindowSize().Y;
            ImGui.EndMainMenuBar();
        }
    }
}