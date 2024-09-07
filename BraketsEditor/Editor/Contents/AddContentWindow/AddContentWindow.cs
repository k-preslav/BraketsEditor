using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;
using ImageMagick;
using BraketsEngine;
using BraketsEditor;
using BraketsPluginIntegration;

namespace BraketsEditor;

public class AddContentWindow
{
    public static List<string> files = new List<string>();
    public static List<string> filesFull = new List<string>();

    public static OptionsType optionType = OptionsType.None;
    public static SelectedOption selectedOption = SelectedOption.Image;

    private static int selectedIndex = 0;

    static string spAnimFile = "image-file";
    static float frameTimer = 0.5f;

    static string addButtonText = "";

    static bool saveInUI = false;
    static bool none = false;

    static DebugWindow win;

    public static void Create()
    {
        win = PluginAbstraction.MakeWindow("Add new Content", () =>
        {
            AddContentWindow.Draw();
        }, () => { }, _flags: ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse, 
                      _closable: true, _visible: false, _widht: 575, _height: 250, _overrideSize:true);
    }

    public static void Draw()
    {
        ImGui.SeparatorText("Files");

        float listBoxWidth = win.Size.X / 2.35f;
        float listBoxHeight = win.Size.Y - 10;

        ImGui.BeginChild("FilesListBox", new Vector2(listBoxWidth, listBoxHeight - 75), ImGuiChildFlags.Border);
        ImGui.SetNextItemWidth(listBoxWidth - 15);
        for (int i = 0; i < files.Count; i++)
        {
            if (ImGui.Selectable(files[i])) selectedIndex = i;
        }
        ImGui.EndChild();

        ImGui.SameLine(); ImGui.Spacing(); ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 25);

        string arrowTexture = WindowTheme.currentTheme == "dark" ? "ui/addContent/arrows-right-white" : "ui/addContent/arrows-right-black";
        ImGui.Image(ResourceManager.GetImGuiTexture(arrowTexture), new Vector2(32));

        ImGui.SameLine(); ImGui.Spacing(); ImGui.SameLine();
        DrawOptions();

        if (selectedOption != SelectedOption.ImageSqnc &&
            selectedOption != SelectedOption.Icon &&
            selectedOption != SelectedOption.Audio &&
            selectedOption != SelectedOption.Font)
        {
            if (files[selectedIndex].Length > 15)
            {
                addButtonText = $"Add {files[selectedIndex].Substring(0, 14)}...";
            }
            else addButtonText = $"Add {files[selectedIndex]}";
        }

        Vector2 textSize = ImGui.CalcTextSize(addButtonText);
        float buttonWidth = textSize.X + 15;
        float buttonHeight = 35;

        Vector2 buttonPosition = new Vector2(
            win.Size.X - buttonWidth - 15,
            win.Size.Y - buttonHeight - 15
        );

        ImGui.SetCursorPos(buttonPosition);
        WindowTheme.PushAccent();
        if (ImGui.Button(addButtonText, new Vector2(buttonWidth, buttonHeight)))
        {
            ProcessImageAdd();
            ProcessSongSoundAdd();
            ProcessFontAdd();
            ProcessLevelAdd();

            ContentPanel.Refresh("last");

            ContentPicker.Refresh();

            files.RemoveAt(selectedIndex);
            filesFull.RemoveAt(selectedIndex);

            if (files.Count <= 0)
                win.Visible = false;
                
            saveInUI = false;
        }
        WindowTheme.PopAccent();

        if (selectedOption == SelectedOption.Image)
        {
            Vector2 checkboxPosition = new Vector2(
                buttonPosition.X - 120,
                buttonPosition.Y
            );
            ImGui.SetCursorPos(checkboxPosition);
            ImGui.Checkbox("Use as UI", ref saveInUI);
            ImGui.NewLine();
        }

        frameTimer -= Globals.DEBUG_DT;
        if (frameTimer <= 0)
        {
            frameTimer = 0.25f;
            if (spAnimFile == "image-file") spAnimFile = "image-file-alt1";
            else if (spAnimFile == "image-file-alt1") spAnimFile = "image-file-alt2";
            else spAnimFile = "image-file";
        }
    }

    public static void UpdateOptionType()
    {
        string fileExtension = Path.GetExtension(filesFull[selectedIndex]).ToLower();

        switch (fileExtension)
        {
            case ".png":
                selectedOption = SelectedOption.Image;
                optionType = OptionsType.Image;
                break;

            case ".wav":
                selectedOption = SelectedOption.Font;
                optionType = OptionsType.Audio;
                addButtonText = $"Add {files[selectedIndex]} as Sound";
                break;

            case ".ogg":
                selectedOption = SelectedOption.Audio;
                optionType = OptionsType.Audio;
                addButtonText = $"Add {files[selectedIndex]} as Song";
                break;

            case ".ttf":
                selectedOption = SelectedOption.Font;
                optionType = OptionsType.Font;
                addButtonText = $"Add {files[selectedIndex]} as Font";
                break;

            case ".level":
                selectedOption = SelectedOption.Level;
                optionType = OptionsType.Level;
                addButtonText = $"Add {files[selectedIndex]} as Level";
                break;

            default:
                optionType = OptionsType.None;
                break;
        }
    }

    static void DrawOptions()
    {
        if (optionType == OptionsType.Image) DrawImageOptions();
        else if (optionType == OptionsType.Audio) DrawSongSoundOptions();
        else if (optionType == OptionsType.Font) DrawFontOptions();
        else if (optionType == OptionsType.Level) DrawLevelOptions();
        else if (optionType == OptionsType.None)
        {
            if (!none)
            {
                win.Visible = false;
                new MessageBox($"Unsupported file format ({Path.GetExtension(files[selectedIndex])})!", callback: (result) =>
                {
                    none = false;
                }).Show();
            }
            none = true;
        }
    }

    static void DrawImageOptions()
    {
        ImGui.BeginGroup();

        if (selectedOption == SelectedOption.Image) WindowTheme.PushAccent();
        if (ImGui.ImageButton("##sprite-button", ResourceManager.GetImGuiTexture("ui/addContent/image-file"), new Vector2(64)))
        {
            selectedOption = SelectedOption.Image;
        }
        WindowTheme.PopAccent();

        ImGui.SameLine();

        if (selectedOption == SelectedOption.ImageSqnc) WindowTheme.PushAccent();
        if (ImGui.ImageButton("##animation-sprite-button", ResourceManager.GetImGuiTexture($"ui/addContent/{spAnimFile}"), new Vector2(64)))
        {
            selectedOption = SelectedOption.ImageSqnc;
            addButtonText = "Add *.png as Sequence";
        }
        WindowTheme.PopAccent();

        ImGui.SameLine();

        if (selectedOption == SelectedOption.Icon) WindowTheme.PushAccent();
        if (ImGui.ImageButton("##icon-button", ResourceManager.GetImGuiTexture("ui/addContent/icon-file"), new Vector2(64)))
        {
            selectedOption = SelectedOption.Icon;
            addButtonText = $"Add {files[selectedIndex]} as Icon";
        }
        WindowTheme.PopAccent();

        ImGui.EndGroup();
    }

    static void ProcessImageAdd()
    {
        if (optionType == OptionsType.Image)
        {
            if (selectedOption == SelectedOption.Image)
            {
                try
                {
                    string imagePath = AddContentWindow.filesFull[selectedIndex];
                    string destFolder = saveInUI
                        ? Path.Combine(Globals.projectContentFolderPath, "images/ui")
                        : Path.Combine(Globals.projectContentFolderPath, "images");

                    string destPath = Path.Combine(destFolder, Path.GetFileName(imagePath));

                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);
                    }
                    File.Copy(imagePath, destPath, overwrite: true);
                }
                catch (Exception ex)
                {
                    BraketsEngine.Debug.Error("Failed to add image!\n Ex: " + ex.Message);
                    new MessageBox("Failed to add image!\n Ex: " + ex.Message).Show();
                }
            }
            else if (selectedOption == SelectedOption.ImageSqnc)
            {
                new MessageBox("Not implemented :]").Show();
            }
            else if (selectedOption == SelectedOption.Icon)
            {
                try
                {
                    string originalPath = AddContentWindow.filesFull[selectedIndex];
                    string destFolder = Path.Combine(Globals.projectContentFolderPath, "images/icon");

                    using (var icoImg = new MagickImage(originalPath))
                    {
                        icoImg.Resize(64, 64);
                        icoImg.Format = MagickFormat.Ico;
                        icoImg.Write(Path.Combine(destFolder, "Icon.ico"));
                    }
                    using (var bmpImg = new MagickImage(originalPath))
                    {
                        bmpImg.Resize(64, 64);
                        bmpImg.Format = MagickFormat.Bmp;
                        bmpImg.Write(Path.Combine(destFolder, "Icon.bmp"));
                    }
                }
                catch (Exception ex)
                {
                    BraketsEngine.Debug.Error("Failed to add image as icon!\n Ex: " + ex.Message);
                    new MessageBox("Failed to add image as icon!\n Ex: " + ex.Message).Show();
                }
            }
        }
    }

    static void DrawSongSoundOptions()
    {
        ImGui.BeginGroup();

        if (selectedOption == SelectedOption.Font) WindowTheme.PushAccent();
        if (ImGui.ImageButton("##sound-button", ResourceManager.GetImGuiTexture("ui/contentExplorer/audio-file"), new Vector2(64)))
        {
            addButtonText = $"Add {files[selectedIndex]} as Sound";
            selectedOption = SelectedOption.Font;
        }
        WindowTheme.PopAccent();

        ImGui.SameLine();

        if (selectedOption == SelectedOption.Audio) WindowTheme.PushAccent();
        if (ImGui.ImageButton("##song-button", ResourceManager.GetImGuiTexture($"ui/contentExplorer/audio-file"), new Vector2(64)))
        {
            addButtonText = $"Add {files[selectedIndex]} as Song";
            selectedOption = SelectedOption.Audio;
        }
        WindowTheme.PopAccent();

        ImGui.EndGroup();
    }

    static void ProcessSongSoundAdd()
    {
        if (optionType == OptionsType.Audio)
        {
            if (selectedOption == SelectedOption.Audio)
            {
                try
                {
                    string songPath = AddContentWindow.filesFull[selectedIndex];
                    string ext = Path.GetExtension(songPath);

                    string destFolder = Path.Combine(Globals.projectContentFolderPath, "songs");

                    if (ext == ".ogg")
                    {
                        string destPath = Path.Combine(destFolder, Path.GetFileName(songPath));

                        if (File.Exists(destPath))
                        {
                            File.Delete(destPath);
                        }
                        File.Copy(songPath, destPath, overwrite: true);
                    }
                }
                catch (Exception ex)
                {
                    BraketsEngine.Debug.Error("Failed to add song!\n Ex: " + ex.Message);
                    new MessageBox("Failed to add song!\n Ex: " + ex.Message).Show();
                }
            }
            else if (selectedOption == SelectedOption.Font)
            {
                try
                {
                    string soundPath = AddContentWindow.filesFull[selectedIndex];
                    string ext = Path.GetExtension(soundPath);

                    string destFolder = Path.Combine(Globals.projectContentFolderPath, "sounds");

                    if (ext == ".wav")
                    {
                        string destPath = Path.Combine(destFolder, Path.GetFileName(soundPath));

                        if (File.Exists(destPath))
                        {
                            File.Delete(destPath);
                        }
                        File.Copy(soundPath, destPath, overwrite: true);
                    }
                }
                catch (Exception ex)
                {
                    BraketsEngine.Debug.Error("Failed to add sound!\n Ex: " + ex.Message);
                    new MessageBox("Failed to add sound!\n Ex: " + ex.Message).Show();
                }
            }
        }
    }

    static void DrawFontOptions()
    {
        ImGui.BeginGroup();

        if (selectedOption == SelectedOption.Font) WindowTheme.PushAccent();
        if (ImGui.ImageButton("##font-button", ResourceManager.GetImGuiTexture("ui/contentExplorer/font-file"), new Vector2(64)))
        {
            addButtonText = $"Add {files[selectedIndex]} as Font";
            selectedOption = SelectedOption.Font;
        }
        WindowTheme.PopAccent();

        ImGui.EndGroup();
    }
    static void ProcessFontAdd()
    {
        if (optionType == OptionsType.Font)
        {
            if (selectedOption == SelectedOption.Font)
            {
                try
                {
                    string fontPath = AddContentWindow.filesFull[selectedIndex];
                    string destFolder = Path.Combine(Globals.projectContentFolderPath, "fonts");
                    string destPath = Path.Combine(destFolder, Path.GetFileName(fontPath));

                    File.Copy(fontPath, destPath, overwrite: true);
                }
                catch (Exception ex)
                {
                    BraketsEngine.Debug.Error("Failed to add font!\n Ex: " + ex.Message);
                    new MessageBox("Failed to add font!\n Ex: " + ex.Message).Show();
                }
            }
        }
    }

    static void DrawLevelOptions()
    {
        ImGui.BeginGroup();

        if (selectedOption == SelectedOption.Level) WindowTheme.PushAccent();
        if (ImGui.ImageButton("##level-button", ResourceManager.GetImGuiTexture("ui/contentExplorer/level-file"), new Vector2(64)))
        {
            addButtonText = $"Add {files[selectedIndex]} as Level";
            selectedOption = SelectedOption.Level;
        }
        WindowTheme.PopAccent();

        ImGui.EndGroup();
    }
    static void ProcessLevelAdd()
    {
        if (optionType == OptionsType.Level)
        {
            if (selectedOption == SelectedOption.Level)
            {
                try
                {
                    string levelPath = AddContentWindow.filesFull[selectedIndex];
                    string destFolder = Path.Combine(Globals.projectContentFolderPath, "levels");
                    string destPath = Path.Combine(destFolder, Path.GetFileName(levelPath));

                    File.Copy(levelPath, destPath, overwrite: true);
                }
                catch (Exception ex)
                {
                    BraketsEngine.Debug.Error("Failed to add level!\n Ex: " + ex.Message);
                    new MessageBox("Failed to add level!\n Ex: " + ex.Message).Show();
                }
            }
        }
    }
}

public enum SelectedOption
{
    Image,
    ImageSqnc,
    Icon,

    Audio,

    Font,

    Level
}

public enum OptionsType
{
    Image,
    Audio,
    Font,
    Level,
    None
}
