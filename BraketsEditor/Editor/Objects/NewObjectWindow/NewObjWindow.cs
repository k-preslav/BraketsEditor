using System;
using System.IO;
using System.Numerics;
using BraketsPluginIntegration;
using BraketsEngine;
using ImageMagick;
using ImGuiNET;
using NAudio.CoreAudioApi;

namespace BraketsEditor;

public class NewObjectWindow
{
    private static int _selectedItem = 0;
    private static string[] _items = {
        "Sprite",
        "Particle Emitter",
        "Sound / Song",
        "User Interface View"
    };
    private static string[] _itemsThumbnails = {
        "ui/addObject/sprite-object",
        "builtin/default_texture",
        "ui/addObject/audio-object",
        "builtin/default_texture"
    };

    private static string objName = "New Object";
    private static string objTag = "new_obj";
    private static int objLayer = 0;
    private static string objTexture = "builtin/default_texture";
    private static float objScale = 1;

    private static bool objSMU = true;
    private static bool objDOL = false;

    private static string objParticleData = "";

    static DebugWindow parent;

    public static void Create()
    {
        parent = PluginAbstraction.MakeWindow("Add new Object", () =>
        {
            NewObjectWindow.Draw();
        }, () => { }, _flags: ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize, 
        _closable: false, _visible: false, _widht: 465, _height: 675, _overrideSize:true);
    }

    public static void Draw()
    {
        if (ImGui.ListBox("", ref _selectedItem, _items, _items.Length))
        {
            if (objParticleData != string.Empty)
            {
                ParticleEditor.Init(objParticleData, refreshRT:true);
            }
        }

        ImGui.SameLine(); ImGui.Spacing(); ImGui.SameLine();
        ImGui.Image(ResourceManager.GetImGuiTexture(_itemsThumbnails[_selectedItem]), new Vector2(128));

        // Properties
        ImGui.Dummy(new Vector2(10));
        ImGui.SeparatorText("Properties");

        if (_items[_selectedItem] == "Sprite") DrawSrpiteOptions(parent);
        else if (_items[_selectedItem] == "Particle Emitter") DrawParticleOptions(parent);

        // Create button
        ImGui.SetCursorPos(new Vector2(
            ImGui.GetWindowSize().X - 125,
            ImGui.GetWindowSize().Y - 45
        ));

        WindowTheme.PushAccent();
        if (ImGui.Button("Create New", new Vector2(115, 35)))
        {
            if (objName == "")
                return;

            objName = objName.Replace(" ", "");
            if (_selectedItem == 0) ObjectCreator.CreateSprite(objName, objTag, objTexture, objScale, objLayer, objSMU, objDOL);
            else if (_selectedItem == 1) ObjectCreator.CreateParticleEmitter(objName, objLayer, objParticleData, objSMU, objDOL);

            Close();
        }
        WindowTheme.PopAccent();

        // Cancel button
        ImGui.SetCursorPos(new Vector2(
            ImGui.GetWindowSize().X - 210,
            ImGui.GetWindowSize().Y - 45
        ));

        if (ImGui.Button("Cancel", new Vector2(75, 35)))
        {
            Close();
        }
    }

    static void DrawSrpiteOptions(DebugWindow parent)
    {
        ImGui.InputText("Name", ref objName, 32);
        ImGui.InputText("Tag", ref objTag, 32);
        ImGui.InputInt("Layer", ref objLayer);
        ImGui.InputFloat("Scale", ref objScale);

        ImGui.Spacing();
        ImGui.SeparatorText("Texture");

        ImGui.BeginChild("texture_box", new Vector2(79), ImGuiChildFlags.Border, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        ImGui.Image(ResourceManager.GetImGuiTexture(objTexture), new Vector2(64));
        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginGroup();
        ImGui.Text(objTexture != string.Empty ? objTexture : "none");
        if (ImGui.Button("Pick ...", new Vector2(75, 35)))
        {
            ContentPicker.Show(ContentType.Image, (string textureName) =>
            {
                objTexture = textureName;
            });
        }
        ImGui.EndGroup();

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        bool isAdvancedOpen = ImGui.TreeNodeEx("Advanced", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.Framed);
        if (isAdvancedOpen)
        {
            ImGui.Checkbox("Smart Update", ref objSMU);
            ImGui.Checkbox("Draw On Loading", ref objDOL);

            ImGui.TreePop();
        }
    }
    static void DrawParticleOptions(DebugWindow parent)
    {
        ImGui.InputText("Name", ref objName, 32);
        ImGui.InputInt("Layer", ref objLayer);

        ImGui.Spacing();
        ImGui.SeparatorText("Particle Data");

        ImGui.BeginChild("particle_preview_box", new Vector2(79), ImGuiChildFlags.Border, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        if (objParticleData == string.Empty) ImGui.Image(ResourceManager.GetImGuiTexture($"ui/contentExplorer/particles-file"), new Vector2(64));
        else RenderTargetToImg.DrawImage(64);
       ImGui.EndChild();

        ImGui.SameLine();
        
        ImGui.BeginGroup();
        ImGui.Text(objParticleData != string.Empty ? objParticleData : "none");

        if (ImGui.Button("Pick ...", new Vector2(75, 35)))
        {
            ContentPicker.Show(ContentType.Particles, (string particlesName) =>
            {
                objParticleData = particlesName;

                ParticleEditor.Init(name:particlesName, refreshRT:true);
            });
        }
        ImGui.SameLine();
        if (ImGui.Button("New Particles", new Vector2(125, 35)))
        {
            ImGui.SetWindowCollapsed(true);

            MainToolsWindow.AddTab(new ToolTab
            {
                name = "Particle Editor",
                view = ParticleEditor.Draw,
                update = ParticleEditor.Update
            });

            ParticleEditor.Init(type: "new");
        }
        ImGui.EndGroup();

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        bool isAdvancedOpen = ImGui.TreeNodeEx("Advanced", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.Framed);
        if (isAdvancedOpen)
        {
            ImGui.Checkbox("Smart Update", ref objSMU);
            ImGui.Checkbox("Draw On Loading", ref objDOL);

            ImGui.TreePop();
        }
    }

    private static void Close()
    {
        objName = "New Object";
        objTag = "new_obj";
        objLayer = 0;
        objTexture = "builtin/default_texture";
        objScale = 1;

        objSMU = true;
        objDOL = false;

        objParticleData = "";

        _selectedItem = 0;

        Globals.DEBUG_UI.GetWindow("Add new Object").Visible = false;
    }
}