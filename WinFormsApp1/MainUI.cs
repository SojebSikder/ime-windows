using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class MainUI : Form
    {
        public MainUI()
        {
            InitializeComponent();

            // Assign the ContextMenuStrip to the button
            menu_button.ContextMenuStrip = contextMenuStrip1;
        }

        private void menu_button_Click(object sender, EventArgs e)
        {
            menu_button.ContextMenuStrip.Show(menu_button, new Point(0, menu_button.Height));
        }

        private void keymapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeymapEditor keymapEditor = new KeymapEditor();
            keymapEditor.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainUI_Load(object sender, EventArgs e)
        {

        }
    }
}
