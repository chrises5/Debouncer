using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Debouncer
{
    public class MouseInputConfig
    {
        public Stopwatch Stopwatch { get; }
        public int Delay { get; set; }
        public bool Debounce { get; set; }

        public MouseInputConfig(int delay, bool debounce = false)
        {
            Stopwatch = new Stopwatch();
            Delay = delay;
            Debounce = debounce;
        }
    }

    public class Config : Dictionary<int, MouseInputConfig>
    {
        private static string ConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Debouncer");
        private static string ConfigFile => Path.Combine(ConfigFolder, "config.json");

        public void Save()
        {
            if(!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);
            this.WriteObjectToFile(ConfigFile);
        }

        public static Config Load()
        {
            return JsonFile.LoadObjectFromFile<Config>(ConfigFile);
        }
    }
}