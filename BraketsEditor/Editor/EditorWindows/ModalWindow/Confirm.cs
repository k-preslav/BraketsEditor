using System;
using System.Numerics;
using System.Threading.Tasks;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class Confirm : DebugWindow
{
    private string message = "";
    private bool result = false;
    private Action<bool> callback;

    public Confirm(string message, Action<bool> callback) 
        : base("Confirm?", overridePos:true, overrideSize:true, visible: false, flags: ImGuiWindowFlags.Modal)
    {
        base.Size = new Vector2(ImGui.CalcTextSize(message).X + 25, 115);
        base.Pos = new Vector2(Globals.APP_Width / 2 - base.Size.X / 2, Globals.APP_Height / 2 - base.Size.Y / 2);
        Globals.DEBUG_UI.AddWindow(this);
        
        this.callback = callback;
        this.message = message;
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
        ImGui.SetCursorPos(new Vector2(this.Size.X - 90, this.Size.Y - 40));
        if (ImGui.Button("Yes"))
        {
            result = true;
            Visible = false;
            callback(result);
        }
        ImGui.SameLine();
        if (ImGui.Button("No"))
        {
            result = false;
            Visible = false;
            callback(result);
        }
    }
}

