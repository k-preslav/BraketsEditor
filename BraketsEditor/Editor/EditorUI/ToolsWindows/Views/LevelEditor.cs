using BraketsEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor;

public class LevelEditor
{
    static Vector2 myVector = new Vector2(500, 300);
    static float arrowSize = 86;

    static float gizmoSpeed = 1f;

    static Vector2 startOffset = new Vector2(500, 300);
    
    static Vector2 panOffset = Vector2.Zero;
    static Vector2 lastMousePos = Vector2.Zero;

    static float zoom = 1f;

    public static void Draw()
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        // Calculate the clip rectangle (avoiding the menu bar with the tools tabs)
        Vector2 windowPos = ImGui.GetWindowPos();
        Vector2 windowSize = ImGui.GetWindowSize();
        Vector2 clipRectMin = windowPos + new Vector2(0, Globals.DEBUG_UI_MENUBAR_SIZE_Y + 10);
        Vector2 clipRectMax = windowPos + windowSize;

        drawList.PushClipRect(clipRectMin, clipRectMax);

        // Draw the level editor
        drawList.AddText((startOffset + panOffset) * zoom - new Vector2(-5, 30),
            ImGui.GetColorU32(Microsoft.Xna.Framework.Color.Gray.ToVector4().ToNumerics()),
            $"Game Viewport ({Globals.projectViewportWidth}x{Globals.projectViewportHeight})"
        );
        drawList.AddRectFilled((startOffset + panOffset) * zoom, (new Vector2(Globals.projectViewportWidth, Globals.projectViewportHeight) + panOffset + startOffset) * zoom, ImGui.GetColorU32(Globals.projectViewportColor.ToNumerics()));
        drawList.AddRect((startOffset + panOffset) * zoom, (new Vector2(Globals.projectViewportWidth, Globals.projectViewportHeight) + panOffset + startOffset) * zoom, 0xFFAAAAAA, 0, ImDrawFlags.None, 2.5f);

        ImGui.Text($"Vector: {myVector - startOffset}");
        ImGui.Text($"Zoom: {zoom}");

        Vector2 imageSize = new Vector2(64 * zoom);
        Vector2 imagePos = (myVector + panOffset) * zoom;
        drawList.AddImage(ResourceManager.GetImGuiTexture("builtin/default_texture"), imagePos, imagePos + imageSize);

        Gizmo.DrawGizmo(ref myVector, (myVector + panOffset) * zoom, arrowSize * zoom * 1.25f, drawList);
        Gizmo.dragSpeed = gizmoSpeed / (zoom);

        drawList.PopClipRect();

        if (ImGui.IsMouseDragging(ImGuiMouseButton.Right))
        {
            Vector2 currentMousePos = ImGui.GetMousePos();
            panOffset += (currentMousePos - lastMousePos) / zoom;
            lastMousePos = currentMousePos;
        }
        else
        {
            lastMousePos = ImGui.GetMousePos();
        }

        if (Input.GetScrollDelta() != 0)
        {
            float wheelDelta = Input.GetScrollDelta();
            zoom *= 1.0f + wheelDelta * 0.1f;
            zoom = Math.Max(0.1f, Math.Min(zoom, 10.0f));
        }
    }


    public static void Update()
    {
        
    }
}
