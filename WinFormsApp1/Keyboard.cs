using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

namespace WinFormsApp1
{
    internal class Keyboard
    {

        public static string ExecutePhonetic(string en)
        {
            // Path to your executable file
            string exePath = "Resources/phonetic.exe";
            // Wrap the argument in quotes to handle spaces
            string arguments = $"convert \"{en}\"";

            // Setup the process
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    return output;
                }

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public static int[] vkCodes = new int[]
        //        {
        //            65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
        //            75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
        //            85, 86, 87, 88, 89, 90
        //        };

        public static bool ProcessCmdKey(Keys keyData)
        {
            int keyCode = (int)keyData;
            //if (vkCodes[keyCode])
            //{
            //    return true;
            //}

            if (vkCodes.TryGetValue(keyCode, out string keyName))
            {
                return true;
            }

            return false;
        }

        //    public static readonly HashSet<int> vkCodes = new HashSet<int>
        //{
        //    65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
        //    75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
        //    85, 86, 87, 88, 89, 90
        //};
        public static readonly Dictionary<int, string> vkCodes = new Dictionary<int, string>
        {
            { 65, "a" },
            { 66, "b" },
            { 67, "c" },
            { 68, "d" },
            { 69, "e" },
            { 70, "f" },
            { 71, "g" },
            { 72, "h" },
            { 73, "i" },
            { 74, "j" },
            { 75, "k" },
            { 76, "l" },
            { 77, "m" },
            { 78, "n" },
            { 79, "o" },
            { 80, "p" },
            { 81, "q" },
            { 82, "r" },
            { 83, "s" },
            { 84, "t" },
            { 85, "u" },
            { 86, "v" },
            { 87, "w" },
            { 88, "x" },
            { 89, "y" },
            { 90, "z" }
        };

        public static char? KeyToPrintableChar(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z)
            {
                bool shift = (Control.ModifierKeys & Keys.Shift) != 0;
                char baseChar = (char)key;
                return shift ? baseChar : char.ToLower(baseChar);
            }
            return null;
        }

        public static bool IsModifierKey(Keys key)
        {
            return key == Keys.ShiftKey || key == Keys.ControlKey ||
                   key == Keys.Menu || key == Keys.Capital || key == Keys.LWin ||
                   key == Keys.RWin || key == Keys.Tab || key == Keys.Escape;
        }

        public static string KeyToChar(Keys key)
        {
            bool shift = Control.ModifierKeys.HasFlag(Keys.Shift);
            bool capsLock = Control.IsKeyLocked(Keys.CapsLock);
            bool isLetter = key >= Keys.A && key <= Keys.Z;

            if (isLetter)
            {
                char c = (char)key;
                return (shift ^ capsLock) ? c.ToString().ToUpper() : c.ToString().ToLower();
            }

            // Handle digits and special characters (optional)
            if (key >= Keys.D0 && key <= Keys.D9)
            {
                return shift ? GetShiftedNumber(key) : ((char)('0' + (key - Keys.D0))).ToString();
            }

            return string.Empty;
        }

        public static string GetShiftedNumber(Keys key)
        {
            return key switch
            {
                Keys.D1 => "!",
                Keys.D2 => "@",
                Keys.D3 => "#",
                Keys.D4 => "$",
                Keys.D5 => "%",
                Keys.D6 => "^",
                Keys.D7 => "&",
                Keys.D8 => "*",
                Keys.D9 => "(",
                Keys.D0 => ")",
                _ => string.Empty,
            };
        }

        //public static char? KeyToPrintableChar(Keys key)
        //{
        //    if (key >= Keys.A && key <= Keys.Z)
        //        return (char)key;

        //    if (key >= Keys.D0 && key <= Keys.D9)
        //        return (char)('0' + (key - Keys.D0));

        //    if (key == Keys.Space)
        //        return ' ';

        //    return null;
        //}
    }
}
