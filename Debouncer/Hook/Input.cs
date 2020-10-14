using System;

namespace Debouncer
{
    public abstract class Input
    {
        public abstract void Init(int ncode, IntPtr wparam, IntPtr lparam);
        public IntPtr WParam { protected set; get; }
        public IntPtr LParam { protected set; get; }
        public int NCode { protected set; get; }
        public bool Block { get; set; }
    }
}