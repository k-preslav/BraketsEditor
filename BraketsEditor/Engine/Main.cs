using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BraketsEditor;
using BraketsEditor.Editor;
using BraketsEditor.Editor.Contents.AddContentWindow;
using FontStashSharp;
using ImageMagick;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

namespace BraketsEngine;

public class Main : Game
{
    public List<Sprite> Sprites;
    public List<UIElement> UI;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private EditorManager _gameManager;
    private DebugUI _debugUi;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "PreloadContent";
        IsMouseVisible = true;

        this.Exiting += OnExit;
        this.Window.ClientSizeChanged += OnResize;

        Window.FileDrop += OnFileDrop;
    }

    protected override void Initialize()
    {
        Debug.Log("Calling Initialize()", this);

        _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
        _gameManager = new EditorManager();

        Globals.LOAD_APP_P();
        IsFixedTimeStep = false;

        Debug.Log("Applying application properties...", this);
        Window.Title = Globals.APP_Title;
        Window.AllowUserResizing = Globals.APP_Resizable;
        _graphics.PreferredBackBufferWidth = Globals.APP_Width;
        _graphics.PreferredBackBufferHeight = Globals.APP_Height;
        _graphics.SynchronizeWithVerticalRetrace = Globals.APP_VSync;
        _graphics.ApplyChanges();

        Globals.ENGINE_Main = this;
        Globals.ENGINE_GraphicsDevice = _graphics.GraphicsDevice;
        Globals.ENGINE_SpriteBatch = _spriteBatch;

        _debugUi = new DebugUI();
        _debugUi.Initialize(this);

        if (Debugger.IsAttached)
            Globals.DEBUG_Overlay = true;

        new Camera(Vector2.Zero);

        this.Sprites = new List<Sprite>();
        this.UI = new List<UIElement>();

        _graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 1.25);
        _graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.25);
        _graphics.ApplyChanges();
        OnResize(Window, EventArgs.Empty);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Debug.Log("Starting EditorManager...", this);
        _gameManager.Start();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.GetKeyboardState();
        Input.GetMouseState();

        if (Input.IsPressed(Keys.F3))
            Globals.DEBUG_Overlay = !Globals.DEBUG_Overlay;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Globals.DEBUG_DT = dt;
        Globals.DEBUG_FPS = 1 / dt;

        Globals.Camera.CalculateMatrix();

        ParticleManager.Update();
        foreach (var elem in UI.ToList())
        {
            elem.Update(dt);
            elem.UpdateRect();
        }

        if (Globals.STATUS_Loading || LoadingScreen.isLoading)
            return;

        _gameManager.Update(dt);
        foreach (var sp in Sprites.ToList())
        {
            sp.Update(dt);
            sp.UpdateRect();
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Globals.Camera.BackgroundColor); 
        
        // ------- Game Layer -------
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: Globals.Camera.TranslationMatrix);
        
        if (!Globals.STATUS_Loading && !LoadingScreen.isLoading)
        {
            var sortedSprites = Sprites.OrderBy(sp => sp.Layer).ToList();
            foreach (var sp in sortedSprites)
            {
                sp.Draw();
            }
        }
        _spriteBatch.End();

        // ------- UI Layer ------- 
        _spriteBatch.Begin();
        foreach (var elem in UI.ToList())
        {
            if (LoadingScreen.isLoading)
            {
                if (elem.Tag.Contains("__loading__"))
                {
                    elem.DrawUI();
                }

                _spriteBatch.End();
                return;
            } 
            
            elem.DrawUI();
        }
        _debugUi.DrawOverlay(_spriteBatch, 0.25f);
        _debugUi.DrawWindows(gameTime);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    public void AddSprite(Sprite sp) => Sprites.Add(sp);
    public void RemoveSprite(Sprite sp) => Sprites.Remove(sp);

    public void AddUIElement(UIElement elem) => UI.Add(elem);
    public void RemoveUIElement(UIElement elem) => UI.Remove(elem);

    private void OnFileDrop(object sender, FileDropEventArgs e)
    {
        AddContentWindow.files.Clear();
        AddContentWindow.filesFull.Clear();

        var extensions = new Dictionary<string, int>();
        var filesToRetain = new List<string>();

        foreach (var file in e.Files)
        {
            string extension = Path.GetExtension(file).ToLower();
            if (extensions.ContainsKey(extension)) extensions[extension]++;
            else extensions[extension] = 1;
        }

        var mostCommonExtension = extensions.OrderByDescending(kv => kv.Value).First().Key;
        
        foreach (var file in e.Files)
        {
            if (Path.GetExtension(file).ToLower() == mostCommonExtension)
            {
                AddContentWindow.files.Add(Path.GetFileNameWithoutExtension(file));
                AddContentWindow.filesFull.Add(file);
            }
        }

        AddContentWindow.UpdateOptionType();
        Globals.DEBUG_UI.GetWindow("Add new Content").Visible = true;
    }

    private void OnResize(object sender, EventArgs e)
    {
        Globals.APP_Width = Window.ClientBounds.Width;
        Globals.APP_Height = Window.ClientBounds.Height;
    }
    private void OnExit(object sender, EventArgs e)
    {
        Debug.Log("Calling OnExit()");
        _gameManager.Stop();
    }
}