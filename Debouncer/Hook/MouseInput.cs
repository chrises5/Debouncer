using System;
using System.Runtime.InteropServices;

namespace Debouncer
{
    public class MouseInput : Input
    {
        public enum WindowsCode
        {
            L_DOWN = 0x0201,
            L_UP = 0x0202,
            R_DOWN = 0x0204,
            R_UP = 0x0205,
            M_DOWN = 0x207,
            M_UP = 0x208,
            X_DOWN = 0x20B,
            X_UP = 0x20C,
            WHEEL = 0x020A,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public HookImports.POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public string Name { protected set; get; }
        public MSLLHOOKSTRUCT HookStruct => (MSLLHOOKSTRUCT)Marshal.PtrToStructure(LParam, typeof(MSLLHOOKSTRUCT));

        public override void Init(int ncode, IntPtr wparam, IntPtr lparam)
        {
            WParam = wparam;
            LParam = lparam;
            NCode = ncode;
            Name = Enum.GetName(typeof(WindowsCode), wparam.ToInt32());
        }
    }
}