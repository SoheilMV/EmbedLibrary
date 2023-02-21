using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using EmbedLibrary.Core.Library;
using dnlib.Load;

namespace EmbedLibrary
{
    public partial class Form1 : Form
    {
        private string _file = string.Empty;
        private List<string> _librarys = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Title = "Choose your file";
                open.Filter = "All File(*.exe, *.dll)|*.exe;*.dll|Execute File(*.exe)|*.exe|Library File(*.dll)|*.dll";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    _file = open.FileName;
                    txtName.Text = Path.GetFileName(open.FileName);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Title = "Choose your file";
                open.Filter = "Library File(*.dll)|*.dll";
                open.Multiselect = true;
                if (open.ShowDialog() == DialogResult.OK)
                {
                    var files = open.FileNames;
                    foreach (var file in files)
                    {
                        _librarys.Add(file);
                        lbLibrarys.Items.Add(Path.GetFileName(file));
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _librarys.Clear();
            lbLibrarys.Items.Clear();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_file) && _librarys.Count > 0)
            {
                using (AssemblyLoader assembly = new AssemblyLoader(_file))
                {
                    ModuleContext2 context = assembly.GetContext2();
                    Embed.Execute(context, _librarys.ToArray());
                    string address = Address(_file);
                    assembly.Write(address);
                }
                if (Path.GetExtension(_file) == ".dll")
                {
                    string name = Path.GetFileName(_file).Replace(".dll", string.Empty);
                    string dir = Path.GetDirectoryName(_file);
                    if (File.Exists($"{dir}\\{name}.runtimeconfig.json"))
                    {
                        try
                        {
                            File.Copy($"{dir}\\{name}.exe", Address($"{dir}\\{name}.exe"));
                            File.Copy($"{dir}\\{name}.runtimeconfig.json", Address($"{dir}\\{name}.runtimeconfig.json"));
                        }
                        catch
                        {
                        }
                    }
                }
                MessageBox.Show("Completed!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Please select the file and required libraries!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void txtName_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array array = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (array != null)
                {
                    string path = array.GetValue(0).ToString();
                    int num = path.LastIndexOf(".");
                    if (num != -1)
                    {
                        string a = path.Substring(num).ToLower();
                        if (a == ".exe" || a == ".dll")
                        {
                            base.Activate();
                            _file = path;
                            txtName.Text = Path.GetFileName(path);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void txtName_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private void lbLibrarys_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array array = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (array != null)
                {
                    foreach (var file in array)
                    {
                        string path = file.ToString();
                        int num = path.LastIndexOf(".");
                        if (num != -1)
                        {
                            string a = path.Substring(num).ToLower();
                            if (a == ".dll")
                            {
                                base.Activate();
                                _librarys.Add(path);
                                lbLibrarys.Items.Add(Path.GetFileName(path));
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void lbLibrarys_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private string Address(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!dir.EndsWith("\\"))
                dir += "\\";
            if (!Directory.Exists($"{dir}Embedded"))
                Directory.CreateDirectory($"{dir}Embedded");
            string file = $"{dir}Embedded\\{Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path)}";
            if (File.Exists(file))
                File.Delete(file);
            return file;
        }
    }
}
