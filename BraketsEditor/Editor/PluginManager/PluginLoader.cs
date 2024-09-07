using BraketsEditor;
using BraketsEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace BraketsPluginIntegration;

public class PluginLoader
{
    public List<IEditorPlugin> Plugins { get; set; } = new List<IEditorPlugin>();
    public List<AssemblyLoadContext> Contexts { get; set; } = new List<AssemblyLoadContext>();

    public async Task LoadPlugins(LoadingBox loadingBox)
    {
        string path = Path.Combine(Globals.CurrentDir, "Plugins");
        if (!Directory.Exists(path))
        {
            Debug.Warning("Plugins cannot be loaded, as 'Plugins' folder is not found!");
            return;
        }

        string[] plugins = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

        float allPlugins = plugins.Length;
        float currentPlugin = 0.0f;

        await Task.Run(async () =>
        {
            foreach (var pluginPath in plugins)
            {
                currentPlugin++;
                string pluginName = Path.GetFileNameWithoutExtension(pluginPath);

                float progress = 50 + (currentPlugin / allPlugins) * 50.0f;
                loadingBox.SetValue(progress);
                loadingBox.SetMessage($"Loading plugin: '{pluginName}'");
                
                RunPlugin(pluginPath, pluginName);

                await Task.Delay(10);
            }
        });
    }

    public void RunPlugin(string path, string name)
    {
        Debug.Log("Loading plugin: " + name);

        var context = new AssemblyLoadContext(name, isCollectible:true);
        Contexts.Add(context);

        Assembly assembly = context.LoadFromAssemblyPath(path);

        // Get all types in the assembly that implement the IPlugin interface
        Type[] pluginTypes = assembly.GetTypes().Where(t => typeof(IEditorPlugin).IsAssignableFrom(t)).ToArray();

        // Check if there is exactly one type that implements the IPlugin interface
        if (pluginTypes.Length == 1)
        {
            // Create the plugin
            Type pluginType = pluginTypes[0];
            IEditorPlugin plugin = (IEditorPlugin)Activator.CreateInstance(pluginType);
            Plugins.Add(plugin);
        }
        else
        {
            Debug.Error("None or multiple plugin types found.");
        }
    }

    public async void CallPInit()
    {
        foreach (var plugin in Plugins)
        {
            await Task.Run(() =>
            {
                plugin.Initialize();
            });
        }
    }

    public void UnloadPlugins()
    {
        foreach (var context in Contexts)
        {
            context.Unload();
        }

        Plugins.Clear();
        Contexts.Clear();
    }
}
