using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BraketsEditor;
using FontStashSharp;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;

namespace BraketsEngine;

public class DebugWindow
{
    public Action<DebugWindow> OnDraw;
    public Action OnUpdate;
    public string Name;
    public Vector2 Pos;
    public Vector2 Size;
    public bool Visible;
    
    private ImGuiWindowFlags _flags;
    private bool _overridePos;
    private bool _overrideSize;
    private bool _closable;

    public DebugWindow(string name, bool overridePos=false, bool overrideSize=false, 
        int posx=0, int posy=0, int width=320, int height=180, bool closable=false,
        bool visible=true, ImGuiWindowFlags flags=ImGuiWindowFlags.None)
    {
        this.Name = name;
        this.Pos = new Vector2(posx, posy);
        this.Size = new Vector2(width, height);
        this._flags = flags;

        this._overridePos = overridePos;
        this._overrideSize = overrideSize;

        this.Visible = visible;
        this._closable = closable;
    }

    public virtual void Draw(ImFontPtr font)
    {
        if (OnDraw is not null && this.Visible)
        {
            if (_overrideSize) ImGui.SetNextWindowSize(this.Size.ToNumerics());
            if (_overridePos) ImGui.SetNextWindowPos(this.Pos.ToNumerics());
            
            if (_closable) ImGui.Begin(this.Name, ref Visible, this._flags);
            else ImGui.Begin(this.Name, this._flags);
            
            ImGui.PushFont(font);
            OnDraw?.Invoke(this);
            ImGui.PopFont();
            ImGui.End();
        }
    }

    public virtual void Update()
    {
        OnUpdate?.Invoke();
    }
}

public class DebugUI
{
    private List<DebugWindow> _windows = new List<DebugWindow>();
    private ImGuiRenderer _renderer;
    private ImFontPtr _debug_windows_font;
    private Action _menuBar;

    public void Initialize(Game owner)
    {
        _renderer = new ImGuiRenderer(owner);

        this._debug_windows_font = ImGui.GetIO().Fonts.AddFontFromFileTTF($"{Globals.CurrentDir}/content/fonts/NeorisMedium.ttf", 20);
        // TODO: Make the font size 'settable' thru settings

        _renderer.RebuildFontAtlas();
        Globals.DEBUG_UI = this;
    }
    public void DrawWindows(GameTime gameTime)
    {
        if (_renderer is null)
            return;

        _renderer.BeginLayout(gameTime);

        _menuBar?.Invoke();
        foreach (var window in _windows.ToList())
        {
            window.Draw(_debug_windows_font);
        }
        _renderer.EndLayout();
    }

    public void UpdateWindow()
    {
        foreach (var win in _windows)
        {
            win.Update();
        }
    }
    
    public void AddWindow(DebugWindow win)
    {
        _windows.Add(win);
    }
    public void AddMenuBar(Action menuBar) => _menuBar = menuBar;
    public DebugWindow GetWindow(string name)
    {
        foreach (var window in _windows)
        {
            if (window.Name == name) 
                return window;
        }

        Debug.Warning($"Failed to find Debug Window: {name}", this);
        return null;
    }

    // ----- OVERLAY -------
    private float _refreshRate;
    private float _refreshTimer;
    private float _currentFps;

    public void DrawOverlay(SpriteBatch sp, float refreshRate)
    {
        if (!Globals.DEBUG_Overlay)
            return;

        _refreshRate = refreshRate;

        _refreshTimer -= Globals.DEBUG_DT;
        if (_refreshTimer <= 0)
        {
            _currentFps = Globals.DEBUG_FPS;
            _refreshTimer = refreshRate;
        }

        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 32), $"{_currentFps.ToString("0.0")} fps", 
            new Vector2(10, 10), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 3
        );
        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 24), $"Version: {Globals.APP_Version}", 
            new Vector2(10,45), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 2
        );
        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 24), $"VSync: {Globals.APP_VSync}", 
            new Vector2(10, 70), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 2
        );
        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 24), $"Sprites: {Globals.ENGINE_Main.Sprites.Count}", 
            new Vector2(10, 95), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 2
        );
    }
}