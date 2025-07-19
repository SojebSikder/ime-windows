using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace WinFormsApp1
{
    internal static class Program
    {
        private static IntPtr hookID = IntPtr.Zero;

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
                LayoutParser.LoadKeyMappings();

                NativeMethods.InitializeSendTimer();
                // Set up global keyboard hook
                hookID = NativeMethods.SetHook(NativeMethods.HookCallback);

                // Ensure the hook is removed when the application exits
                Application.ApplicationExit += (sender, args) => NativeMethods.UnhookWindowsHookEx(hookID);

                // Run the main application form
                Application.Run(new MainUI());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Application initialization error: {ex.Message}");
            }
        }
    }
}
