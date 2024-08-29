using System;
using System.Numerics;
using BraketsEditor.Editor.Contents.ContentPicker;
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
    private static string[] _itemsThumbnails = {
        "ui/addObject/sprite-object",
        "builtin/default_texture",
        "ui/addObject/audio-object",
        "builtin/default_texture"
    };

    private static string objName = "New Object";
    private static string objTag = "new_obj";
    private static int objLayer = 0;
    private static string objTexture = "builtin/default_texture";
    private static float objScale = 1;

    private static bool objSMU= true;
    private static bool objDOL= false;

    public static void Draw(DebugWindow parent)
    {
        ImGui.ListBox("", ref _selectedItem, _items, _items.Length);

        ImGui.SameLine(); ImGui.Spacing(); ImGui.SameLine();
        ImGui.Image(ResourceManager.GetImGuiTexture(_itemsThumbnails[_selectedItem]), new Vector2(128));

        // Properties
        ImGui.Dummy(new Vector2(10));
        ImGui.SeparatorText("Properties");

        ImGui.InputText("Name", ref objName, 32);
        ImGui.InputText("Tag", ref objTag, 32);
        ImGui.InputInt("Layer", ref objLayer);
        ImGui.InputFloat("Scale", ref objScale);

        ImGui.Spacing();
        ImGui.SeparatorText("Texture");

        //ImGui.Spacing();
        //ImGui.Image(ResourceManager.GetImGuiTexture("ui/addObject/browse"), new Vector2(32));
        //ImGui.SameLine();
        if (ImGui.Button("Pick Texture", new Vector2(200, 35)))
        {
            parent.TopMost = false;
            ContentPicker.Show(ContentType.Image, (string textureName) =>
            {
                parent.TopMost = true;
                objTexture = textureName;
            });
        }
        ImGui.Spacing();
        ImGui.BeginChild("texture_box", new Vector2(79), ImGuiChildFlags.Border, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        ImGui.Image(ResourceManager.GetImGuiTexture(objTexture), new Vector2(64));
        ImGui.EndChild();
        ImGui.SameLine();
        ImGui.Text(objTexture);

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        bool isAdvancedOpen = ImGui.TreeNodeEx("Advanced", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.Framed);
        if (isAdvancedOpen)
        {
            ImGui.Checkbox("Smart Update", ref objSMU);
            ImGui.Checkbox("Draw On Loading", ref objDOL);

            ImGui.TreePop();
        }

        // Create button
        ImGui.SetCursorPos(new Vector2(
            ImGui.GetWindowSize().X  - 125,
            ImGui.GetWindowSize().Y  - 45
        ));

        WindowTheme.PushAccent();
        if (ImGui.Button("Create New", new Vector2(115, 35)))
        {
            if (objName == "")
                return;

            objName = objName.Replace(" ", "");
            if (_selectedItem == 0) ObjectCreator.CreateSprite(objName, objTag, objTexture, objScale, objLayer, objSMU, objDOL);
        
            Close();
        }
        WindowTheme.PopAccent();

        // Cancel button
        ImGui.SetCursorPos(new Vector2(
            ImGui.GetWindowSize().X  - 210,
            ImGui.GetWindowSize().Y  - 45
        ));

        if (ImGui.Button("Cancel", new Vector2(75, 35)))
        {
            Close();
        }
    }

    private static void Close()
    {
        objName = "New Object";
        objTag = "new_obj";
        objLayer = 0;
        objTexture = "builtin/default_texture";
        objScale = 1;

        objSMU = true;
        objDOL = false;
        
        _selectedItem = 0;

        Globals.DEBUG_UI.GetWindow("Add new Object").Visible = false;
    }
}