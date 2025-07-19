namespace WinFormsApp1
{
    public partial class KeymapEditor : Form
    {
        private Dictionary<Keys, KeyEntry> keyMappings;

        public KeymapEditor()
        {
            InitializeComponent();
            InitializeGrid();
            LoadMappings();
        }

        /// <summary>
        /// Setup DataGridView columns for Key, Normal, and Shift
        /// </summary>
        private void InitializeGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Key", "Key");
            dataGridView1.Columns.Add("Normal", "Normal");
            dataGridView1.Columns.Add("Shift", "Shift");
        }

        private void LoadMappings()
        {
            keyMappings = new Dictionary<Keys, KeyEntry>(LayoutParser.KeyReplacements);
            dataGridView1.Rows.Clear();

            foreach (var pair in keyMappings)
            {
                dataGridView1.Rows.Add(pair.Key.ToString(), pair.Value.Normal, pair.Value.Shift);
            }
        }

        private void Save(object sender, EventArgs e)
        {
            Dictionary<Keys, KeyEntry> newMappings = new Dictionary<Keys, KeyEntry>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                var keyCell = row.Cells[0].Value;
                var normalCell = row.Cells[1].Value;
                var shiftCell = row.Cells[2].Value;

                if (keyCell != null && normalCell != null && shiftCell != null)
                {
                    if (Enum.TryParse(keyCell.ToString(), out Keys key))
                    {
                        newMappings[key] = new KeyEntry
                        {
                            vkCode = key.ToString(),
                            Normal = normalCell.ToString(),
                            Shift = shiftCell.ToString()
                        };
                    }
                }
            }

            LayoutParser.SaveKeyMappings(newMappings);
            MessageBox.Show("Key mappings saved!");
        }
    }
}
