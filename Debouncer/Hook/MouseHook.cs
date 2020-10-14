namespace Debouncer
{
    public class MouseHook : Hook<MouseInput>
    {
        private MouseHook() : base(14)
        {
        }

        public static MouseHook CreateInstance()
        {
            if(Instance == null)
                new MouseHook();

            return (MouseHook)Instance;
        }
    }
}