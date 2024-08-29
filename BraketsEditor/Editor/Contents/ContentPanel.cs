using BraketsEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor.Editor;

public class ContentPanel
{
    static List<ContentButton> content = new List<ContentButton>();
    static string currentPathFull;
    static string currentPath;
    public static string lastPath = Globals.projectContentFolderPath;

    static bool listLayout = true;
    static int maxGridFolderCount;

    static int parentWidth = 300;

    public static void Draw(DebugWindow parent)
    {
        parent.Pos = new Vector2(0, Globals.APP_Height / 2);
        parent.Size = new Vector2(parentWidth, Globals.APP_Height / 2);

        ImGui.SeparatorText("Content");

        if (ImGui.SmallButton("<"))
        {
            string parentPath = Path.GetFullPath(Path.GetDirectoryName(currentPathFull)).TrimEnd(Path.DirectorySeparatorChar);
            string projectPath = Path.GetFullPath(Globals.projectPath).TrimEnd(Path.DirectorySeparatorChar);
            
            if (parentPath.Equals(projectPath))
                return;
            
            Refresh(parentPath);
            lastPath = parentPath;
        }


        ImGui.SameLine(); ImGui.Spacing(); ImGui.SameLine();
        ImGui.Text($"{currentPath}");

        ImGui.SameLine();
        ImGui.SetCursorPosX(parent.Size.X - 32);
        if (ImGui.SmallButton("L"))
        {
            listLayout = !listLayout;
        }

        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();

        for (int i = 0; i < content.Count; i++)
        {
            if (listLayout)
                content[i].DrawList(parent);
            else
            {
                ImGui.PushID($"content{i}");
                ImGui.BeginGroup();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
                content[i].DrawGrid(parent);
                ImGui.EndGroup();
                ImGui.PopID();

                if ((i + 1) % maxGridFolderCount != 0)
                {
                    ImGui.SameLine();
                    ImGui.Spacing();
                    ImGui.SameLine();
                }
                else ImGui.NewLine();
            }
        }
    }

    public static void Refresh(string path = "/")
    {
        maxGridFolderCount = parentWidth / 96;

        if (path == "/")
            path = Globals.projectContentFolderPath;
        else if (path == "last")
            path = lastPath;

        currentPathFull = Path.GetFullPath(path);
        
        int cntPIndex = path.IndexOf("content");
        currentPath = Path.Combine("\\", path.Substring(cntPIndex).Replace("content", "").TrimStart('\\'));

        content.Clear();

        string[] folders = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var folder in folders)
        {
            string name = Path.GetFileNameWithoutExtension(folder);
            string fullPath = Path.GetFullPath(folder);

            if (Path.GetExtension(fullPath).Contains("~"))
                continue; // skip as it is a temp folder

            content.Add(
                new ContentButton(name, fullPath)
            );
        }

        string[] files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string fullPath = Path.GetFullPath(file);

            if (Path.GetExtension(fullPath).Contains("~"))
                continue; // skip as it is a temp file

            content.Add(
                new ContentButton(name, fullPath)
            );
        }
    }
}
