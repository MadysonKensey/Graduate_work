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
    public partial class AddProducts : MetroForm
    {
        string prod = "";
        Class11 remote;
        public AddProducts()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                         typeof(Class11),
                          "http://DESKTOP-9E64BSA:5000/MathFunctions.soap");
            dataGridView1.ColumnCount = 2;
            dataGridView2.ColumnCount = 1;
            dataGridView1.Columns[0].HeaderText = "Ингириенты";
            dataGridView1.Columns[1].HeaderText = "Количество (гр/мл)";
            dataGridView2.Columns[0].HeaderText = "Этапы производства";
            radioButton1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string n = textBox1.Text;
            string n2 = maskedTextBox1.Text;
            if (prod == "p")
            {
                comboBox2.Text = comboBox2.Text;
            }
            if (prod == "n")
            {
                comboBox2.SelectedIndex = 0;
            }
            if ((textBox1.Text == "") ^ (maskedTextBox1.Text == "") ^ (textBox3.Text == "") ^ (textBox4.Text == "") ^ (comboBox1.Text == "") ^ (comboBox2.Text == "") ^ (dataGridView1.Rows.Count > 1) ^ (dataGridView2.Rows.Count > 1))
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                string dough = "";
                string name = textBox1.Text;
                string size = comboBox1.Text;
                dough = comboBox2.Text;
                string price = maskedTextBox1.Text;
                string log = textBox3.Text;
                string pass = textBox4.Text;
                string[,] ing = new string[dataGridView1.Rows.Count - 1, 2]; // СДЕЛАТЬ ДВОЙНОЙ МАССИВ И ПЕРЕДВАТЬ ИНГРИДИЕНТ/КОЛИЧЕСТВО
                string[] tmap = new string[dataGridView2.Rows.Count - 1];
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++) //запоминаем ингридиенты
                {
                    ing[i,0] = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    ing[i, 1] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                }
                for (int i = 0; i < dataGridView2.Rows.Count - 1; i++) //запоминаем технологию
                {
                    tmap[i] = dataGridView2.Rows[i].Cells[0].Value.ToString();
                }
                bool f = remote.AddPr(name, size, dough, price, log, pass, ing, tmap, out int error);
                if (!f) 
                {
                    if (error == 1)
                    {
                        MessageBox.Show("Такой товар уже существует");
                    }
                    if (error == 2)
                    {
                        MessageBox.Show("Введите логин и пароль директора");
                    }
                }
                if (f)
                {
                    MessageBox.Show("Товар добавлен, добавьте другие размеры, тесто и цену");
                    textBox3.Text = "";
                    textBox4.Text = "";
                }
            }
        }

        private void AddProducts_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new MainKitchen();
            this.Hide();
            f1.Show();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            if (radioButton1.Checked == true) //если пицца
            {
                prod = "p";
                label2.Text = "Размер пиццы";
                comboBox1.Items.Add("30 см");
                comboBox1.Items.Add("45 см");
                comboBox1.Items.Add("60 см");
                comboBox2.Items.Add("тол");
                comboBox2.Items.Add("ср");
                comboBox2.Items.Add("тон");
                label3.Visible = true;
                comboBox2.Visible = true;
            }
            if (radioButton2.Checked == true) //если напиток
            {
                prod = "n";
                label2.Text = "Объём напитка";
                comboBox1.Items.Add("500 мл");
                comboBox1.Items.Add("750 мл");
                comboBox1.Items.Add("1000 мл");
                comboBox2.Items.Add("мл");
                comboBox2.Visible = false;
                label3.Visible = false;
            }
        }
    }
}
