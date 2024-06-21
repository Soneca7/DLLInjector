using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Injector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Select File",
                InitialDirectory = @"C:\",
                Filter = "All files (*.*)|*.*|Text File (*.dll)|*.dll",
                FilterIndex = 2
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                InputPath.Text = openFileDialog1.FileName;
            }
            else
            {
                InputPath.Text = "You didn't select the file!";
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string processIdentifier = guna2CustomRadioButton1.Checked ? InputProcessName.Text : InputProcessID.Text;
            string dllPath = InputPath.Text;
            bool success = false;

            if (guna2CustomRadioButton1.Checked)
            {
                success = DllInjector.InjectDll(processIdentifier, dllPath, DllInjector.InjectionMethod.ByProcessName);
            }
            else if (guna2CustomRadioButton2.Checked)
            {
                success = DllInjector.InjectDll(processIdentifier, dllPath, DllInjector.InjectionMethod.ByPID);
            }

            if (success)
            {
                MessageBox.Show("DLL injetada com sucesso!");
            }
            else
            {
                MessageBox.Show("Falha ao injetar DLL.");
            }

        }
    }
}
