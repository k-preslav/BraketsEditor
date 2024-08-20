using System;
using System.Numerics;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class NewObjectWindow
{    
    private static int _selectedItem = 0;
    private static string[] _items = {
        "Sprite",
        "Particle Emitter",
        "Sound / Song",
        "User Interface View"
    };

    private static string objName = "";

    public static void Draw()
    {
        ImGui.ListBox("", ref _selectedItem, _items, _items.Length);

        // ImGui.SameLine(); // TODO:Add images to different items
        // ImGui.Image();

        // Properties
        ImGui.Dummy(new Vector2(10));
        ImGui.SeparatorText("Properties");

        ImGui.InputText("Name", ref objName, 32);

        // Create button
        ImGui.SetCursorPos(new Vector2(
            ImGui.GetWindowSize().X  - 110,
            ImGui.GetWindowSize().Y  - 45
        ));

        if (ImGui.Button("Create New"))
        {
            if (objName == "")
                return;

            objName = objName.Replace(" ", "");
            if (_selectedItem == 0) ObjectCreator.CreateSprite(objName);
        
            Close();
        }

        // Cancel button
        ImGui.SetCursorPos(new Vector2(
            ImGui.GetWindowSize().X  - 180,
            ImGui.GetWindowSize().Y  - 45
        ));

        if (ImGui.Button("Cancel"))
        {
            Close();
        }
    }

    private static void Close()
    {
        objName = "";
        _selectedItem = 0;

        Globals.DEBUG_UI.GetWindow("Add new Object").Visible = false;
    }
}