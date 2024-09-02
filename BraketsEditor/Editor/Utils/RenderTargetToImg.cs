using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BraketsEditor.Editor.Utils
{
    internal class RenderTargetToImg
    {
        internal static nint DrawImage(int previewSize)
        {
            int renderTargetWidth = Globals.APP_Width;
            int renderTargetHeight = Globals.APP_Height;

            // Calculate aspect ratio
            float renderTargetAspect = (float)renderTargetWidth / renderTargetHeight;
            float previewAspect = 1.0f;

            Vector2 uv0, uv1;
            if (renderTargetAspect > previewAspect)
            {
                // Wider image; fit height
                float newWidth = renderTargetHeight * previewAspect;
                float offsetX = (renderTargetWidth - newWidth) / (2 * renderTargetWidth);
                uv0 = new Vector2(offsetX, 0.0f);
                uv1 = new Vector2(offsetX + newWidth / renderTargetWidth, 1.0f);
            }
            else
            {
                // Taller image; fit width
                float newHeight = renderTargetWidth / previewAspect;
                float offsetY = (renderTargetHeight - newHeight) / (2 * renderTargetHeight);
                uv0 = new Vector2(0.0f, offsetY);
                uv1 = new Vector2(1.0f, offsetY + newHeight / renderTargetHeight);
            }

            Texture2D tex = Globals.ENGINE_Main.RenderTarget;
            nint texId = Globals.DEBUG_UI.Renderer.BindTexture(tex);

            ImGui.Image(texId, new Vector2(previewSize, previewSize), uv0, uv1);

            return texId;
        }
    }
}
