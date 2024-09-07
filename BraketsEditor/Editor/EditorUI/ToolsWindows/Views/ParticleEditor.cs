using BraketsEditor.Engine;
using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BraketsEditor;

public class ParticleEditor
{
    static ParticleEmitter particleEmitter;
    static float zoom = 1f;
    static float yOffset = 0f;

    static float angleVariance = 45f;
    static float lifespanMin = 0.1f;
    static float lifespanMax = 2f;
    static float speedMin = 100f;
    static float speedMax = 100f;
    static float sizeStartMin = 1;
    static float sizeStartMax = 3f;
    static float sizeEndMin = 24f;
    static float sizeEndMax = 36f;
    static System.Numerics.Vector4 colorStart = Color.Yellow.ToVector4().ToNumerics();
    static System.Numerics.Vector4 colorEnd = Color.Red.ToVector4().ToNumerics();
    static float opacityStart = 1f;
    static float opacityEnd = 0f;
    static float interval = 0.5f;
    static int emitCount = 1;
    static bool visible = true;
    static bool drawOnLoading = false;
    static string textureName = "builtin/particle_default";

    static string emitterName = "";
    static string lastEmitterName = emitterName;
    static string saveName = "new_particles";

    static float refreshTimer = 0;
 
    static bool isInit = false;
    static bool isUnloading = false;
    static bool isLoading = false;

    public static async void Init(string name = "last", string type = "existing", bool refreshRT=false)
    {
        if (refreshRT) refreshTimer = -15;

        await Task.Run(async() =>
        {
            isLoading = true;

            Globals.EditorManager.Status = "Loading particle manager...";
            Throbber.visible = true;

            await Unload();
            isUnloading = false;

            if (name == "last") emitterName = lastEmitterName;
            else emitterName = name;

            lastEmitterName = emitterName;

            await Load(type);

            OnAppResize();
            particleEmitter = new ParticleEmitter("particleEmitter", new Vector2(0).ToNumerics(), new ParticleEmitterData
            {
                angleVariance = angleVariance,
                lifeSpanMin = lifespanMin,
                lifeSpanMax = lifespanMax,
                speedMin = speedMin,
                speedMax = speedMax,
                sizeStartMin = sizeStartMin,
                sizeStartMax = sizeStartMax,
                sizeEndMin = sizeEndMin,
                sizeEndMax = sizeEndMax,
                colorStart = new Color(colorStart),
                colorEnd = new Color(colorEnd),
                opacityStart = opacityStart,
                opacityEnd = opacityEnd,
                interval = interval,
                emitCount = emitCount,
                visible = visible,
                textureName = textureName,
            }, 0);

            isInit = true;
            isLoading = false;
            Globals.EditorManager.Status = "Ready";
            Throbber.visible = false;
        });
    }
    public static async Task Unload()
    {
        try
        {
            await Task.Run(() =>
            {
                isUnloading = true;

                if (particleEmitter is not null)
                {
                    ParticleManager.UnloadAll();
                }
                isInit = false;

                angleVariance = 45f;
                lifespanMin = 0.1f;
                lifespanMax = 2f;
                speedMin = 100f;
                speedMax = 100f;
                sizeStartMin = 1;
                sizeStartMax = 3f;
                sizeEndMin = 24f;
                sizeEndMax = 36f;
                colorStart = Color.Yellow.ToVector4().ToNumerics();
                colorEnd = Color.Red.ToVector4().ToNumerics();
                opacityStart = 1f;
                opacityEnd = 0f;
                interval = 0.5f;
                emitCount = 1;
                visible = true;
                drawOnLoading = false;
                textureName = "builtin/particle_default";
            });
        }
        catch (Exception ex)
        {
            Debug.Error("Exception while unloading particle emitter: " + ex.Message);
        }
        finally
        {
            isUnloading = false;
        }
    }


    public static void Draw()
    {
        if (isUnloading)
        {
            ImGui.Dummy(new Vector2(15).ToNumerics());
            ImGui.Text("Please wait while unload operation has finished...");
            ImGui.Dummy(new Vector2(15).ToNumerics());

            if (!isLoading) ImGui.BeginDisabled();
        }
        else if (!isInit)
        {
            ImGui.Dummy(new Vector2(15).ToNumerics());
            ImGui.Text("Particle Editor service was unloaded.");
            ImGui.SameLine(); ImGui.Spacing(); ImGui.SameLine();

            WindowTheme.PushAccent();
            if (ImGui.SmallButton("Reload"))
            {
                Init();
            }
            WindowTheme.PopAccent();
            ImGui.Dummy(new Vector2(15).ToNumerics());

            if (!isLoading) ImGui.BeginDisabled();
        }

        if (isLoading) ImGui.BeginDisabled();

        refreshTimer = 0;
        int previewSize = (Globals.APP_Width > 1600 && Globals.APP_Height > 900) ? 640 : 480;

        ImGui.BeginGroup();
        RenderTargetToImg.DrawImage(previewSize);

        ImGui.SetNextItemWidth(previewSize - ImGui.CalcTextSize("View Zoom").X - 205);
        ImGui.SliderFloat("View Zoom", ref zoom, Globals.Camera.ZoomMin, Globals.Camera.ZoomMax);
        Globals.Camera.SetZoom(zoom);

        ImGui.SetNextItemWidth(previewSize - ImGui.CalcTextSize("View Offset").X - 200);
        ImGui.SliderFloat("View Offset", ref yOffset, -350, 200);
        Globals.Camera.Teleport(new Vector2(Globals.Camera.TargetPosition.X, yOffset));

        ImGui.Spacing();

        ImGui.SetNextItemWidth(200);
        ImGui.InputText("Save Name", ref saveName, 32);

        WindowTheme.PushAccent();
        if (ImGui.Button("Save", new System.Numerics.Vector2(150, 35)))
        {
            Save();
        }
        WindowTheme.PopAccent();

        ImGui.EndGroup();

        ImGui.SameLine();
        ImGui.Spacing();
        ImGui.SameLine();

        ImGui.BeginGroup();
        ImGui.BeginChild("Properties", new System.Numerics.Vector2(ImGui.GetWindowWidth() - previewSize - 30, ImGui.GetWindowHeight() - 60), ImGuiChildFlags.AlwaysUseWindowPadding | ImGuiChildFlags.Border);

        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Interval", ref interval, 0.01f, 1))
        {
            if (interval <= 0)
                interval = 1f;

            particleEmitter.ModifyParticleDataProp("interval", interval);
        }
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderInt("Emit Count", ref emitCount, 0, 128)) particleEmitter.ModifyParticleDataProp("emitCount", emitCount);
        ImGui.Spacing();
        ImGui.SeparatorText("Texture");
        ImGui.BeginChild("TextureBorder", new System.Numerics.Vector2(80), ImGuiChildFlags.Border);
        ImGui.Image(ResourceManager.GetImGuiTexture(textureName), new System.Numerics.Vector2(64));
        ImGui.EndChild();
        ImGui.SameLine();
        ImGui.BeginGroup();
        ImGui.Text(textureName);
        ImGui.Spacing();
        if (ImGui.Button("Pick ..."))
        {
            ContentPicker.Show(ContentType.Image, (name) =>
            {
                textureName = name;
                particleEmitter.ModifyParticleDataProp("textureName", textureName);
            });
        }
        ImGui.EndGroup();
        ImGui.Spacing();

        ImGui.SeparatorText("Angle");
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Variance", ref angleVariance, 0, 180)) particleEmitter.ModifyParticleDataProp("angleVariance", angleVariance);
        ImGui.Spacing();

        ImGui.SeparatorText("Lifespan");
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Lifespan Min", ref lifespanMin, 0.1f, 25f)) particleEmitter.ModifyParticleDataProp("lifeSpanMin", lifespanMin);
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Lifespan Max", ref lifespanMax, 0.1f, 25f)) particleEmitter.ModifyParticleDataProp("lifeSpanMax", lifespanMax);
        ImGui.Spacing();

        ImGui.SeparatorText("Impulse");
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Impulse Min", ref speedMin, 10, 300f)) particleEmitter.ModifyParticleDataProp("speedMin", speedMin);
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Impulse Max", ref speedMax, 10f, 300f)) particleEmitter.ModifyParticleDataProp("speedMax", speedMax);
        ImGui.Spacing();

        ImGui.SeparatorText("Size");
        ImGui.SetNextItemWidth(150);
        if (ImGui.SliderFloat("Size Start Min", ref sizeStartMin, 1, 50f)) particleEmitter.ModifyParticleDataProp("sizeStartMin", sizeStartMin);
        ImGui.SetNextItemWidth(150);
        if (ImGui.SliderFloat("Size Start Max", ref sizeStartMax, 1f, 50f)) particleEmitter.ModifyParticleDataProp("sizeStartMax", sizeStartMax);
        ImGui.SetNextItemWidth(150);
        if (ImGui.SliderFloat("Size End Min", ref sizeEndMin, 1, 50f)) particleEmitter.ModifyParticleDataProp("sizeEndMin", sizeEndMin);
        ImGui.SetNextItemWidth(150);
        if (ImGui.SliderFloat("Size End Max", ref sizeEndMax, 1f, 50f)) particleEmitter.ModifyParticleDataProp("sizeEndMax", sizeEndMax);
        ImGui.Spacing();

        ImGui.SeparatorText("Color");
        ImGui.SetNextItemWidth(250);
        if (ImGui.ColorEdit4("Color Start", ref colorStart)) particleEmitter.ModifyParticleDataProp("colorStart", new Color(colorStart));
        ImGui.SetNextItemWidth(250);
        if (ImGui.ColorEdit4("Color End", ref colorEnd)) particleEmitter.ModifyParticleDataProp("colorEnd", new Color(colorEnd));
        ImGui.Spacing();

        ImGui.SeparatorText("Opacity");
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Opacity Start", ref opacityStart, 0, 1)) particleEmitter.ModifyParticleDataProp("opacityStart", opacityStart);
        ImGui.SetNextItemWidth(250);
        if (ImGui.SliderFloat("Opacity End", ref opacityEnd, 0, 1)) particleEmitter.ModifyParticleDataProp("opacityEnd", opacityEnd);
        ImGui.Spacing();

        ImGui.SeparatorText("Other");
        ImGui.SetNextItemWidth(250);
        if (ImGui.Checkbox("Visible", ref visible)) particleEmitter.ModifyParticleDataProp("visible", visible);
        ImGui.SetNextItemWidth(250);
        if (ImGui.Checkbox("Draw On Loading", ref drawOnLoading)) particleEmitter.drawOnLoading = drawOnLoading;
        ImGui.Spacing();

        ImGui.EndDisabled();

        ImGui.EndChild();
        ImGui.EndGroup();
    }

    public static async void Update()
    {
        refreshTimer += Globals.DEBUG_DT;
        if (refreshTimer > 20)
        {
            await Unload();
        }
    }

    public static void Save()
    {
        emitterName = saveName;

        string[] data = {
            $"angleVariance:{angleVariance}",
            $"lifeSpanMin:{lifespanMin}",
            $"lifeSpanMax:{lifespanMax}",
            $"speedMin:{speedMin}",
            $"speedMax:{speedMax}",
            $"sizeStartMin:{sizeStartMin}",
            $"sizeStartMax:{sizeStartMax}",
            $"sizeEndMin:{sizeEndMin}",
            $"sizeEndMax:{sizeEndMax}",
            $"colorStart:{colorStart}",
            $"colorEnd:{colorEnd}",
            $"opacityStart:{opacityStart}",
            $"opacityEnd:{opacityEnd}",
            $"interval:{interval}",
            $"emitCount:{emitCount}",
            $"visible:{visible}",
            $"textureName:{textureName}"
        };
        File.WriteAllLines($"{Path.Combine(Globals.projectContentFolderPath, "particles", emitterName)}.particles", data);
    }
    public static Task Load(string type = "existing")
    {
        if (type == "new")
        {
            return Task.CompletedTask;
        }

        string path = $"{Path.Combine(Globals.projectContentFolderPath, "particles", emitterName)}.particles";
        if (!File.Exists(path))
        {
            Debug.Error("Path not found!");
            new MessageBox("Path not found!").Show();

            return Task.CompletedTask;
        }

        saveName = emitterName;

        string[] data = File.ReadAllLines(path);
        foreach (var line in data)
        {
            if (line == string.Empty)
                continue;

            string key = line.Split(":")[0];
            string value = line.Split(":")[1];
            switch (key)
            {
                case "angleVariance":
                    angleVariance = float.Parse(value);
                    break;
                case "lifeSpanMin":
                    lifespanMin = float.Parse(value);
                    break;
                case "lifeSpanMax":
                    lifespanMax = float.Parse(value);
                    break;
                case "speedMin":
                    speedMin = float.Parse(value);
                    break;
                case "speedMax":
                    speedMax = float.Parse(value);
                    break;
                case "sizeStartMin":
                    sizeStartMin = float.Parse(value);
                    break;
                case "sizeStartMax":
                    sizeStartMax = float.Parse(value);
                    break;
                case "sizeEndMin":
                    sizeEndMin = float.Parse(value);
                    break;
                case "sizeEndMax":
                    sizeEndMax = float.Parse(value);
                    break;
                case "colorStart":
                    colorStart = Parser.ParseVec4(value);
                    break;
                case "colorEnd":
                    colorEnd = Parser.ParseVec4(value);
                    break;
                case "opacityStart":
                    opacityStart = float.Parse(value);
                    break;
                case "opacityEnd":
                    opacityEnd = float.Parse(value);
                    break;
                case "interval":
                    interval = float.Parse(value);
                    break;
                case "emitCount":
                    emitCount = int.Parse(value);
                    break;
                case "visible":
                    visible = bool.Parse(value);
                    break;
                case "textureName":
                    textureName = value;
                    break;
            }
        }

        return Task.CompletedTask;
    }

    public static void OnAppResize()
    {
        Globals.ENGINE_Main.CreateRenderTarget(Globals.APP_Width, Globals.APP_Height);
    }
}
