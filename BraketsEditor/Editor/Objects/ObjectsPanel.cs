using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Transactions;
using BraketsEngine;
using FontStashSharp.Rasterizers.StbTrueTypeSharp;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace BraketsEditor;

public class ObjectsPanel
{
    static List<ObjectButton> objects = new List<ObjectButton>();

    public static void Draw(DebugWindow parent)
    {
        parent.Pos = new Vector2(0, Globals.DEBUG_UI_MENUBAR_SIZE_Y);
        parent.Size = new Vector2(300, Globals.APP_Height / 2 - Globals.DEBUG_UI_MENUBAR_SIZE_Y);

        ImGui.SeparatorText("Objects");
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 10);
        if (ImGui.SmallButton("R")) Refresh();

        if (Input.IsMouseClicked(1) && Input.GetMousePosition().X < parent.Size.X && Input.GetMousePosition().Y < parent.Size.Y)
        {
            ImGui.OpenPopup("Create Class Popup");
        }

        if (ImGui.BeginPopup("Create Class Popup"))
        {
            if (ImGui.Button("Create New...", new Vector2(150, 25).ToNumerics()))
            {
                Globals.DEBUG_UI.GetWindow("Add new Object").Visible = true;
            }
            ImGui.EndPopup();
        }

        for (int i = 0; i < objects.Count; i++)
        {
            ImGui.PushID($"BUTTON {i}");
            objects[i].Draw(parent);
            ImGui.PopID();
        }
    }

    public static void Refresh()
    {
        objects.Clear();

        string[] files = Directory.GetFiles(Globals.projectGameFolderPath, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string fullPath = Path.GetFullPath(file);

            objects.Add(
                new ObjectButton(name, fullPath)
            );
        }
    }
}