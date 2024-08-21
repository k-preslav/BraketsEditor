using System;
using System.IO;
using BraketsEngine;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            Globals.projectName = args[0];

            Globals.projectPath = args[1];
            Globals.projectContentFolderPath = $"{Globals.projectPath}/content/";
            Globals.projectGameFolderPath = $"{Globals.projectPath}/Game/"; 
        }

        using var game = new BraketsEngine.Main();
        game.Run();
    }
}   