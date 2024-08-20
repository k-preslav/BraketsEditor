using System.Diagnostics;

namespace BraketsEditor
{
    public class OpenFileInEditor
    {
        public static void Open(string filePath, string folder)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "code",
                Arguments = $"{folder} --goto \"{filePath}\"",
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
    }
}
