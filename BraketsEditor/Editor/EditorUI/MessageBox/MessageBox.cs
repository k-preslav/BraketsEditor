using System;
using System.Numerics;
using System.Threading.Tasks;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class MessageBox : DebugWindow
{
    private string message = "";
    private string option1Text = "";
    private string option2Text = "";
    private int result = 0;
    private Action<int> callback;

    public MessageBox(string message, string option1="Ok", string option2="", Action<int> callback=null) 
        : base("Attention", overridePos:true, overrideSize:true, visible: false, topmost: true, 
            flags: ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize)
    {
        base.Size = new Vector2(ImGui.CalcTextSize(message).X + 35, 120);
        base.Pos = new Vector2(Globals.APP_Width / 2 - base.Size.X / 2, Globals.APP_Height / 2 - base.Size.Y / 2);
        Globals.DEBUG_UI.AddWindow(this);
        
        this.callback = callback;
        this.message = message;
        this.option1Text = option1;
        this.option2Text = option2;
    }

    public void Show()
    {
        this.Visible = true;
        OnDraw += Draw;
    }

    public void Draw(DebugWindow parent)
    {
        ImGui.TextWrapped(message);
        
        ImGui.Dummy(new Vector2(5));

        if (option2Text != string.Empty)
        {
            ImGui.SetCursorPos(new Vector2(this.Size.X - (ImGui.CalcTextSize(option2Text).X + ImGui.CalcTextSize(option1Text).X + 45), this.Size.Y - 45));
            if (ImGui.Button(option2Text))
            {
                result = 2;
                Visible = false;
                if (callback is not null)
                    callback(result);
            }
            ImGui.SameLine();
        }

        ImGui.SetCursorPos(new Vector2(this.Size.X - (ImGui.CalcTextSize(option1Text).X + 25), this.Size.Y - 45));
        WindowTheme.PushAccent();
        if (ImGui.Button(option1Text))
        {
            result = 1;
            Visible = false;
            if (callback is not null) 
                callback(result);
        }
        WindowTheme.PopAccent();
    }
}

