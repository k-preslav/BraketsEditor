using BraketsEngine;
using ImGuiNET;
using System.Drawing;
using System.Numerics;

namespace BraketsEditor;

public class Gizmo
{
    public static float dragSpeed = 1;
    
    static bool isDraggingX = false;
    static bool isDraggingY = false;
    static bool isDraggingButton = false;

    static Vector2 dragStartPos = Vector2.Zero;

    public static void DrawGizmo(ref Vector2 vector, Vector2 position, float arrowSize, ImDrawListPtr drawList)
    {
        ImGui.SetCursorPos(position);

        Vector4 xAxisBaseColor = new Vector4(1f, 0f, 0f, 1f);
        Vector4 yAxisBaseColor = new Vector4(0f, 1f, 0f, 1f);

        Vector4 xAxisColor = isDraggingX ? new Vector4(xAxisBaseColor.X, xAxisBaseColor.Y, xAxisBaseColor.Z, 0.5f) : xAxisBaseColor;
        Vector4 yAxisColor = isDraggingY ? new Vector4(yAxisBaseColor.X, yAxisBaseColor.Y, yAxisBaseColor.Z, 0.5f) : yAxisBaseColor;

        uint xAxisColorU32 = ImGui.GetColorU32(xAxisColor);
        uint yAxisColorU32 = ImGui.GetColorU32(yAxisColor);

        Vector2 xAxisEnd = position + new Vector2(arrowSize, 0);
        drawList.AddLine(position, xAxisEnd, xAxisColorU32, 2f);
        drawList.AddCircleFilled(xAxisEnd, 5f, xAxisColorU32);

        Vector2 buttonSize = new Vector2(16, 16);
        Vector2 buttonPosition = position + new Vector2(arrowSize / 2 - buttonSize.X / 2, -arrowSize / 2 - buttonSize.Y / 2);
        drawList.AddRectFilled(buttonPosition, buttonPosition + buttonSize, ImGui.GetColorU32(new Vector4(0.75f)));

        Vector2 yAxisEnd = position + new Vector2(0, -arrowSize);
        drawList.AddLine(position, yAxisEnd, yAxisColorU32, 2f);
        drawList.AddCircleFilled(yAxisEnd, 5f, yAxisColorU32);

        HandleDragging(ref vector, position, arrowSize, buttonPosition, buttonSize);
    }

    private static void HandleDragging(ref Vector2 vector, Vector2 position, float arrowSize, Vector2 buttonPosition, Vector2 buttonSize)
    {
        Vector2 mousePos = Input.GetMousePositionScreen().ToNumerics();

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && IsMouseOverLine(mousePos, position, position + new Vector2(arrowSize, 0), 5f))
        {
            isDraggingX = true;
            dragStartPos = mousePos;
        }

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && IsMouseOverLine(mousePos, position, position + new Vector2(0, -arrowSize), 5f))
        {
            isDraggingY = true;
            dragStartPos = mousePos;
        }

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && IsMouseOverRect(mousePos, buttonPosition, buttonSize))
        {
            isDraggingButton = true;
            dragStartPos = mousePos;
        }

        if (isDraggingX && ImGui.IsMouseDragging(ImGuiMouseButton.Left))
        {
            Vector2 delta = ImGui.GetMousePos() - dragStartPos;
            vector.X += delta.X * dragSpeed;
            dragStartPos = ImGui.GetMousePos();
        }

        if (isDraggingY && ImGui.IsMouseDragging(ImGuiMouseButton.Left))
        {
            Vector2 delta = ImGui.GetMousePos() - dragStartPos;
            vector.Y += delta.Y * dragSpeed;
            dragStartPos = ImGui.GetMousePos();
        }

        if (isDraggingButton && ImGui.IsMouseDragging(ImGuiMouseButton.Left))
        {
            Vector2 delta = ImGui.GetMousePos() - dragStartPos;
            vector.X += delta.X * dragSpeed;
            vector.Y += delta.Y * dragSpeed;
            dragStartPos = ImGui.GetMousePos();
        }

        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            isDraggingX = false;
            isDraggingY = false;
            isDraggingButton = false;
        }
    }


    private static bool IsMouseOverRect(Vector2 mousePos, Vector2 rectPosition, Vector2 rectSize)
    {
        return mousePos.X >= rectPosition.X && mousePos.X <= rectPosition.X + rectSize.X &&
               mousePos.Y >= rectPosition.Y && mousePos.Y <= rectPosition.Y + rectSize.Y;
    }


    private static bool IsMouseOverLine(Vector2 mousePos, Vector2 lineStart, Vector2 lineEnd, float tolerance)
    {
        Vector2 lineDir = lineEnd - lineStart;
        Vector2 lineToPoint = mousePos - lineStart;
        float projectionLength = Vector2.Dot(lineToPoint, lineDir) / lineDir.LengthSquared();
        Vector2 projectedPoint = lineStart + projectionLength * lineDir;
        return Vector2.Distance(mousePos, projectedPoint) < tolerance;
    }
}
