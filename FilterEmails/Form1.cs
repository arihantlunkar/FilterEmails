using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace FilterEmails
{
    public partial class Form1 : Form
    {
        private OpenFileDialog ofd = new OpenFileDialog();
        private FolderBrowserDialog fbd = new FolderBrowserDialog();

        public Form1()
        {
            InitializeComponent();

            //implement this
            checkBox3.Enabled = false;
            checkBox4.Enabled = false;
            checkBox5.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                ofd.Filter = "Text|*.txt";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = ofd.FileName;
                    InputData.FilteringOn = radioButton1.Text.ToString();
                    InputData.EmailLocation = ofd.FileName;
                }
            }
            else
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = fbd.SelectedPath;
                    InputData.FilteringOn = radioButton2.Text.ToString();
                    InputData.EmailLocation = fbd.SelectedPath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(InputData.EmailLocation))
            {
                MessageBox.Show("Email location cannot be left empty.", "Email Filter Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                button3.Enabled = false;
                button4.Enabled = true;
                button5.Enabled = false;

                progressBar1.Value = 1;
                label4.Text = "No task started.";

                InputData.RemoveDuplicates = checkBox1.Checked;
                InputData.KeepAtLeastDomain = checkBox2.Checked;
                InputData.NoOfDomainsToKeep = Convert.ToUInt16(Math.Round(numericUpDown1.Value, 0));

                FilterEmailsController.Instance.UpdateStatusProgressBar += new FilterEmails.StatusProgressBarHandler(Instance_UpdateStatusProgressBar);
                FilterEmailsController.Instance.execute();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FilterEmailsController.Instance.abort();

            label4.Text = "Task Aborted";
            button3.Enabled = true;
            button4.Enabled = false;
            button5.Enabled = true;
        }


        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Visible = checkBox3.Checked ? true : false;
            button2.Visible = checkBox3.Checked ? true : false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.Visible = checkBox4.Checked ? true : false;
            button7.Visible = checkBox4.Checked ? true : false;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Visible = checkBox5.Checked ? true : false;
            button6.Visible = checkBox5.Checked ? true : false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            InputData.EmailLocation = String.Empty;
            textBox1.Text = String.Empty;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            InputData.EmailLocation = String.Empty;
            textBox1.Text = String.Empty;
        }

        private void Instance_UpdateStatusProgressBar(String status, UInt16 progressValue, Boolean isFilterButtonVisible, Boolean isCancelButtonVisible, Boolean isStatisticsButtonVisible)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                progressValue = (progressValue > (UInt16) 100) ? (UInt16) 100 : progressValue;
                progressBar1.Value = progressValue;
                label4.Text = status;
                button3.Enabled = isFilterButtonVisible;
                button4.Enabled = isCancelButtonVisible;
                button5.Enabled = false; //  isStatisticsButtonVisible;
            });
        }
    }
}
