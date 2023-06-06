using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class MainKitchen : MetroForm
    {
        public MainKitchen()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form kitchen = new Kitchen();
            this.Hide();
            kitchen.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form AddProducts = new AddProducts();
            this.Hide();
            AddProducts.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form Auto = new Auto();
            this.Hide();
            Auto.Show();
        }

        private void MainKitchen_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
