
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using BraketsEditor.Editor;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class GamePropertiesWindow
{
    static string title = Globals.projectName, 
                version = "",
                width = "",
                height = "";
    static bool resize = true,
                vsync = true;

    public static void Create()
    {
        PluginAbstraction.MakeWindow("Game Properties", (window) =>
        {
            GamePropertiesWindow.Draw(window);
        }, () => { }, _flags: ImGuiWindowFlags.NoCollapse, _closable: true, _visible: false, _widht: 465, _height: 635, _overrideSize:true);
    }

    public static void Draw(DebugWindow parent)
    {
        parent.Size = new Vector2(385, 485);
        parent.Pos = new Vector2(Globals.APP_Width / 2 - parent.Size.X / 2, Globals.APP_Height / 2 - parent.Size.Y / 2);

        ImGui.SeparatorText("General");
        ImGui.SetNextItemWidth(200);
        ImGui.InputText("Game Title", ref title, 32);
        ImGui.SetNextItemWidth(200);
        ImGui.InputText("Game Version", ref version, 32);

        ImGui.Spacing();
        ImGui.SeparatorText("Window Size");
        ImGui.Spacing(); 
        ImGui.SetNextItemWidth(100);
        ImGui.InputText("Width", ref width, 4, ImGuiInputTextFlags.CharsDecimal);
        ImGui.SetNextItemWidth(100);
        ImGui.InputText("Height", ref height, 4, ImGuiInputTextFlags.CharsDecimal);

        ImGui.Spacing();
        ImGui.SeparatorText("Window Options");
        ImGui.Spacing(); 
        ImGui.Checkbox("Resizable", ref resize);
        ImGui.Checkbox("VSync", ref vsync);
        
        ImGui.SetCursorPos(new Vector2(parent.Size.X - 115, parent.Size.Y - 50));
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
            $"engine_ver,{Globals.APP_Version}",
            $"app_ver,{version}", 
            $"app_title,{title}", 
            $"app_width,{width}", 
            $"app_height,{height}",
            $"app_resizable,{resize}",
            $"app_vsync,{vsync}"
        };

        await File.WriteAllLinesAsync(path, data);
        Debug.Log("Saved game properties.");
    }

    internal static void LoadProperties()
    {
        string path = $"{Globals.projectPath}/game.properties";
        string[] data = File.ReadAllLines(path);


        foreach (string line in data)
        {
            string key = line.Split(",")[0];
            string value = line.Split(",")[1];

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
                case "app_resizable":
                    resize = bool.Parse(value);
                    break;
                case "app_vsync":
                    vsync = bool.Parse(value);
                    break;
            }
        }

        Debug.Log("Loaded game properties.");
    }
}