using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ClassLibrary1;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.CompilerServices;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Security.Cryptography;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class NewOrder : MetroForm
    {
        Class11 remote;
        int disс = 0;
        float amount = 0;
        float amount2 = 0;
        public NewOrder()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                       typeof(Class11),
                        "http://DESKTOP-9E64BSA:5000/MathFunctions.soap"); // localhost - временное имя нашего компьютера
            FillCliens();
            bool f = remote.FO2(Post.id_operator, out string n);
            if (f)
            {
                textBox2.Text = n;
            }
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns[0].HeaderText = "Пицца";
            dataGridView1.Columns[1].HeaderText = "Количество";
            dataGridView1.Columns[2].HeaderText = "Сумма";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            radioButton1.Checked = true;
            Products();
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            bool b = false;
            if (comboBox2.Text.Split(' ')[0] == "id")
            {
                string l = listBox1.SelectedItem.ToString().Split(' ')[1]; // берём знаки до ограничителя (это ид пиццы)
                int n = int.Parse(l); //узнаём id пиццы в 
                int i = 0;
                while (i != dataGridView1.RowCount)
                {
                    if (listBox1.SelectedItem == dataGridView1.Rows[i].Cells[0].Value)
                    {

                        dataGridView1.Rows[i].Cells[1].Value = int.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) + 1;
                        dataGridView1.Rows[i].Cells[2].Value = int.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) * float.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString().Split(' ')[6]);
                        b = true;
                        break;
                    }
                    i++;
                }
                if (!b)
                {
                    dataGridView1.Rows.Add(listBox1.SelectedItem, 1, listBox1.SelectedItem.ToString().Split(' ')[6]);
                }
                float pri;
                float pri2;
                string pr = listBox1.SelectedItem.ToString().Split(' ')[6];  // узнаём цену пиццы
                var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ",";
                if (disс != 0)
                {
                    pri = float.Parse(pr) - (float.Parse(pr) / 100 * disс);//вычитаем скидку из пиццы
                }
                else
                {
                    pri = float.Parse(pr); //не вычитаем скидку, если скидка 0
                }
                pri2 = float.Parse(pr);
                amount2 = pri2 + amount2;
                amount = pri + amount;
                decimal d = Math.Round(Convert.ToDecimal(amount), 2);
                decimal d2 = Math.Round(Convert.ToDecimal(amount2), 2);
                amount2 = (float)d2;
                amount = (float)d;
                label11.Text = amount.ToString() + " рублей"; //Добавляем данные
                label9.Text = amount2.ToString() + " рублей";
            }
            else
            {
                MessageBox.Show("Выбирете клиента");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.Rows.Count == 0 || String.IsNullOrEmpty(comboBox2.Text) || String.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                int countPi = dataGridView1.Rows.Count;
                int[,] masP = new int[countPi, 2];
                int n = 0;
                while (n < countPi)
                {
                    masP[n, 0] = int.Parse(dataGridView1.Rows[n].Cells[0].Value.ToString().Split(' ')[1]);
                    masP[n, 1] = int.Parse(dataGridView1.Rows[n].Cells[1].Value.ToString());
                    n++;
                }
                string stol = comboBox1.Text;
                string id_op = textBox2.Text.Split(' ')[1];
                string id_cl = comboBox2.Text.Split(' ')[1];
                string adress = textBox1.Text;
                string tel = maskedTextBox1.Text;
                double price = Math.Round(amount, 2);
                double price2 = Math.Round(amount2, 2);
                string dis = label7.Text.Split(' ')[1];
                bool f = remote.InsOrder(countPi, masP, id_op, id_cl, adress, price, tel, stol, price2, dis);
                if (f)
                {
                    dataGridView1.Rows.Clear();
                    amount = 0;
                    amount2 = 0;
                    label11.Text = amount.ToString();
                    label9.Text = amount2.ToString();
                    textBox1.Text = "";
                    comboBox2.Text = "";
                    label3.Text = "Телефон: ";
                    label7.Text = "Скидка: ";
                    maskedTextBox1.Text = "";
                    MessageBox.Show("Заказ оформлен");
                    if (comboBox1.Text=="На вынос")
                    {
                        MessageBox.Show("Необходимо оплатить заказ");
                        Form f1 = new Payment();
                        this.Hide();
                        f1.Show();
                    }
                }
            }
        }

        private void NewOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new Form1();
            this.Hide();
            f1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int a = dataGridView1.CurrentRow.Index; //узнали номер удаляемой строки
            string l = dataGridView1[0, a].Value.ToString().Split(' ')[6]; //достали цену пиццы
            string k = dataGridView1[1, a].Value.ToString(); //достали кол-во пицц
            string c = label11.Text.Split(' ')[0];
            string c2 = label9.Text.Split(' ')[0];
            decimal d = Math.Round(Convert.ToDecimal(float.Parse(l) - float.Parse(l) / 100 * disс), 2);
            decimal d2 = Math.Round(Convert.ToDecimal(float.Parse(l)), 2);
            amount2 = float.Parse(c2) - ((float)d2 * float.Parse(k));
            amount = float.Parse(c)-((float)d * float.Parse(k));
            d = Math.Round(Convert.ToDecimal(amount), 2);
            d2 = Math.Round(Convert.ToDecimal(amount2), 2);
            amount = (float)d;
            amount2 = (float)d2;
            label11.Text = amount.ToString() + " рублей";
            label9.Text = amount2.ToString() + " рублей";
            dataGridView1.Rows.Remove(dataGridView1.Rows[a]); //удалили строку
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string prod = "";
           // comboBox3.Items.Clear();
            if (radioButton1.Checked == true)
            {
                prod = "'тон', 'ср', 'тол'";
            }
            if (radioButton2.Checked == true)
            { // тут функция для добавления напитков
                prod = "'мл'";
            }
            int sel = comboBox3.SelectedIndex;
            string piz =comboBox3.SelectedItem.ToString(); 
            bool f = remote.ListPizza(prod, piz, sel, out string[] p, out int c);
            if (f)
            {
                int i = 0;
                while (i < c)
                {
                    listBox1.Items.Add(p[i]);
                    i++;
                }
            }
        }
        private void Products()
        {
            string prod = "";
            comboBox3.Items.Clear();
            if (radioButton1.Checked == true) //выбор пицц
            {
                label4.Text = "Пиццы";
                prod = "'тон', 'ср', 'тол'";
            }
            if (radioButton2.Checked == true) //выбор напитков
            {
                label4.Text = "Напитки";
                prod = "'мл'";
            }
            comboBox3.Items.Add("Все");
            bool f = remote.Pizza(prod, out string[] p, out int c);
            if (f)
            {
                int i = 0;
                while (i < c)
                {
                    comboBox3.Items.Add(p[i]);
                    i++;
                }
            }
            comboBox3.SelectedIndex = 0;
        }
        private void FillCliens ()
        {
            comboBox2.Items.Clear();
            string cm2 = comboBox2.Text;
            string tel = maskedTextBox1.Text;
            bool f = remote.FC(cm2, tel, out string[] cl, out int c);
            if (f)
            {
                int i = 0;
                while (i < c)
                {
                    comboBox2.Items.Add(cl[i]);
                    i++;
                }
            }
            comboBox2.Focus();
            comboBox2.SelectionStart = comboBox2.Text.Length;
        }
        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text.Split(' ')[0] != "id")
            {
                FillCliens();
            }
            else
            {
                int id_c = int.Parse(comboBox2.Text.Split(' ')[1]);
                bool f = remote.FCTelDis(id_c, out string tel, out string dis);
                if (f)
                {
                    disс = int.Parse(dis);
                    maskedTextBox1.Text = tel;
                    label7.Text = "Скидка: " + dis + " %";
                }
            }
        }
        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            FillCliens2();
        }
        private void FillCliens2()
        {
            comboBox2.Items.Clear();
            string cm2 = comboBox2.Text;
            string tel = maskedTextBox1.Text;
            bool f = remote.FC(cm2, tel, out string[] cl, out int c);
            if (f)
            {
                int i = 0;
                while (i < c)
                {
                    comboBox2.Items.Add(cl[i]);
                    i++;
                }
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            Products();
        }
    }
}
