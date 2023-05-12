using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace File_Organiser
{
    public partial class Form1 : Form
    {
        public Thread thread;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (thread == null || !thread.IsAlive)
            {
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                folderBrowserDialog1.ShowDialog();
                string originPath = folderBrowserDialog1.SelectedPath;

                if (!string.IsNullOrEmpty(originPath))
                {
                    textBox1.Text = originPath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (thread == null || !thread.IsAlive)
            {
                FolderBrowserDialog folderBrowserDialog2 = new FolderBrowserDialog();
                folderBrowserDialog2.ShowDialog();
                string savePath = folderBrowserDialog2.SelectedPath;

                if (!string.IsNullOrEmpty(savePath))
                {
                    textBox2.Text = savePath;
                }
            }
        }

        public void startCopying(bool includeSubfolders)
        {
            string[] files = { };
            
            if (includeSubfolders)
            {
                files = Directory.GetFiles(textBox1.Text, "*", SearchOption.AllDirectories);
            }
            else
            {
                files = Directory.GetFiles(textBox1.Text);
            }

            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Clear()));
            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Maximum = files.Length));
            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Step = 1));

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                fi.Refresh();
                int year = fi.CreationTimeUtc.Year;
                int month = fi.CreationTimeUtc.Month;
                string destination = textBox2.Text + "\\" + year;
                Directory.CreateDirectory(destination);
                Directory.CreateDirectory(destination + "\\" + month);
                destination += "\\" + month + "\\" + Path.GetFileName(file);

                if (!File.Exists(destination))
                {
                    File.Copy(file, destination);
                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(file)));
                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(destination)));
                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add("")));
                    listBox1.Invoke((MethodInvoker)(() => listBox1.TopIndex = listBox1.Items.Count - 3));
                }
                else
                {
                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(file)));
                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add("FILE ALREADY EXISTS.")));
                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add("")));
                    listBox1.Invoke((MethodInvoker)(() => listBox1.TopIndex = listBox1.Items.Count - 3));
                }

                progressBar1.Invoke((MethodInvoker)(() => progressBar1.PerformStep()));
            }

            label3.Invoke((MethodInvoker)(() => label3.Text = "Files copied"));
            checkBox1.Invoke((MethodInvoker)(() => checkBox1.Enabled = true));
            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
        }

        public void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0 && textBox2.Text.Length != 0)
            {
                if (thread == null || !thread.IsAlive)
                {
                    if (Directory.Exists(textBox1.Text) && (Directory.Exists(textBox2.Text)))
                    {
                        listBox1.Items.Clear();
                        checkBox1.Enabled = false;
                        progressBar1.Value = 0;
                        thread = new Thread(() => startCopying(checkBox1.Checked));
                        thread.Start();
                        label3.Text = "Copying files";
                    }
                }
            }
        }

        private void reset()
        {
            if (thread == null || !thread.IsAlive)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                label3.Text = "No files copied";
                listBox1.Items.Clear();
                progressBar1.Value = 0;
                thread = null;
                checkBox1.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (thread != null && thread.IsAlive)
            {
                label3.Text = "Canceld";
                thread.Abort();
                checkBox1.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Files Location Input- Enter the location of the files you would like to move.\n\nSave Location Input: Enter the location you would like the files to be organised in.\n\nOrganise Files Button- Start the organising process.\n\nCancel Button- Cancel the running organsing process.\n\nClear Button- Reset all the entries to blank.\n\nInclude Subdirectories Checkbox- Also move the files contained in subfolders to the one you selected.", "Help");
        }
    }
}
