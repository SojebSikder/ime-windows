using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinFormsApp1
{
    internal static class Program
    {
        private static IntPtr hookID = IntPtr.Zero;

        // Dictionary to map key replacements
        private static readonly Dictionary<Keys, string> KeyReplacements = new Dictionary<Keys, string>
        {
            { Keys.A, "α" },
            { Keys.B, "β" },
            { Keys.C, "γ" }
            // Add more key mappings as needed
        };

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

                // Set up global keyboard hook
                hookID = SetHook(HookCallback);

                // Run the main application form
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                // Log any initialization errors
                LogError($"Application initialization error: {ex.Message}");
            }
            finally
            {
                // Ensure hook is always unhooked
                if (hookID != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(hookID);
                }
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
            if (nCode >= 0)
            {
                try
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    Keys key = (Keys)vkCode;

                    // Log key press
                    LogKeyPress(key);

                    // Check if key has a replacement
                    if (KeyReplacements.TryGetValue(key, out string replacement))
                    {
                        // Replace the key with custom character
                        SendKeys.SendWait(replacement);
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
                System.Diagnostics.Debug.WriteLine($"Key Pressed: {key}");

                // Optional: Add file logging
                // System.IO.File.AppendAllText("keylog.txt", $"{DateTime.Now}: {key}\n");
            }
            catch (Exception ex)
            {
                // Silent catch for logging errors
                System.Diagnostics.Debug.WriteLine($"Logging error: {ex.Message}");
            }
        }

        /// <summary>
        /// Centralized error logging method
        /// </summary>
        private static void LogError(string errorMessage)
        {
            // Use System.Diagnostics.Debug for development
            System.Diagnostics.Debug.WriteLine(errorMessage);

            // Optional: Implement more robust logging
            // You could add file logging, event logging, or integration with a logging framework
        }

        // Windows API Imports (Kept the same as original)
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_KEYBOARD_LL = 13;

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
    }
}