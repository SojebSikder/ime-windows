namespace WinFormsApp1
{
    public partial class KeymapEditor : Form
    {
        private Dictionary<Keys, string> keyMappings;
        public KeymapEditor()
        {
            InitializeComponent();
            LoadMappings();
        }

        private void LoadMappings()
        {
            keyMappings = new Dictionary<Keys, string>(LayoutParser.KeyReplacements);
            dataGridView1.Rows.Clear();

            foreach (var pair in keyMappings)
            {
                dataGridView1.Rows.Add(pair.Key.ToString(), pair.Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<Keys, string> newMappings = new Dictionary<Keys, string>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    if (Enum.TryParse(row.Cells[0].Value.ToString(), out Keys key))
                    {
                        newMappings[key] = row.Cells[1].Value.ToString();
                    }
                }
            }

            LayoutParser.SaveKeyMappings(newMappings);
            MessageBox.Show("Key mappings saved!");
        }
    }
}
