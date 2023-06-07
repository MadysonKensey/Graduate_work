using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class Payment : MetroForm
    {
        Class11 remote;
        public Payment()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                        typeof(Class11),
                         "http://DESKTOP-9E64BSA:5000/MathFunctions.soap");
            bool f = remote.Payment(out string[] orders, out int c);
            int s = 0;
            if (f)
            {
                while (s < c)
                {
                    listBox1.Items.Add(orders[s]);
                    s++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id_ord = int.Parse(listBox1.SelectedItem.ToString().Split(' ')[1]);
            bool f = remote.Pay(id_ord);
            if (f)
            {
                bool ff = remote.PayOrd(id_ord);
                if (ff)
                {
                    MessageBox.Show("Чек создан");
                    System.Diagnostics.Process txt = new System.Diagnostics.Process();
                    txt.StartInfo.FileName = "notepad.exe";
                    txt.StartInfo.Arguments = $@"C:\Users\madke\Desktop\ДИПЛОМ\Чеки\{id_ord}.txt";
                    txt.Start();
                }
                if (!ff)
                {
                    MessageBox.Show("Заказ ещё не приготовлен");
                }
            }
        }
        private void Payment_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new Form1();
            this.Hide();
            f1.Show();
        }
    }
}
