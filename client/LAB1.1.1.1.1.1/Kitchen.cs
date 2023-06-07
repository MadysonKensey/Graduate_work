using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary1;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using MetroFramework.Components;
using MetroFramework.Forms;
using Microsoft.Isam.Esent.Interop;

namespace LAB1._1._1._1._1._1
{
    public partial class Kitchen : MetroForm
    {
        int rows;
        int rows2;
        int index;
        bool s = false;
        bool b = true;
        Class11 remote;
        public Kitchen()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                         typeof(Class11),
                          "http://DESKTOP-9E64BSA:5000/MathFunctions.soap");
            DOrd();
            dataGridView1.CellContentClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
        }
       
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (!s)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    index = dataGridView1.CurrentRow.Index; //запоминаем выделенную строку
                    int id = int.Parse(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString()); //id выделенного заказа
                    bool f = remote.ViewPiz(id, out string[] piz, out int c);
                    if (f)
                    {
                        listBox1.Items.Clear();
                        listBox1.Items.Add("Товары в заказе id - " + id);
                        int i = 0;
                        while (i < c)
                        {
                            listBox1.Items.Add(piz[i]);
                            i++;
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0)
            {
                int id_p = int.Parse(listBox1.SelectedItem.ToString().Split(' ')[1]);    //получили ид выделеной пиццы
                bool f = remote.Ingr(id_p, out string ing);
                if (f)
                {
                    MessageBox.Show(ing);
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для просмотра ингридиентов");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0)
            {
                int id_p = int.Parse(listBox1.SelectedItem.ToString().Split(' ')[1]);
                bool f = remote.TMap(id_p, out string tmap);
                if (f)
                {
                    MessageBox.Show(tmap);
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для просмотра инструкций");
            }
        }
        private void DOrd()
        {
            DateTime date = new DateTime();
            DateTime date2 = new DateTime();
            s = true;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            bool f = remote.VieKit(out string[,] ord, out int cou);
            int i = 0;
            int g = 0;
            if (f)
            {
                dataGridView1.ColumnCount = ord.GetLength(1)+1;
                while (i < ord.GetLength(1))
                {
                    dataGridView1.Columns[g].Name = ord[0, i];
                    i++;
                    g++;
                }
                dataGridView1.Columns[g].Name = "Прошло";
                i = 1;
                while (i < ord.GetLength(0))
                {
                    dataGridView1.Rows.Add();
                    for (int j = 0; j < ord.GetLength(1); j++)
                    {
                        dataGridView1.Rows[i - 1].Cells[j].Value = ord[i, j];
                    }
                    date = DateTime.Parse(dataGridView1.Rows[i - 1].Cells[1].Value.ToString());
                    date2 = DateTime.Now;
                    TimeSpan diff = (date - date2).Duration();
                    dataGridView1.Rows[i - 1].Cells[5].Value = diff.TotalSeconds.ToString().Split(',')[0];
                    if (diff.TotalSeconds < 300)
                    {
                        dataGridView1.Rows[i - 1].Cells[5].Style.BackColor = System.Drawing.Color.Green;
                    }
                    if ((diff.TotalSeconds > 300) & (diff.TotalSeconds < 600))
                    {
                        dataGridView1.Rows[i - 1].Cells[5].Style.BackColor = System.Drawing.Color.Yellow;
                    }
                    if (diff.TotalSeconds > 600)
                    {
                        dataGridView1.Rows[i - 1].Cells[5].Style.BackColor = System.Drawing.Color.Red;
                    }
                    i++;
                }
            }
            
            DataGridViewButtonColumn dataGridViewButtonColumn = new DataGridViewButtonColumn();
            dataGridViewButtonColumn.HeaderText = "Изменить статус";
            dataGridViewButtonColumn.Text = "Внести изменения"; //текст кнопки
            dataGridViewButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(dataGridViewButtonColumn);
            dataGridView1.ClearSelection();
            dataGridView1.AllowUserToAddRows = false;
            rows2 = dataGridView1.Rows.Count;
            if (b)
            {
                rows = rows2;
                b = false;
            }
            if ((rows2 != 0)&(rows == rows2))
            {
                dataGridView1.ClearSelection();
            }
            if ((rows2 != 0)&(rows != rows2)) 
            {
                dataGridView1.ClearSelection();
                listBox1.Items.Clear();
                rows = rows2;
            }            
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            
            s = false;
        }
        void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) //нажатие кнопки (изменение статуса заказа)
        {
            DataGridView dgv = sender as DataGridView;
            
            if (dgv != null)
            {
                if (e.RowIndex >= 0)
                {
                    if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell d)
                    {
                        int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()); //id заказа
                        bool ff = remote.ChangeStatK(id);
                        if (ff)
                        {
                            rows = dataGridView1.Rows.Count;
                            DOrd();
                            MessageBox.Show("Статус изменён");
                        }
                        if (!ff)
                        {
                            MessageBox.Show("Заказ уже приготовлен");
                        }                
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DOrd();
        }

        private void Kitchen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f1 = new MainKitchen();
            this.Hide();
            f1.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DOrd();
        }
    }
}
