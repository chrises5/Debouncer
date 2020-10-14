using System;
using System.Threading;

namespace Debouncer
{
    public abstract class Hook<InputType> : IInputSource<InputType> where InputType : Input, new()
    {
        public event EventHandler<InputType> InputRegisterd;
        public event EventHandler<Exception> ExceptionRaised;

        private readonly HookImports.HookProc proc = HookCallback;
        private readonly int hookId = 0;
        private IntPtr hookHandle = IntPtr.Zero;
        private Thread hookThread = null;
        public static Hook<InputType> Instance { private set; get; } = null;

        protected Hook(int hookId)
        {
            this.hookId = hookId;
            Instance = this;
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode < 0)
                return HookImports.CallNextHookEx(Instance.hookHandle, nCode, wParam, lParam);

            try
            {
                InputType input = new InputType();
                input.Init(nCode, wParam, lParam);
                Instance.InputRegisterd?.Invoke(Instance, input);
                if (input.Block)
                    return new IntPtr(-1);
            }
            catch (Exception e)
            {
                Instance.ExceptionRaised?.Invoke(Instance, e);
            }

            return HookImports.CallNextHookEx(Instance.hookHandle, nCode, wParam, lParam);
        }

        public void SetHook()
        {
            hookThread = new Thread(() =>
            {
                hookHandle = HookImports.SetWindowsHookEx(hookId, proc, IntPtr.Zero, 0);
                HookImports.MSG msg = new HookImports.MSG();
                while (HookImports.GetMessage(ref msg, IntPtr.Zero, 0, 0))
                {
                    HookImports.TranslateMessage(ref msg);
                    HookImports.DispatchMessage(ref msg);
                }
            })
            {
                Priority = ThreadPriority.Highest
            };
            hookThread.Start();
        }

        public void RemoveHook()
        {
            HookImports.UnhookWindowsHookEx(hookHandle);
            hookThread.Abort();
        }
    }
}