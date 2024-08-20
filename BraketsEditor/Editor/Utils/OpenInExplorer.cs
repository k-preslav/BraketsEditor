using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using BraketsEngine;

namespace BraketsEditor;

public class OpenInExplorer
{
    public static void OpenContentFolder()
    {
        Open($"{Globals.projectContentFolderPath}");
    }
    public static void OpenGameFolder()
    {
        Open($"{Globals.projectGameFolderPath}");
    }

    static void Open(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start("explorer.exe", path);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", path);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", path);
        }
        else
        {
            BraketsEngine.Debug.Error("[OPEN_IN_EXPLORER] Unsupported operating system.");
        }
    }
}