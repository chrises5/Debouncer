using System;
using System.IO;
using IWshRuntimeLibrary;
using System.Diagnostics;


namespace Installer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var folder = Path.Combine(programFiles, "Debouncer");
            Directory.CreateDirectory(folder);
            var fullpath = Path.Combine(folder, "Debouncer.exe");

            using (var fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(Resource1.Debouncer, 0, Resource1.Debouncer.Length);
            }

            var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            WshShell shell = new WshShell();
            string shortcutAddress = Path.Combine(startupFolder, "Debouncer.lnk");
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Shortcut to Debouncer.exe";
            shortcut.TargetPath = fullpath;
            shortcut.Save();

            Process.Start(fullpath);
        }
    }
}
