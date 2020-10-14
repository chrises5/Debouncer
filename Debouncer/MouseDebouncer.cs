using System;
using System.Threading.Tasks;

namespace Debouncer
{
    public class MouseDebouncer
    {
        private readonly IInputSource<MouseInput> inputSource;
        private readonly Config config;

        public MouseDebouncer(IInputSource<MouseInput> source, Config config)
        {
            this.config = config;
            inputSource = source;
            inputSource.InputRegisterd += InputSource_InputRegisterd;
            inputSource.ExceptionRaised += InputSource_ExceptionRaised;
        }

        private void InputSource_ExceptionRaised(object sender, Exception e)
        {
            Task.Run(() =>
            {
                Logger.LogException(e, "HookCallback");
            });
        }

        private void InputSource_InputRegisterd(object sender, MouseInput e)
        {
            var code = e.WParam.ToInt32();
            if (code == 0x200)
                return;

            ConfigForm.Instance.OnInputRegistered(e);

            if (config[code].Debounce)
            {
                config[code].Stopwatch.Stop();
                if (config[code].Stopwatch.ElapsedMilliseconds < config[code].Delay)
                    e.Block = true;
                config[code].Stopwatch.Reset();
                config[code].Stopwatch.Start();
            }
        }
    }
}