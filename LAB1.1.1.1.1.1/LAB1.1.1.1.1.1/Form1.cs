using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
            if (Post.raz[1] == false)
            {
                button4.Visible = false;
                button8.Visible = false;    
            }
            if (Post.raz[3] == false)
            {
                button6.Visible = false;
            }
            if (Post.raz[0] == false)
            {
                button1.Visible = false;
                button3.Visible = false;
            }
            if (Post.raz[5] == false) 
            {
                button7.Visible = false;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form NewOper = new NewOper();
            this.Hide();
            NewOper.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form NewOrder = new NewOrder();
            this.Hide();
            NewOrder.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form Cust = new Cust();
            this.Hide();
            Cust.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form Orders = new Orders();
            this.Hide();
            Orders.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form Auto = new Auto();
            this.Hide();
            Auto.Show();
        }

       

        private void button7_Click(object sender, EventArgs e)
        {
            Form pay = new Payment();
            this.Hide();
            pay.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form stat = new Stat();
            this.Hide();
            stat.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form Dismissal = new Dismissal();
            this.Hide();
            Dismissal.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
