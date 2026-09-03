using System.Runtime.InteropServices;

namespace rans0m
{
    internal static class DragDropFix
    {
        private const uint WM_DROPFILES = 0x0233;
        private const uint WM_COPYDATA = 0x004A;
        private const uint WM_COPYGLOBALDATA = 0x0049;
        private const uint MSGFLT_ALLOW = 1;

        [DllImport("user32.dll")]
        private static extern bool ChangeWindowMessageFilterEx(
            IntPtr hwnd, uint msg, uint action, IntPtr pChangeFilterStruct);

        public static void Allow(IntPtr hwnd)
        {
            ChangeWindowMessageFilterEx(hwnd, WM_DROPFILES, MSGFLT_ALLOW, IntPtr.Zero);
            ChangeWindowMessageFilterEx(hwnd, WM_COPYDATA, MSGFLT_ALLOW, IntPtr.Zero);
            ChangeWindowMessageFilterEx(hwnd, WM_COPYGLOBALDATA, MSGFLT_ALLOW, IntPtr.Zero);
        }
    }
}