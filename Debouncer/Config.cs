using System.Collections.Generic;
using System.Diagnostics;

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
        private static readonly string configPath = "config.json";

        public void Save()
        {
            this.WriteObjectToFile(configPath);
        }

        public static Config Load()
        {
            return JsonFile.LoadObjectFromFile<Config>(configPath);
        }
    }
}