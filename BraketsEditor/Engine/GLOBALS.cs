using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BraketsEditor;
using BraketsPluginIntegration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BraketsEngine;

public static class Globals
{
    #region ENGINE SYSTEMS
    public static GraphicsDevice ENGINE_GraphicsDevice;
    public static SpriteBatch ENGINE_SpriteBatch;
    public static Main ENGINE_Main;
    #endregion

    #region APPLICATION PROPERTIES
    public static string APP_Title = "Brakets Editor";
    public static string APP_Version = "INDEV 0.7";
    public static int APP_Width = 1280;
    public static int APP_Height = 720;
    public static bool APP_Resizable = true;
    public static bool APP_VSync = true;
    public static void LOAD_APP_P()
    {
        Debug.Log("[GLOBALS] Loading application properties...");
        //TODO: Load application properties from JSON File
    }
    #endregion

    #region APPLICATION
    public static bool STATUS_Loading = false;
    public static string CurrentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    #endregion

    #region EDITOR
    public static Camera Camera;
    public static EditorManager EditorManager;
    public static GlobalMenuBar GlobalMenuBar;
    #endregion

    #region DEBUG
    public static float DEBUG_FPS = 0;
    public static float DEBUG_DT = 0;
    public static bool DEBUG_Overlay;
    public static DebugUI DEBUG_UI;
    public static int DEBUG_UI_MENUBAR_SIZE_X;
    public static int DEBUG_UI_MENUBAR_SIZE_Y;
    #endregion

    #region PROJECT
    public static string projectName = "Template";
    public static string projectPath = "../../../../../BraketsRaw-Template/BraketsTemplate/";
    public static string projectGameFolderPath = $"{projectPath}/Game/";
    public static string projectContentFolderPath = $"{projectPath}/content/";
    public static bool IS_DEV_BUILD = true;

    public static int projectViewportWidth = 1920;
    public static int projectViewportHeight = 1080;
    public static Vector4 projectViewportColor = Color.MediumSeaGreen.ToVector4();

    #endregion

    #region BRIDGE
    public static BridgeServer GameDebugBridgeServer;
    #endregion
}
