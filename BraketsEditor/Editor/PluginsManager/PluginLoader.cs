using BraketsEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace BraketsEditor.Editor.PluginsManager
{
    public class PluginLoader
    {
        static List<string> ENTRY_plugins = new List<string>();

        public static void LoadPlugins()
        {
            string path = Path.Combine(Globals.CurrentDir, "Plugins");
            string[] plugins = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

            foreach (var plugin in plugins)
            {
                if (plugin.Contains("ENTRY")) ENTRY_plugins.Add(plugin.Replace("ENTRY", ""));

                string pluginName = Path.GetFileNameWithoutExtension(plugin);
                RunPlugin(plugin, pluginName);
            }
        }

        static void RunPlugin(string code, string name)
        {
            
        }
    }
}
