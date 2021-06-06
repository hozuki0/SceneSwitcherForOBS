using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SceneSwitcherForOBS
{
    public class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}
