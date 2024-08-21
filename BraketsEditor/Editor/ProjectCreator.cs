using System.Diagnostics;
using BraketsEngine;

namespace BraketsEditor;

public class ProjectCreator
{
    public static void OpenProjectCreator()
    {
        Process.Start("python3", "main.py");
        Globals.ENGINE_Main.Exit();
    }
}