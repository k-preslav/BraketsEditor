using System;
using System.Numerics;
using System.Threading.Tasks;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class Attention : DebugWindow
{
    private string message = "";
    private string option2Text = "";
    private bool result = false;
    private Action<bool> callback;

    public Attention(string message, string option2="", Action<bool> callback=null) 
        : base("Attention", overridePos:true, overrideSize:true, visible: false, flags: ImGuiWindowFlags.Modal)
    {
        base.Size = new Vector2(ImGui.CalcTextSize(message).X + 25, 115);
        base.Pos = new Vector2(Globals.APP_Width / 2 - base.Size.X / 2, Globals.APP_Height / 2 - base.Size.Y / 2);
        Globals.DEBUG_UI.AddWindow(this);
        
        this.callback = callback;
        this.message = message;
        this.option2Text = option2;
    }

    public void Show()
    {
        this.Visible = true;
        OnDraw += Draw;
    }

    public void Draw(DebugWindow parent)
    {
        ImGui.Text(message);
        
        ImGui.Dummy(new Vector2(5));
        ImGui.SetCursorPos(new Vector2(this.Size.X - 110, this.Size.Y - 40));

        if (option2Text != string.Empty)
        {
            if (ImGui.Button(option2Text))
            {
                result = false;
                Visible = false;
                callback(result);
            }
            ImGui.SameLine();
        }
        if (ImGui.Button("OK"))
        {
            result = true;
            Visible = false;
            callback(result);
        }
    }
}

