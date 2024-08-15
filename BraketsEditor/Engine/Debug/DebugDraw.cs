using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class DebugDraw
{
    public static void DrawRect(Rectangle rect, Color color)
    {
        Globals.ENGINE_SpriteBatch.Draw(ResourceLoader.GetTexture("_debug_draw_rect"), rect, color);
    }
}