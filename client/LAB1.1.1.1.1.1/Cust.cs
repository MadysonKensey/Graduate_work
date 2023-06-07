using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class Cust : MetroForm
    {
        Class11 remote;
        public Cust()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                         typeof(Class11),
                          "http://DESKTOP-9E64BSA:5000/MathFunctions.soap"); // localhost - временное имя нашего компьютера
            MessageBox.Show("Номер телефона необходим для скидки");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Введи обязательное поле (Имя)");
            }
            else
            {
                string f = textBox1.Text;
                string i = textBox3.Text;
                string o = textBox4.Text;
                string t = maskedTextBox1.Text;
                bool g = remote.Client(f, i, o, t);
                if (g)
                {
                    MessageBox.Show("Клиент добавлен");
                }
            }
        }
        private void Cust_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new Form1();
            this.Hide();
            f1.Show();
        }
    }
}
