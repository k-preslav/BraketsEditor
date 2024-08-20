using System;
using System.Collections.Generic;
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
    public static string APP_Version = "indev 0.0.1";
    public static int APP_Width = 1280;
    public static int APP_Height = 720;
    public static bool APP_VSync = true;
    public static void LOAD_APP_P()
    {
        Debug.Log("[GLOBALS] Loading application properties...");
        //TODO: Load application properties from JSON File
    }
    #endregion

    #region APPLICATION STATUS
    public static bool STATUS_Loading = false;
    #endregion

    #region GAME
    public static Camera Camera;
    public static EditorManager EditorManager;
    #endregion

    #region DEBUG
    public static float DEBUG_FPS = 0;
    public static float DEBUG_DT = 0;
    public static bool DEBUG_Overlay;
    public static DebugUI DEBUG_UI;
    public static int DEBUG_UI_MENUBAR_SIZE_X;
    public static int DEBUG_UI_MENUBAR_SIZE_Y;
    #endregion

    #region EDITOR
    public static string projectName = "Test";
    public static string projectPath = "../../projects/Test/Test/";
    public static string projectGameFolderPath = "../../projects/Test/Test/Game/";
    public static string projectContentFolderPath = "../../projects/Test/Test/content/";

    #endregion

    #region BRIDGE
    public static BridgeClient BRIDGE_Client;
    public static bool BRIDGE_Run = false;
    public static string BRIDGE_Hostname = "";
    public static int BRIDGE_Port = 0;
    public static string BRIDGE_SERVER_CurrentMessage = "";
    #endregion
}
