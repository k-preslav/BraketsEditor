using System.IO;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class ObjectCreator
{
    static bool isInsideJob = false;

    #region File Watcher
    public static void SetupFileWatcher()
    {
        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher{
            Path = Globals.projectPath,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            Filter = "*.*",
            IncludeSubdirectories = true
        };
        fileSystemWatcher.Changed += OnFileChanged;
        fileSystemWatcher.Created += OnFileChanged;
        fileSystemWatcher.Deleted += OnFileChanged;
        fileSystemWatcher.Renamed += OnFileRenamed;

        fileSystemWatcher.EnableRaisingEvents = true;
    }
    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        ObjectsPanel.Refresh();
    }

    private static void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        Debug.Log("Detected file rename: ", e.Name);

        if (isInsideJob)
        {
            isInsideJob = false;
            return;   
        }

        new MessageBox("Renaming a file is not adviced, as it cound break you project!", "Revert", "Continue", (result) => {
            if (result == 1)
            {
                isInsideJob = true;
                File.Move(e.FullPath, e.OldFullPath);
                File.Delete(e.FullPath);

                ObjectsPanel.Refresh();
            }
            else if (result == 2) ObjectsPanel.Refresh();  
        }).Show();
    }
    #endregion

    public static async void CreateSprite(string name, string tag, string texture, float objScale, int layer, bool smu, bool dol)
    {
        name = name.Replace(".cs", "");
        
        // get the template
        string template = await File.ReadAllTextAsync($"{Globals.CurrentDir}/content/templates/code/SpriteTemplate.txt");

        // process it
        template = template
            .Replace("&NAMESPACE&", Globals.projectName)
            .Replace("&NAME&", name)
            .Replace("&TAG&", tag)
            .Replace("&POSX&", "0")
            .Replace("&POSY&", "0")
            .Replace("&TEXTURE_NAME&", texture)
            .Replace("&LAYER&", layer.ToString())
            .Replace("&SMU&", !smu ? $"this.smartUpdate = false;" : "")
            .Replace("&DOL&", dol ? $"this.drawOnLoading = true;" : "")
            .Replace("&SCALE&", dol ? $"this.Scale = {objScale}f;" : "");

        // create the file with the template
        CreateFile("sprites", name, template);

        ImGui.CloseCurrentPopup();
    }

    public static async void CreateParticleEmitter(string name, int layer, string particleDataName, bool smu, bool dol)
    {
        name = name.Replace(".cs", "");
        string template = await File.ReadAllTextAsync($"{Globals.CurrentDir}/content/templates/code/ParticleEmitterTemplate.txt");

        // process it
        template = template
            .Replace("&NAMESPACE&", Globals.projectName)
            .Replace("&NAME&", name)
            .Replace("&POSX&", "0")
            .Replace("&POSY&", "0")
            .Replace("&PARTICLE_DATA&", particleDataName)
            .Replace("&LAYER&", layer.ToString())
            .Replace("&SMU&", !smu ? $"this.smartUpdate = false;" : "")
            .Replace("&DOL&", dol ? $"this.drawOnLoading = true;" : "");

        // create the file with the template
        CreateFile("particles", name, template);

        ImGui.CloseCurrentPopup();
    }
    
    private static void CreateFile(string group, string name, string template)
    {
        string groupPath = $"{Globals.projectGameFolderPath}/{group}";
        if (!Directory.Exists(groupPath))
        {
            Directory.CreateDirectory(groupPath);
        }

        string fullPath = $"{groupPath}/{name}.cs";
        File.WriteAllText(fullPath, template);

        ObjectsPanel.Refresh();
    }

    public static void RemoveFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);

        ObjectsPanel.Refresh();
    }
}