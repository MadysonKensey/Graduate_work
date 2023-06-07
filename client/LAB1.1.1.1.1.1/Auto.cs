using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary1;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace LAB1._1._1._1._1._1
{
    public partial class Auto : MetroForm
    {
        Class11 remote;
        public Auto()
        {
            InitializeComponent();
            HttpChannel ch = new HttpChannel(); // номер порта не нужен
            if (!Post.http)
            {
                // 2. Зарегистрировать канал
                ChannelServices.RegisterChannel(ch, false);
                Post.http = true;
            }
            remote = (Class11)Activator.GetObject(
                         typeof(Class11),
                          "http://DESKTOP-9E64BSA:5000/MathFunctions.soap"); 
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text; // создаётся переменная, куда вноситься введённое значение
            string pass = textBox2.Text; // создаётся переменная, куда вноситься введённое значение
            string b = "";
            bool f = remote.Autor(login, pass, out b, out Post.raz, out Post.post, out Post.id_operator);
            if (f)
            {
                if (Post.post == "Повар")
                {
                    Form MainKitchen = new MainKitchen();
                    this.Hide();
                    MainKitchen.Show();
                }
                else
                {
                    Form f1 = new Form1();
                    this.Hide();
                    f1.Show();
                }
            }
            else
            {
                MessageBox.Show("Логин или пароль введены неверно");//иначе показываем окно с ошибкой
            }
        }

        private void Auto_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
