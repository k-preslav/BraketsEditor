using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace BraketsEditor;

public class objectsWindowDraw
{
    public static void Draw(DebugWindow parent)
    {
        parent.Pos = new Vector2(0, Globals.DEBUG_UI_MENUBAR_SIZE_Y);
        parent.Size = new Vector2(300, Globals.APP_Height - Globals.DEBUG_UI_MENUBAR_SIZE_Y);

        if (Input.IsMouseClicked(1) && Input.GetMousePosition().X < parent.Size.X)
        {
            ImGui.OpenPopup("NewObjectPopup");
        }

        if (ImGui.BeginPopup("NewObjectPopup"))
        {
            ImGui.Button("New Sprite", new Vector2(150, 25).ToNumerics());
            ImGui.Button("New Particle Emitter", new Vector2(150, 25).ToNumerics());
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Button("New Level", new Vector2(150, 25).ToNumerics());
            ImGui.Button("New Tilemap", new Vector2(150, 25).ToNumerics());
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
            
            ImGui.Button("New Sound", new Vector2(150, 25).ToNumerics());
            ImGui.Button("New Song", new Vector2(150, 25).ToNumerics());
            
            ImGui.EndPopup();
        }
    }
}