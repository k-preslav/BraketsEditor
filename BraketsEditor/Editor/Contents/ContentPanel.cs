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

    public static void Create()
    {
        PluginAbstraction.MakeWindow("Content", (window) =>
        {
            ContentPanel.Draw(window);
        }, () => { }, _flags: ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus,
                      _overrideSize:true, _overridePos:true);
        ContentPanel.Refresh("/");
    }

    public static void Draw(DebugWindow parent)
    {
        parent.Pos = new Vector2(0, Globals.APP_Height / 2);
        parent.Size = new Vector2(parentWidth, Globals.APP_Height / 2);

        bool hasScrollbar = content.Count * (listLayout ? 36 : 70) > parent.Size.Y;

        ImGui.SeparatorText("Content");

        string backTexture = WindowTheme.currentTheme == "dark" ? "ui/contentExplorer/back-white" : "ui/contentExplorer/back-black";
        if (ImGui.ImageButton("###back-content_button", ResourceManager.GetImGuiTexture(backTexture), new Vector2(16)))
        {
            string parentPath = Path.GetFullPath(Path.GetDirectoryName(currentPathFull)).TrimEnd(Path.DirectorySeparatorChar);
            string projectPath = Path.GetFullPath(Globals.projectPath).TrimEnd(Path.DirectorySeparatorChar);
            
            if (parentPath.Equals(projectPath))
                return;
            
            Refresh(parentPath);
            lastPath = parentPath;
        }


        ImGui.SameLine(); ImGui.Spacing(); ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);
        ImGui.Text($"{currentPath}");

        ImGui.SameLine();
        ImGui.SetCursorPosX(parentWidth - (hasScrollbar ? 50 : 42));
        string refreshTexture = WindowTheme.currentTheme == "dark" ? "ui/refresh/refresh-white" : "ui/refresh/refresh-black";
        if (ImGui.ImageButton("###refresh-obj_button", ResourceManager.GetImGuiTexture(refreshTexture), new Vector2(16))) Refresh("last");

        ImGui.SameLine();
        ImGui.SetCursorPosX(parentWidth - (hasScrollbar ? 86 : 78));        
        string listGridTexture = listLayout ?
            (WindowTheme.currentTheme == "dark" ? "ui/contentExplorer/list-white" : "ui/contentExplorer/list-black") :
            (WindowTheme.currentTheme == "dark" ? "ui/contentExplorer/grid-white" : "ui/contentExplorer/grid-black");
        if (ImGui.ImageButton("###layout-content_button", ResourceManager.GetImGuiTexture(listGridTexture), new Vector2(16)))
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
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (hasScrollbar ? 0 : 5));
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
        ResourceManager.Refresh();

        maxGridFolderCount = parentWidth / 96;

        if (path == "/")
            path = Globals.projectContentFolderPath;
        else if (path == "last")
            path = lastPath;

        currentPathFull = Path.GetFullPath(path);
        
        int cntPIndex = path.IndexOf("content");
        currentPath = Path.Combine("\\", path.Substring(cntPIndex).Replace("content", "").TrimStart('\\')).Replace(@"/", @"\");

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
