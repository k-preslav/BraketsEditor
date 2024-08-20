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
        ObjectsWindow.Refresh();
    }

    private static void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        if (isInsideJob)
        {
            isInsideJob = false;
            return;   
        }

        new Attention("Renaming a file is not adviced, as it cound break you project!", "Revert", (result) => {
            if (result == true)
                ObjectsWindow.Refresh();
            else
            {
                isInsideJob = true;
                File.Move(e.FullPath, e.OldFullPath);
                File.Delete(e.FullPath);
            }     
        }).Show();
    }
    #endregion

    public static async void CreateSprite(string name)
    {
        name = name.Replace(".cs", "");
        
        // get the template
        string template = await File.ReadAllTextAsync($"{Directory.GetCurrentDirectory()}/content/templates/code/SpriteTemplate.txt");
        
        // process it
        template = template
            .Replace("&NAMESPACE&", Globals.projectName)
            .Replace("&NAME&", name)
            .Replace("&TAG&", "test_tag") // TODO: Fix the TAG, POS, TEXTURE_NAME, and LAYER props to be set when creating with ui
            .Replace("&POSX&", "0")
            .Replace("&POSY&", "0")
            .Replace("&TEXTURE_NAME&", "default_texture")
            .Replace("&LAYER&", "0");

        // create the file with the template
        CreateFile("sprites", name, template);

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

        ObjectsWindow.Refresh();
    }

    public static void RemoveFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);

        ObjectsWindow.Refresh();
    }
}