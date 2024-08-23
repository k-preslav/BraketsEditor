using System;
using System.IO;
using BraketsEngine;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            // Since args are given, they are most likely passed by the project manager, so this is an instance of an editor for a project. 
            // Otherwise 'IS_DEV_BUILD' is true, while developing the engine and/or the editor
            Globals.IS_DEV_BUILD = false;

            Globals.projectName = args[0];

            Globals.projectPath = args[1];
            Globals.projectContentFolderPath = $"{Globals.projectPath}/content/";
            Globals.projectGameFolderPath = $"{Globals.projectPath}/Game/"; 
        }

        using var game = new BraketsEngine.Main();
        game.Run();
    }
}   