using System.Diagnostics;
using BraketsEngine;

namespace BraketsEditor;

public class ProjectManager
{
    public static void NewProject()
    {
        Process.Start(GetPyVer.BasedOnOS(), "main.py --start new");
        Globals.ENGINE_Main.Exit();
    }

    public static void OpenProject()
    {
        Process.Start(GetPyVer.BasedOnOS(), "main.py --start open");
        Globals.ENGINE_Main.Exit();
    }
}