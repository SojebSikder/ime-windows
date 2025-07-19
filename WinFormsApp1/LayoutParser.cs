using System.Diagnostics;
using System.Text.Json;

namespace WinFormsApp1
{
    public class KeyEntry
    {
        public required string vkCode { get; set; }
        public required string Normal { get; set; }
        public required string Shift { get; set; }
    }

    public class KeyboardLayout
    {
        public required List<KeyEntry> keys { get; set; }
    }

    internal class LayoutParser
    {
        public static Dictionary<Keys, KeyEntry> KeyReplacements = new Dictionary<Keys, KeyEntry>();

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
                    KeyboardLayout layout = JsonSerializer.Deserialize<KeyboardLayout>(json);
                    if (layout == null || layout.keys == null)
                    {
                        Debug.WriteLine(json);
                        Debug.WriteLine("Invalid layout data.");
                        return;
                    }
                    KeyReplacements.Clear();
                    Debug.WriteLine("layout data",layout);
                    if (layout != null)
                    {
                        foreach (var entry in layout.keys)
                        {
                            if (Enum.TryParse(entry.vkCode, out Keys key))
                            {
                                KeyReplacements[key] = new KeyEntry
                                {
                                    vkCode = entry.vkCode,
                                    Normal = entry.Normal,
                                    Shift = entry.Shift
                                };
                            }
                        }
                    }


                    Debug.WriteLine("Key mappings loaded successfully.");
                }
                else
                {
                    Debug.WriteLine("Key mappings file not found.");
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
        public static void SaveKeyMappings(Dictionary<Keys, KeyEntry> mappings)
        {
            try
            {
                string filePath = "Resources/Keymap/keyMappings.json";

                var layout = new KeyboardLayout
                {
                    keys = new List<KeyEntry>()
                };

                foreach (var pair in mappings)
                {
                    layout.keys.Add(new KeyEntry
                    {
                        vkCode = pair.Key.ToString(),
                        Normal = pair.Value.Normal,
                        Shift = pair.Value.Shift
                    });
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(filePath, JsonSerializer.Serialize(layout, options));

                KeyReplacements = new Dictionary<Keys, KeyEntry>(mappings);

                Debug.WriteLine("Key mappings saved successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving key mappings: {ex.Message}");
            }
        }
    }
}
