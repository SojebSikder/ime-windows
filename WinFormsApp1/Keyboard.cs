namespace WinFormsApp1
{
    internal class Keyboard
    {
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

        public static char? KeyToPrintableChar(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z)
                return (char)key;

            if (key >= Keys.D0 && key <= Keys.D9)
                return (char)('0' + (key - Keys.D0));

            if (key == Keys.Space)
                return ' ';

            return null;
        }
    }
}
