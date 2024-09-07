using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor;

public class Grid
{
    public static void Draw(int resolutionWidth, int resolutionHeight, float gridSpacing, Vector2 offset, ImDrawListPtr drawList)
    {
        Vector2 gridSizeVec = new Vector2(resolutionWidth, resolutionHeight);
        Vector2 gridSpacingVec = new Vector2(gridSpacing, gridSpacing);

        Vector2 gridStart = new Vector2(0, 0) + offset;
        Vector2 gridEnd = new Vector2(resolutionWidth, resolutionHeight);

        for (float x = gridStart.X; x < gridEnd.X + offset.X; x += gridSpacingVec.X)
        {
            drawList.AddLine(new Vector2(x, gridStart.Y), new Vector2(x, gridEnd.Y + offset.Y), 0xFFAAAAAA);
        }

        for (float y = gridStart.Y; y < gridEnd.Y + offset.Y; y += gridSpacingVec.Y)
        {
            drawList.AddLine(new Vector2(gridStart.X, y), new Vector2(gridEnd.X + offset.X, y), 0xFFAAAAAA);
        }
    }
}
