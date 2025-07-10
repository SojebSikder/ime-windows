using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class NativeMethods
    {
        private static IntPtr hookID = IntPtr.Zero;

        // Windows API Imports
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        // buffer/internal temp text
        private static string previousString = string.Empty;
        private static string currentString = string.Empty;
        private static readonly List<char> inputBuffer = new List<char>();

        /// <summary>
        /// Sets up the low-level keyboard hook
        /// </summary>
        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            try
            {
                using Process curProcess = Process.GetCurrentProcess();
                using ProcessModule curModule = curProcess.MainModule;

                if (curModule == null)
                    throw new Exception("Failed to get process module.");

                return SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    proc,
                    GetModuleHandle(curModule.ModuleName),
                    0
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting up keyboard hook: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Callback for low-level keyboard events
        /// </summary>
        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN))
            {
                try
                {
                    KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                    Keys key = (Keys)hookStruct.vkCode;

                    if (!Keyboard.IsModifierKey(key))
                    {
                        char? keyChar = Keyboard.KeyToPrintableChar(key);
                        if (keyChar.HasValue)
                        {
                            previousString = currentString;
                            inputBuffer.Add(keyChar.Value);
                            currentString = new string(inputBuffer.ToArray());
                        }
                    }
                    if (key == Keys.Space || key == Keys.Enter)
                    {
                        Debug.WriteLine("clearing");
                        previousString = "";
                        inputBuffer.Clear();
                        currentString = string.Empty;
                    }

                    if (LayoutParser.KeyReplacements.TryGetValue(key, out string replacement))
                    {
                        // Contextual logic: if "আ" and there is previous char, replace with "া"
                        if (replacement == "আ" && !string.IsNullOrEmpty(previousString))
                        {
                            replacement = "া";

                            if (inputBuffer.Count > 0)
                            {
                                inputBuffer[inputBuffer.Count - 1] = 'া';
                            }
                            else
                            {
                                inputBuffer.Add('া');
                            }

                            currentString = new string(inputBuffer.ToArray());
                        }
                        else
                        {
                            // Default behavior
                            currentString = replacement;
                            inputBuffer.Clear();
                            foreach (char c in replacement)
                                inputBuffer.Add(c);
                        }

                        Debug.WriteLine($"Key {key} replaced with {replacement}");
                        SendKeys.SendWait(replacement);
                        return (IntPtr)1; // Suppress original key
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in keyboard hook callback: {ex.Message}");
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
    }
}
