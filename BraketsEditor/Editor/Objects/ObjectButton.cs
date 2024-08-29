using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Numerics;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class ObjectButton
{
    public string Name;
    public string FilePath;

    public ObjectButton(string name, string path)
    {
        Name = name;
        FilePath = path;
    }

    public void Draw(DebugWindow parent)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(3, 3));
        ImGui.Button(Name, new Vector2(parent.Size.X - 85, 32));

        ImGui.SameLine();
        if (ImGui.Button("E", new Vector2(30, 32)))
        {
            OpenFileInEditor.Open(FilePath, Globals.projectPath);
        }

        ImGui.SameLine();
        if (ImGui.Button("X", new Vector2(30, 32)))
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