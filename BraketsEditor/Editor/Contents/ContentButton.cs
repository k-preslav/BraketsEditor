﻿using System.IO;
using System.Numerics;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class ContentButton
{
    public string Name;
    public string FilePath;

    public ContentButton(string name, string path)
    {
        Name = name;
        FilePath = path;
    }

    public void DrawList(DebugWindow parent)
    {
        string extension = Path.GetExtension(this.FilePath);        

        ImGui.Image(ResourceManager.GetImGuiTexture(GetThumbnailPath(extension)), new Vector2(20));
        ImGui.SameLine();

        if (ImGui.Selectable(Name + extension))
        {
            if (extension == string.Empty)
            {
                string path = $"/{this.FilePath.Replace(Path.GetPathRoot(this.FilePath), "")}";
                ContentPanel.lastPath = path;

                ContentPanel.Refresh(path);
            }
        }
    }

    public void DrawGrid(DebugWindow parent)
    {
        string extension = Path.GetExtension(this.FilePath);

        if (ImGui.ImageButton("", ResourceManager.GetImGuiTexture(GetThumbnailPath(extension)), new Vector2(64)))
        {
            {
                if (extension == string.Empty)
                {
                    string path = $"/{this.FilePath.Replace(Path.GetPathRoot(this.FilePath), "")}";
                    ContentPanel.lastPath = path;

                    ContentPanel.Refresh(path);
                }
            }
        }


        string displayText = Name + extension;
        int maxLength = 8;

        if (displayText.Length > maxLength)
        {
            displayText = displayText.Substring(0, maxLength) + "...";
        }

        var textSize = ImGui.CalcTextSize(displayText);
        float xPos = (86 - textSize.X) / 2;

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