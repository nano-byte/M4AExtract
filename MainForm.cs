/*
 * Copyright 2012-2014 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NanoByte.Common;
using NanoByte.Common.Cli;

namespace NanoByte.M4AExtract
{
    public partial class MainForm : Form
    {
        public MainForm(string[] paths)
        {
            InitializeComponent();

            AddFiles(paths);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (paths == null) return;

            AddFiles(paths);
        }

        private void AddFiles(string[] paths)
        {
            try
            {
                var files = ArgumentUtils.GetFiles(paths, defaultPattern: "*.mp4");
                listBoxFiles.Items.AddRange(files.Cast<object>().ToArray());
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonGo_Click(object sender, System.EventArgs e)
        {
            // Prevent user input while processing
            Enabled = false;

            foreach (FileInfo file in listBoxFiles.Items)
            {
                string oldName = file.FullName;
                string newName = oldName.Substring(0, oldName.Length - 4) + ".m4a";

                var process = Process.Start("MP4Box", "-add " + oldName.EscapeArgument() + "#audio -new " + newName.EscapeArgument());
                if (process != null) process.WaitForExit();
            }

            Close();
        }
    }
}