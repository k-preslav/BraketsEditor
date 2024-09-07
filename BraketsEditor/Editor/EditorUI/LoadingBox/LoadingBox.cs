using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BraketsEditor;

public class LoadingBox : DebugWindow
{
    public float Value { get; private set; }

    private float _targetValue;
    private float _lerpSpeed = 5f;

    private string message = "";

    public LoadingBox(string message)
        : base("Loading...", overridePos: true, overrideSize: true, visible: false,
            flags: ImGuiWindowFlags.AlwaysAutoResize)
    {
        base.Size = new Vector2(350, 125);
        base.Pos = new Vector2(Globals.APP_Width / 2 - base.Size.X / 2, Globals.APP_Height / 2 - base.Size.Y / 2);
        Globals.DEBUG_UI.AddWindow(this);

        this.message = message;
    }

    public void Show()
    {
        this.Visible = true;
        OnDraw += Draw;
    }

    public void Draw()
    {
        if (MathF.Ceiling(Value) >= 100)
        {
            this.Value = 0;
            this.Visible = false;

            return;
        }

        ImGui.TextWrapped(message);
        ImGui.Dummy(new Vector2(5));

        ImGui.ProgressBar(Value / 100, new Vector2(this.Size.X - 20, 35), $"{Value:0}%");

        UpdateValue();
    }

    public void SetValue(float value)
    {
        _targetValue = value;
    }
    public void SetMessage(string msg)
    {
        message = msg;
    }

    void UpdateValue()
    {
        if (_targetValue != Value)
        {
            Value = Microsoft.Xna.Framework.MathHelper.Lerp(Value, _targetValue, _lerpSpeed * Globals.DEBUG_DT);
        }
    }
}
