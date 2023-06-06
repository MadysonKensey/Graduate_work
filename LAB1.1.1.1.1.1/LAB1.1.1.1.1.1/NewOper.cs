using ClassLibrary1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class NewOper : MetroForm
    {
        Class11 remote;
        public NewOper()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                        typeof(Class11),
                         "http://DESKTOP-9E64BSA:5000/MathFunctions.soap"); // localhost - временное имя нашего компьютера
            bool f = remote.Dolj(out string[] dol, out int c);
            if (f)
            {
                int i = 0;
                while (i<c)
                {
                    comboBox1.Items.Add(dol[i]);
                    i++;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || maskedTextBox2.Text == "" || maskedTextBox1.Text == "" || comboBox1.SelectedIndex == -1)
            { MessageBox.Show("Заполните все поля"); }
            else
            {
                string f = textBox1.Text;
                string i = textBox2.Text;
                string o = textBox3.Text;
                string t = maskedTextBox1.Text;
                string p = maskedTextBox2.Text;
                string d = comboBox1.Text;
                string log = textBox4.Text;
                string pas = textBox5.Text;
                string login = textBox4.Text;
                bool ff = remote.Operator(f, i, o, t, p, d, log, pas);
                if (!ff)
                {
                    MessageBox.Show("Данный логин занят");
                }
                if (ff)
                {
                    MessageBox.Show("Оператор добавлен");
                }
            }
        }

        private void NewOper_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new Form1();
            this.Hide();
            f1.Show();
        }
    }
}
