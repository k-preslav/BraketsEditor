using BraketsEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor.Editor;

internal class Throbber
{
    public static bool visible = false;

    public static void Draw(float x, float y, int _size=28)
    {
        if (!visible)
            return;

        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        IntPtr texture = ResourceManager.GetImGuiTexture(WindowTheme.currentTheme == "dark" ? "ui/throbber/throbber-white" : "ui/throbber/throbber-black");
        Vector2 size = new Vector2(_size);
        Vector2 center = new Vector2(x, y);

        float angle = (float)ImGui.GetTime() * 6;

        Vector2[] vertices = new Vector2[4];
        vertices[0] = new Vector2(-size.X / 2, -size.Y / 2);
        vertices[1] = new Vector2(size.X / 2, -size.Y / 2);
        vertices[2] = new Vector2(size.X / 2, size.Y / 2);
        vertices[3] = new Vector2(-size.X / 2, size.Y / 2);

        for (int i = 0; i < 4; i++)
        {
            float xPos = vertices[i].X;
            float yPos = vertices[i].Y;

            vertices[i].X = center.X + (xPos * MathF.Cos(angle) - yPos * MathF.Sin(angle));
            vertices[i].Y = center.Y + (xPos * MathF.Sin(angle) + yPos * MathF.Cos(angle));
        }

        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(0, 1);

        drawList.AddImageQuad(texture, vertices[0], vertices[1], vertices[2], vertices[3], uvs[0], uvs[1], uvs[2], uvs[3], 0xFFFFFFFF);
    }
}
