using System;
using System.IO;
using System.Windows.Forms;

namespace Debouncer
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Logger.CreateInstance(Directory.GetCurrentDirectory());
            Config config;
            try
            {
                config = Config.Load();
            }
            catch (Exception)
            {
                config = new Config
                {
                    { (int)MouseInput.WindowsCode.L_DOWN, new MouseInputConfig(110) },
                    { (int)MouseInput.WindowsCode.L_UP, new MouseInputConfig(110) },

                    { (int)MouseInput.WindowsCode.R_DOWN, new MouseInputConfig(110) },
                    { (int)MouseInput.WindowsCode.R_UP, new MouseInputConfig(110) },

                    { (int)MouseInput.WindowsCode.M_DOWN, new MouseInputConfig(110) },
                    { (int)MouseInput.WindowsCode.M_UP, new MouseInputConfig(110) },

                    { (int)MouseInput.WindowsCode.X_DOWN, new MouseInputConfig(110) },
                    { (int)MouseInput.WindowsCode.X_UP, new MouseInputConfig(110) },

                    { (int)MouseInput.WindowsCode.WHEEL, new MouseInputConfig(110) }
                };

                try
                {
                    config.Save();
                }
                catch(Exception e)
                {
                    Logger.LogException(e);
                }
            }

            MouseHook.CreateInstance().SetHook();
            _ = new MouseDebouncer(MouseHook.Instance, config);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = ConfigForm.CreateInstance(config);
            form.InitNotifyIcon();
            Application.Run();

            MouseHook.Instance.RemoveHook();
        }
    }
}