using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Transactions;
using BraketsEditor.Editor;
using BraketsEngine;
using FontStashSharp.Rasterizers.StbTrueTypeSharp;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace BraketsEditor;

public class ObjectsPanel
{
    static List<ObjectButton> objects = new List<ObjectButton>();

    public static void Create()
    {
        PluginAbstraction.MakeWindow("Objects", (window) =>
        {
            ObjectsPanel.Draw(window);
        }, () => { }, _flags: ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus,
                    _overridePos:true, _overrideSize:true);
        ObjectsPanel.Refresh();
    }

    public static void Draw(DebugWindow parent)
    {
        parent.Pos = new Vector2(0, Globals.DEBUG_UI_MENUBAR_SIZE_Y);
        parent.Size = new Vector2(300, Globals.APP_Height / 2 - Globals.DEBUG_UI_MENUBAR_SIZE_Y);

        bool hasScrollbar = objects.Count * 64 > parent.Size.Y;

        ImGui.SeparatorText("Objects");
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 10);

        string textureName = WindowTheme.currentTheme == "dark" ? "ui/refresh/refresh-white" : "ui/refresh/refresh-black";
        if (ImGui.ImageButton("###refresh-obj_button", ResourceManager.GetImGuiTexture(textureName), new Vector2(14).ToNumerics())) Refresh();

        if (Input.IsMouseClicked(1) && Input.GetMousePositionScreen().X < parent.Size.X && Input.GetMousePositionScreen().Y < parent.Size.Y)
        {
            ImGui.OpenPopup("Create Class Popup");
        }

        if (ImGui.BeginPopup("Create Class Popup"))
        {
            if (ImGui.Button("Create New...", new Vector2(150, 30).ToNumerics()))
            {
                Globals.DEBUG_UI.GetWindow("Add new Object").Visible = true;
            }
            ImGui.EndPopup();
        }

        for (int i = 0; i < objects.Count; i++)
        {
            ImGui.PushID($"BUTTON {i}");
            objects[i].Draw(parent, hasScrollbar);
            ImGui.PopID();
        }
    }

    public static void Refresh()
    {
        objects.Clear();

        string[] files = Directory.GetFiles(Globals.projectGameFolderPath, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string fullPath = Path.GetFullPath(file);

            string type = "", specialArgs = "";
            if (fullPath.Contains("sprites") || fullPath.Contains("GameManager"))
            {
                type = "sprite";
            }
            else if (fullPath.Contains("particles"))
            {
                type = "particles";
                
                // Find the name of the particle data, passed in to ParticleEmitterData().FromFile('xxx')
                string contents = File.ReadAllText(fullPath);
                
                string pattern = @"FromFile\(@""([^""]+)""\)";
                Match match = Regex.Match(contents, pattern);

                if (match.Success)
                {
                    specialArgs = match.Groups[1].Value;
                }
                else
                {
                    Debug.Error($"Failed to parse object of type {type}");
                    return;
                }
            }

            objects.Add(
                new ObjectButton(name, fullPath, type, specialArgs)
            );
        }
    }
}