namespace WinFormsApp1
{
    partial class MainUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menu_button = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            keymapEditorToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menu_button
            // 
            menu_button.Location = new Point(3, 3);
            menu_button.Name = "menu_button";
            menu_button.Size = new Size(75, 23);
            menu_button.TabIndex = 0;
            menu_button.Text = "Menu";
            menu_button.UseVisualStyleBackColor = true;
            menu_button.Click += menu_button_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { keymapEditorToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(152, 48);
            // 
            // keymapEditorToolStripMenuItem
            // 
            keymapEditorToolStripMenuItem.Name = "keymapEditorToolStripMenuItem";
            keymapEditorToolStripMenuItem.Size = new Size(151, 22);
            keymapEditorToolStripMenuItem.Text = "Keymap Editor";
            keymapEditorToolStripMenuItem.Click += keymapEditorToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(151, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // MainUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(277, 29);
            Controls.Add(menu_button);
            Name = "MainUI";
            Text = "MainUI";
            TopMost = true;
            Load += MainUI_Load;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button menu_button;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem keymapEditorToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
    }
}