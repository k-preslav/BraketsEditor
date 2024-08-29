using System.Diagnostics;
using System.Runtime.InteropServices;

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
                UseShellExecute = true,
                CreateNoWindow = true,
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            Process.Start(startInfo);
        }
    }
}
