using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;

namespace WinFormsApp1
{
    internal static class Program
    {
        private static IntPtr hookID = IntPtr.Zero;

        // Dictionary to map key replacements
        //private static readonly Dictionary<Keys, string> KeyReplacements = new Dictionary<Keys, string>
        //{
        //    { Keys.A, "α" },
        //    { Keys.B, "β" },
        //    { Keys.C, "γ" }
        //    // Add more key mappings as needed
        //};
        public static Dictionary<Keys, string> KeyReplacements = new Dictionary<Keys, string>();


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Configure application settings
                ApplicationConfiguration.Initialize();

                // Load key mappings from JSON file
                LoadKeyMappings();

                // Set up global keyboard hook
                hookID = SetHook(HookCallback);

                // Ensure the hook is removed when the application exits
                Application.ApplicationExit += (sender, args) => UnhookWindowsHookEx(hookID);

                // Run the main application form
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                LogError($"Application initialization error: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads key mappings from a JSON file.
        /// </summary>
        private static void LoadKeyMappings()
        {
            try
            {
                string filePath = "Resources/Keymap/keyMappings.json";
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Dictionary<string, string> mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    KeyReplacements.Clear();

                    foreach (var pair in mappings)
                    {
                        if (Enum.TryParse(pair.Key, out Keys key))
                        {
                            KeyReplacements[key] = pair.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error loading key mappings: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves key mappings to a JSON file.
        /// </summary>
        public static void SaveKeyMappings(Dictionary<Keys, string> mappings)
        {
            try
            {
                string filePath = "Resources/Keymap/keyMappings.json";
                var jsonMappings = new Dictionary<string, string>();

                foreach (var pair in mappings)
                {
                    jsonMappings[pair.Key.ToString()] = pair.Value;
                }

                File.WriteAllText(filePath, JsonSerializer.Serialize(jsonMappings, new JsonSerializerOptions { WriteIndented = true }));
                KeyReplacements = new Dictionary<Keys, string>(mappings);
            }
            catch (Exception ex)
            {
                LogError($"Error saving key mappings: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets up the low-level keyboard hook
        /// </summary>
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
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
                LogError($"Error setting up keyboard hook: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Callback for low-level keyboard events
        /// </summary>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
            {
                try
                {
                    KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                    Keys key = (Keys)hookStruct.vkCode;

                    // Log key press
                    LogKeyPress(key);

                    // Check if key has a replacement
                    if (KeyReplacements.TryGetValue(key, out string replacement))
                    {
                        if (wParam == (IntPtr)WM_KEYDOWN) // Process only key-down events
                        {
                            // Simulate replacement key press
                            SendKeys.SendWait(replacement);
                        }
                        return (IntPtr)1; // Suppress original key event
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error in keyboard hook callback: {ex.Message}");
                }
            }

            // Pass the event to the next hook in the chain
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// Logs key press events
        /// </summary>
        private static void LogKeyPress(Keys key)
        {
            try
            {
                // Use System.Diagnostics.Debug for development logging
                Debug.WriteLine($"Key Pressed: {key}");

                // Optional: Add file logging
                // System.IO.File.AppendAllText("keylog.txt", $"{DateTime.Now}: {key}\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logging error: {ex.Message}");
            }
        }

        /// <summary>
        /// Centralized error logging method
        /// </summary>
        private static void LogError(string errorMessage)
        {
            Debug.WriteLine(errorMessage);

            // Optional: Implement more robust logging
            // You could add file logging, event logging, or integration with a logging framework
        }

        // Windows API Imports
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

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
    }
}
