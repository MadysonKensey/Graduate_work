using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class Stat : MetroForm
    {
        Class11 remote;
        public Stat()
        {
            InitializeComponent();
            comboBox1.Items.Add("Операторы");
            comboBox1.Items.Add("Клиенты");
            comboBox1.Items.Add("Товары");
            remote = (Class11)Activator.GetObject(
                        typeof(Class11),
                         "http://DESKTOP-9E64BSA:5000/MathFunctions.soap");
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
        }

        private void Stat_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new Form1();
            this.Hide();
            f1.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            string s = comboBox1.Text;
            bool f = remote.Statistic(s, out string[,] stat);
            int i = 0;
            int g = 0;
            if (f)
            {
                dataGridView1.ColumnCount = stat.GetLength(1);
                while (i < stat.GetLength(1))
                {
                    dataGridView1.Columns[g].Name = stat[0, i];
                    i++;
                    g++;
                }
                i = 1;
                while (i < stat.GetLength(0))
                {
                    dataGridView1.Rows.Add();
                    for (int j = 0; j < stat.GetLength(1); j++)
                    {
                        dataGridView1.Rows[i - 1].Cells[j].Value = stat[i, j];
                    }
                    i++;
                }
            }
        }
    }
}
