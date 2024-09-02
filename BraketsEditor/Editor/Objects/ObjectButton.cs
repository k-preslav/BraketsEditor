using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Numerics;
using BraketsEditor.Editor;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class ObjectButton
{
    public string Name;
    public string FilePath;
    public string Type;

    public string specialArgs;

    public ObjectButton(string name, string path, string type, string specialArgs="")
    {
        Name = name;
        FilePath = path;
        Type = type;
        
        this.specialArgs = specialArgs;
    }

    public void Draw(DebugWindow parent, bool hasScrollbar)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(3));
        ImGui.Button(Name, new Vector2(parent.Size.X - (hasScrollbar ? 95 : 85), 32));

        ImGui.SameLine();
        string textureName = WindowTheme.currentTheme == "dark" ? "ui/edit/edit-white" : "ui/edit/edit-black";
        if (ImGui.ImageButton("###edit_button", ResourceManager.GetImGuiTexture(textureName), new Vector2(16, 16)))
        {
            if (Type == "sprite") OpenFileInEditor.Open(FilePath, Globals.projectPath);
            else if (Type == "particles")
            {
                MainToolsWindow.AddTab(new ToolTab
                {
                    name = "Particle Editor",
                    view = ParticleEditor.Draw,
                    update = ParticleEditor.Update
                });

                ParticleEditor.Init(specialArgs);
            }
        }

        ImGui.SameLine();
        if (ImGui.ImageButton("###delete_button", ResourceManager.GetImGuiTexture("ui/reject/reject"), new Vector2(16, 16)))
        {
            var confirm = new MessageBox($"Are you sure you want to delete object '{Name}'", "No", "Yes", (result) =>
            {
                if (result == 2)
                {
                    ObjectCreator.RemoveFile(FilePath);
                }
            });
            confirm.Show();
        }

        ImGui.PopStyleVar();
    }
}