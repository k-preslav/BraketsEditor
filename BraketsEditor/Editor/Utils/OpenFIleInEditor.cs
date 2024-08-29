using BraketsEditor.Editor;
using BraketsEngine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BraketsEditor
{
    public class OpenFileInEditor
    {
        public static async void Open(string filePath, string folder)
        {
            Globals.EditorManager.Status = "Opening code editor...";
            Throbber.visible = true;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "code",
                Arguments = $"{folder} --goto \"{filePath}\"",
                UseShellExecute = true,
                CreateNoWindow = true,
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            await Process.Start(startInfo).WaitForExitAsync();

            Throbber.visible = false;
            Globals.EditorManager.Status = "Ready";
        }
    }
}
