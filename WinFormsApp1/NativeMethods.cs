using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1
{
    internal class NativeMethods
    {
        private static IntPtr hookID = IntPtr.Zero;

        // Windows API Imports
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

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

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private static string previousString = string.Empty;
        private static string currentString = string.Empty;
        private static readonly List<char> inputBuffer = new List<char>();

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

        private static bool isSending = false;
        private static Queue<string> sendQueue = new Queue<string>();
        private static System.Windows.Forms.Timer sendTimer;

        public static void InitializeSendTimer()
        {
            sendTimer = new System.Windows.Forms.Timer();
            sendTimer.Interval = 10; // 10 ms delay
            sendTimer.Tick += (s, e) =>
            {
                if (sendQueue.Count > 0)
                {
                    string keys = sendQueue.Dequeue();
                    SendKeys.SendWait(keys);
                }
                else
                {
                    sendTimer.Stop();
                    isSending = false;
                }
            };
        }


        private static bool _suppressNextA = false;
        private static bool _suppressNextB = false;
        private static bool suppressNext = false;


        static int counter = 0;
        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                try
                {
                    KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                    Keys key = (Keys)hookStruct.vkCode;

                    // Check if shift is pressed
                    bool isShiftPressed = (GetKeyState((int)Keys.ShiftKey) & 0x8000) != 0;
                    bool isCtrlPressed = (GetKeyState((int)Keys.ControlKey) & 0x8000) != 0;
                    bool isAltPressed = (GetKeyState((int)Keys.Menu) & 0x8000) != 0;

                    if (!isCtrlPressed && !isAltPressed)
                    {
                        if (!Keyboard.IsModifierKey(key))
                        {
                            char? keyChar = Keyboard.KeyToPrintableChar(key);
                            if (keyChar.HasValue)
                            {
                                if (!suppressNext)
                                {
                                    previousString = currentString;
                                    inputBuffer.Add(keyChar.Value);
                                    currentString = new string(inputBuffer.ToArray());

                                    counter++;
                                    Debug.WriteLine(counter);
                                }
                            }
                        }

                        // Clear buffer on whitespace or enter
                        if (key == Keys.Space || key == Keys.Enter)
                        {
                            string bn = Keyboard.ExecutePhonetic(currentString);

                            string temp = currentString;

                            // remove buffer
                            previousString = "";
                            inputBuffer.Clear();
                            currentString = string.Empty;

                            if (_suppressNextB)
                            {
                                _suppressNextB = false;
                            }
                            else
                            {
                                _suppressNextB = true;
                                suppressNext = true;

                                // Erase English characters by sending backspaces
                                for (int i = 0; i < temp.Length; i++)
                                {
                                    SendKeys.SendWait("{BACKSPACE}");
                                }

                                SendKeys.SendWait(bn);

                                SendKeys.SendWait("{BACKSPACE}");
                                SendKeys.SendWait(" ");

                                suppressNext = false;

                                return (IntPtr)1;
                            }
                        }

                        // if delete or backspace, remove last character
                        if (key == Keys.Back || key == Keys.Delete)
                        {
                            if (inputBuffer.Count > 0)
                            {
                                inputBuffer.RemoveAt(inputBuffer.Count - 1);
                                currentString = new string(inputBuffer.ToArray());
                            }
                            Debug.WriteLine(new string(inputBuffer.ToArray()));
                        }


                        if (LayoutParser.KeyReplacements.TryGetValue(key, out var banglaKey))
                        {
                            if (_suppressNextA)
                            {
                                // Suppress this key as it's triggered by SendKeys
                                _suppressNextA = false;
                            }
                            else
                            {
                                _suppressNextA = true;

                                string enChar = key.ToString().ToLower();
                                suppressNext = true;
                                SendKeys.Send(enChar);
                                suppressNext = false;
                                return (IntPtr)1;
                            }
                        }
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
