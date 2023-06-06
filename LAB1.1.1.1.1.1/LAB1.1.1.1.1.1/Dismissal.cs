using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class Dismissal : MetroForm
    {
        Class11 remote;
        public Dismissal()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                        typeof(Class11),
                         "http://DESKTOP-9E64BSA:5000/MathFunctions.soap");
            DO();
        }
        private void DO()
        {
            listBox1.Items.Clear();
            string cm1 = "";
            bool f = remote.FO(cm1, out string[] op, out int c);
            if (f)
            {
                int i = 0;
                while (i < c)
                {
                    listBox1.Items.Add(op[i]);
                    i++;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string id = listBox1.SelectedItem.ToString().Split(' ')[1];
                bool f = remote.Dismissal(id);
                if (f)
                {
                    MessageBox.Show("Сотрудник удалён");
                    DO();
                }
            }
            else
            {
                MessageBox.Show("Выбирете сотрудника");
            }
        }

        private void Dismissal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new Form1();
            this.Hide();
            f1.Show();
        }
    }
}
