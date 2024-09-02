using BraketsEditor.Editor;
using BraketsEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor.Editor.Contents.ContentPicker
{
    internal class ContentPicker
    {
        public static Action<string, string> OnSelected;
        public static Action<string> OnPicked;

        static ContentType cType;
        static List<ContentPickerButton> pickerButtons = new List<ContentPickerButton>();

        static string selectedFileName = "";
        static string selectedFilePath = "";

        public static void Create()
        {
            PluginAbstraction.MakeWindow("Content Picker", (window) =>
            {
                ContentPicker.Draw(window);
            }, () => { }, _flags: ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize, _closable: true, 
            _visible: false, _widht: 580, _height: 400, _overrideSize: true);
        }

        public static void Show(ContentType type, Action<string> onPicked)
        {
            cType = type;
            Refresh();

            OnPicked = onPicked;

             Globals.DEBUG_UI.GetWindow("Content Picker").Visible = true;

            OnSelected = (filePath, name) =>
            {
                selectedFileName = name;
                selectedFilePath = filePath;
            };
        }

        public static void Draw(DebugWindow parent)
        {
            bool hasScrollbar = pickerButtons.Count * 72 > parent.Size.Y;

            ImGui.BeginChild("content", new Vector2(ImGui.GetWindowSize().X - 15, ImGui.GetWindowSize().Y - 100), ImGuiChildFlags.Border);
            for (int i = 0; i < pickerButtons.Count; i++)
            {
                int maxGridFolderCount = (int)parent.Size.X / (pickerButtons[i].SizeX + 25);
                
                ImGui.PushID($"content{i}");
                ImGui.BeginGroup();
                if (pickerButtons[i].Name == selectedFileName) WindowTheme.PushAccent();
                pickerButtons[i].DrawGrid(parent, hasScrollbar);
                WindowTheme.PopAccent();
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
            ImGui.EndChild();

            ImGui.SetCursorPos(new Vector2(
                ImGui.GetWindowSize().X - 110,
                ImGui.GetWindowSize().Y - 45
            ));
            WindowTheme.PushAccent();
            if (ImGui.Button("Pick", new Vector2(100, 35)))
            {
                if (selectedFilePath != string.Empty)
                {
                    OnPicked?.Invoke(selectedFilePath);
                    parent.Visible = false;
                }
            }
            WindowTheme.PopAccent();
        }

        internal static void Refresh()
        {
            pickerButtons.Clear();

            string[] _files = Directory.GetFiles(Globals.projectContentFolderPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in _files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string fullPath = Path.GetFullPath(file);
                string ext = Path.GetExtension(file);

                string filePath = "";
                string relativeTo = "images/";

                if (ext == ".png")
                {
                    bool isInUiDirectory = fullPath.Contains($"{Path.DirectorySeparatorChar}ui{Path.DirectorySeparatorChar}");
                    filePath = Path.GetRelativePath(Path.Combine(Globals.projectContentFolderPath, relativeTo), fullPath).Replace(".png", "");

                    if (cType == ContentType.UI_Image && isInUiDirectory)
                    {
                        relativeTo = "images/";
                        filePath = Path.GetRelativePath(Path.Combine(Globals.projectContentFolderPath, relativeTo), fullPath).Replace(".png", "");
                        pickerButtons.Add(new ContentPickerButton(name, fullPath, () => { OnSelected?.Invoke(filePath, name); }));
                    }
                    else if (cType == ContentType.Image && !isInUiDirectory)
                        pickerButtons.Add(new ContentPickerButton(name, fullPath, () => { OnSelected?.Invoke(filePath, name); }));
                }
                else if (cType == ContentType.Sound && ext == ".wav")
                {
                    relativeTo = "sounds/";
                    filePath = Path.GetRelativePath(Path.Combine(Globals.projectContentFolderPath, relativeTo), fullPath).Replace(".wav", "");
                    pickerButtons.Add(new ContentPickerButton(name, fullPath, () => { OnSelected?.Invoke(filePath, name); }));
                }
                else if (cType == ContentType.Song && ext == ".ogg")
                {
                    relativeTo = "songs/";
                    filePath = Path.GetRelativePath(Path.Combine(Globals.projectContentFolderPath, relativeTo), fullPath).Replace(".ogg", "");
                    pickerButtons.Add(new ContentPickerButton(name, fullPath, () => { OnSelected?.Invoke(filePath, name); }));
                }
                else if (cType == ContentType.Font && ext == ".ttf")
                {
                    relativeTo = "fonts/";
                    filePath = Path.GetRelativePath(Path.Combine(Globals.projectContentFolderPath, relativeTo), fullPath).Replace(".ttf", "");
                    pickerButtons.Add(new ContentPickerButton(name, fullPath, () => { OnSelected?.Invoke(filePath, name); }));
                }
                else if (cType == ContentType.Level && ext == ".level")
                {
                    relativeTo = "levels/";
                    filePath = Path.GetRelativePath(Path.Combine(Globals.projectContentFolderPath, relativeTo), fullPath).Replace(".level", "");
                    pickerButtons.Add(new ContentPickerButton(name, fullPath, () => { OnSelected?.Invoke(filePath, name); }));
                }
                else if (cType == ContentType.Particles && ext == ".particles")
                {
                    relativeTo = "particles/";
                    filePath = Path.GetRelativePath(Path.Combine(Globals.projectContentFolderPath, relativeTo), fullPath).Replace(".particles", "");
                    pickerButtons.Add(new ContentPickerButton(name, fullPath, () => { OnSelected?.Invoke(filePath, name); }));
                }
            }
        }
    }

    public enum ContentType
    {
        Image,
        UI_Image,

        Sound,
        Song,
        
        Font,
        
        Level,

        Particles
    }
}
