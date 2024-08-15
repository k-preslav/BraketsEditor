using System;
using BraketsEngine;

class Program
{
    static void Main(string[] args)
    {
        bool startBridgeClient = false;
        string hostname = "";
        int port = 0;

        if (args.Length > 0 && args.Length < 3)
        {
            startBridgeClient = args[0] == "bridge";
            hostname = args[1];
            port = int.Parse(args[2]);
        }
        if (startBridgeClient)
        {
            Globals.BRIDGE_Run = true;
            Globals.BRIDGE_Hostname = hostname;
            Globals.BRIDGE_Port = port;
        }

        Console.WriteLine("Hello, Curious Player!");

        using var game = new BraketsEngine.Main();
        game.Run();
    }
}