using BraketsEngine;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;

namespace BraketsEditor;

public class ContentPickerButton
{
    public string Name;
    public string FilePath;

    public int SizeX;

    public Action Clicked;

    public ContentPickerButton(string name, string path, Action clicked)
    {
        Name = name;
        FilePath = path;
        Clicked = clicked;

        SizeX = 84;
    }

    public void DrawGrid(DebugWindow parent, bool hasScrollbar)
    {
        if (hasScrollbar) SizeX = 82;
        else SizeX = 84;

        string extension = Path.GetExtension(this.FilePath);

        if (ImGui.ImageButton("", ResourceManager.GetImGuiTexture(GetThumbnailPath(extension)), new Vector2(SizeX)))
        {
            Clicked.Invoke();
        }

        string displayText = Name + extension;
        int maxLength = 12;

        if (displayText.Length > maxLength)
        {
            displayText = displayText.Substring(0, maxLength) + "...";
        }

        var textSize = ImGui.CalcTextSize(displayText);
        float xPos = ((SizeX + 12) - textSize.X) / 2;

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + xPos);
        ImGui.Text(displayText);
    }

    string GetThumbnailPath(string extension)
    {
        string imagePath = "ui/ui_default";
        switch (extension)
        {
            case "":
                imagePath = "ui/contentExplorer/folder";
                break;
            case ".png":
                ResourceManager.LoadTextureFromFullPath(Path.GetFullPath(this.FilePath));
                imagePath = this.Name;
                break;
            case ".ttf":
                imagePath = "ui/contentExplorer/font-file";
                break;
            case ".ogg":
            case ".wav":
                imagePath = "ui/contentExplorer/audio-file";
                break;
            case ".level":
                imagePath = "ui/contentExplorer/level-file";
                break;
            case ".particles":
                imagePath = "ui/contentExplorer/particles-file";
                break;
            default:
                imagePath = "ui/contentExplorer/unknown";
                break;
        }

        return imagePath;
    }
}
