using System.Diagnostics;
using BraketsEngine;

namespace BraketsEditor;

public class ProjectManager
{
    public static void NewProject()
    {
        Process.Start("python3", "main.py --start new");
        Globals.ENGINE_Main.Exit();
    }

    public static void OpenProject()
    {
        Process.Start("python3", "main.py --start open");
        Globals.ENGINE_Main.Exit();
    }
}