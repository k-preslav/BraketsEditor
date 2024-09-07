using System.IO;
using System.Numerics;
using BraketsPluginIntegration;
using BraketsEngine;
using ImGuiNET;
using BraketsEditor.Engine;

namespace BraketsEditor;

public class GamePropertiesWindow
{
    static string title = Globals.projectName,
                version = "",
                width = "",
                height = "",
                view_width = "",
                view_height = "";
    static bool resize = true,
                vsync = true;

    static DebugWindow parent;

    public static void Create()
    {
        parent = PluginAbstraction.MakeWindow("Game Properties", () =>
        {
            Draw();
        }, () => { }, _flags: ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize, _closable: true, _visible: false, _widht: 465, _height: 660, _overrideSize:true);
    }

    public static void Draw()
    {
        ImGui.SeparatorText("General");
        ImGui.SetNextItemWidth(200);
        ImGui.InputText("Game Title", ref title, 32);
        ImGui.SetNextItemWidth(200);
        ImGui.InputText("Game Version", ref version, 32);

        ImGui.Spacing();
        ImGui.SeparatorText("Window Size");
        ImGui.Spacing(); 
        ImGui.SetNextItemWidth(100);
        ImGui.InputText("Win Width", ref width, 4, ImGuiInputTextFlags.CharsDecimal);
        ImGui.SetNextItemWidth(100);
        ImGui.InputText("Win Height", ref height, 4, ImGuiInputTextFlags.CharsDecimal);

        ImGui.Spacing();
        ImGui.SeparatorText("Viewport Settings");
        ImGui.Spacing();
        ImGui.SetNextItemWidth(100);
        ImGui.InputText("View Width", ref view_width, 4, ImGuiInputTextFlags.CharsDecimal);
        ImGui.SetNextItemWidth(100);
        ImGui.InputText("View Height", ref view_height, 4, ImGuiInputTextFlags.CharsDecimal);
        ImGui.SetNextItemWidth(250);
        Vector4 viewportColor = Globals.projectViewportColor.ToNumerics();
        ImGui.ColorEdit4("Camera Color", ref viewportColor);
        Globals.projectViewportColor = viewportColor;

        ImGui.Spacing();
        ImGui.SeparatorText("Window Options");
        ImGui.Spacing(); 
        ImGui.Checkbox("Resizable", ref resize);
        ImGui.Checkbox("VSync", ref vsync);

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.SetCursorPos(new Vector2(parent.Size.X - 110, ImGui.GetWindowHeight() - 45));
        WindowTheme.PushAccent();
        if (ImGui.Button("Save", new Vector2(100, 35)))
        {
            Save();
            parent.Visible = false;
        }
        WindowTheme.PopAccent();
    }

    private static async void Save()
    {
        string path = $"{Globals.projectPath}/game.properties";
        string[] data = new string[] {
            $"engine_ver:{Globals.APP_Version}",
            $"app_ver:{version}", 
            $"app_title:{title}", 
            $"app_width:{width}", 
            $"app_height:{height}",
            $"app_view_width:{view_width}",
            $"app_view_height:{view_height}",
            $"app_view_color:{Globals.projectViewportColor.ToNumerics()}",
            $"app_resizable:{resize}",
            $"app_vsync:{vsync}"
        };

        Globals.projectViewportWidth = int.Parse(view_width);
        Globals.projectViewportHeight = int.Parse(view_height);

        await File.WriteAllLinesAsync(path, data);
        Debug.Log("Saved game properties.");
    }

    internal static void LoadProperties()
    {
        string path = $"{Globals.projectPath}/game.properties";
        string[] data = File.ReadAllLines(path);


        foreach (string line in data)
        {
            string key = line.Split(":")[0];
            string value = line.Split(":")[1];

            switch (key)
            {
                case "app_ver":
                    version = value;
                    break;
                case "app_title":
                    title = value;
                    break;
                case "app_width":
                    width = value;
                    break;
                case "app_height":
                    height = value;
                    break;
                case "app_view_width":
                    view_width = value;
                    break;
                case "app_view_height":
                    view_height = value;
                    break;
                case "app_view_color":
                    Globals.projectViewportColor = Parser.ParseVec4(value);
                    break;
                case "app_resizable":
                    resize = bool.Parse(value);
                    break;
                case "app_vsync":
                    vsync = bool.Parse(value);
                    break;
            }
        }

        Globals.projectViewportWidth = int.Parse(view_width);
        Globals.projectViewportHeight = int.Parse(view_height);

        Debug.Log("Loaded game properties.");
    }
}