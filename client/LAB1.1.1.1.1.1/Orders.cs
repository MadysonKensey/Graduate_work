using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using ClassLibrary1;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class Orders : MetroForm
    {
        int index;
        bool b = true;
        Class11 remote;
        string st = "";
        public Orders()
        {
            InitializeComponent();
            remote = (Class11)Activator.GetObject(
                        typeof(Class11),
                         "http://DESKTOP-9E64BSA:5000/MathFunctions.soap");
            
            if (Post.raz[2]==false) //удаление заказа
            {
                button2.Visible = false;
            }
            bool f = remote.Stat(out string[] stat, out int c);
            if (f)
            {
                int k = 0;
                while (k < c)
                {
                    checkedListBox1.Items.Add(stat[k], true);
                    st = st +"'" +stat[k] + "', ";
                    k++;    
                }
            }
            int idx = st.LastIndexOf(',');
            st = st.Substring(0, idx);
            dataGridView1.CellContentClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            DOrder();
        }

        private void DOrder()
        {
            dataGridView1.Columns.Clear();
            bool f = remote.Orders(st, out string[,] ord, out int cou);
            ord.GetLength(0); //количество строк
            ord.GetLength(1); //количество столбцов

            int i = 0;
            int g = 0;
            if (f)
            {
                dataGridView1.ColumnCount = ord.GetLength(1);
                while (i < ord.GetLength(1))
                {
                    dataGridView1.Columns[g].Name = ord[0, i];
                    i++;
                    g++;
                }
                i = 1;
                while (i < ord.GetLength(0))
                {
                    dataGridView1.Rows.Add();
                    for (int j = 0; j < ord.GetLength(1); j++)
                    {
                        dataGridView1.Rows[i - 1].Cells[j].Value = ord[i, j];
                    }
                    i++;
                }
            }

            DataGridViewComboBoxColumn dgvCmb = new DataGridViewComboBoxColumn();
            f = remote.Stat(out string[] stat, out int c);
            i = 0;
            while (i < c)
            {
                dgvCmb.Items.Add(stat[i]);
                i++;
            }
            dataGridView1.Columns.Add(dgvCmb);
            dgvCmb.HeaderText = "Статус";
            for (i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewComboBoxCell comboBoxCell = (dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 1] as DataGridViewComboBoxCell);
                comboBoxCell.Value = dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 2].Value;
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; //размер ячеек
            DataGridViewButtonColumn dataGridViewButtonColumn = new DataGridViewButtonColumn();
            dataGridViewButtonColumn.HeaderText = "Изменить статус";
            dataGridViewButtonColumn.Text = "Внести изменения"; //текст кнопки
            dataGridViewButtonColumn.UseColumnTextForButtonValue = true;
            
            dataGridView1.Columns.Add(dataGridViewButtonColumn);
            DataGridViewButtonColumn dataGridViewButtonColumn2 = new DataGridViewButtonColumn();
            dataGridViewButtonColumn2.HeaderText = "Товары";
            dataGridViewButtonColumn2.Text = "Просмотреть товары"; //текст кнопки
            dataGridViewButtonColumn2.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(dataGridViewButtonColumn2);
            for (i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
            }

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Columns[8].ReadOnly = false;
            dataGridView1.Columns[7].Visible = false;
            if (Post.raz[2] == false) //удаление заказа
            {
                dataGridView1.Columns[7].Visible = true;
                dataGridView1.Columns[8].Visible = false;
                dataGridView1.Columns[9].Visible = false;
            }

        }
        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) //нажатие кнопки (изменение статуса заказа)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null)
            {
                if (e.RowIndex >= 0)
                {
                    if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell d)
                    {
                        //код для кнопки статуса
                        if (e.ColumnIndex == 9)
                        {

                            int i = e.RowIndex;
                            string stat = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString(); //выбранный статус
                            int id = int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()); //id заказа
                            bool f = remote.ChangeStat(stat, id);
                            if (f)
                            {
                                MessageBox.Show("Статус изменён");
                            }

                        }

                        if (e.ColumnIndex == 10)
                        {
                            int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()); //id заказа
                            bool f = remote.ViewingOrd(id, out string text);
                            if (f)
                            {
                                MessageBox.Show(text);
                            }
                        }
                    }
                }
            }
        }

        private void Orders_FormClosed(object sender, FormClosedEventArgs e)
        {
         
            Form f1 = new Form1();
            this.Hide();
            f1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            st = "";
            for (int a = 0; a < (checkedListBox1.Items.Count); a++)
            {
                if (checkedListBox1.GetItemChecked(a)==true)
                {
                    st = st + "'" + checkedListBox1.Items[a].ToString() + "', ";
                }
            }
            int idx = st.LastIndexOf(',');
            if (idx != -1)
            {
                st = st.Substring(0, idx);
                DOrder();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int id = int.Parse(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString());
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentCell.RowIndex);
            bool f = remote.DelOrd(id);
            if (f)
            {
                MessageBox.Show("Заказ удалён");
            }
        }
    }
}
