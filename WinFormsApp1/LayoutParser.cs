using System.Diagnostics;
using System.Text.Json;

namespace WinFormsApp1
{
    internal class LayoutParser
    {
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
        /// Loads key mappings from a JSON file.
        /// </summary>
        public static void LoadKeyMappings()
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
                Debug.WriteLine($"Error loading key mappings: {ex.Message}");
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
                Debug.WriteLine($"Error saving key mappings: {ex.Message}");
            }
        }
    }
}
