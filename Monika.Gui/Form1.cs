using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Monika.Gui
{
    public partial class Form1 : Form
    {
        private string mapFile;
        public Form1()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Darkmet98/Monika");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Darkmet98/Monika/wiki");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog {Filter = @"Character map file (*.map)|*.map"};

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                mapFile = openFileDialog.FileName;
                label4.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new FolderBrowserDialog();

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            var result = Converter.ExportRpyFolder(openFileDialog.SelectedPath, mapFile);

            MessageBox.Show(result.Item1?"The po files is generated on the rpy folder.":result.Item2, result.Item1?"Success":"Failed",
                MessageBoxButtons.OK,
                result.Item1?MessageBoxIcon.Information:MessageBoxIcon.Error);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new FolderBrowserDialog();

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            var result = Converter.ImportRpyFolder(openFileDialog.SelectedPath);

            MessageBox.Show(result.Item1 ? "The new rpy files is generated on the po folder." : result.Item2, result.Item1 ? "Success" : "Failed",
                MessageBoxButtons.OK,
                result.Item1 ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }
    }
}
