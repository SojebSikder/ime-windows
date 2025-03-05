using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Dictionary<Keys, string> keyMappings;
        public Form1()
        {
            InitializeComponent();
            LoadMappings();
        }

        private void LoadMappings()
        {
            keyMappings = new Dictionary<Keys, string>(Program.KeyReplacements);
            dataGridView1.Rows.Clear();

            foreach (var pair in keyMappings)
            {
                dataGridView1.Rows.Add(pair.Key.ToString(), pair.Value);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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

            Program.SaveKeyMappings(newMappings);
            MessageBox.Show("Key mappings saved!");
        }
    }
}
