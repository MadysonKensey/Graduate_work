using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ClassLibrary1
{
    public class Class11 : MarshalByRefObject
    {
        bool block = true;
        SqlConnection conn = DBUtils.GetDBConnection(); // создаём подключение
        
        public bool Autor(string login, string pass, out string b, out bool[] raz1, out string post1, out int id) //форма авторизации
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            b = "";
            conn.Open();
            string qq = "SELECT count(*) FROM information_schema.columns WHERE table_name = 'Post'";
            SqlCommand sql = new SqlCommand(qq, conn);
            Int32 count = (Int32)sql.ExecuteScalar()-2;
            raz1=new bool[count];
            id = 0;
            post1 = "";
            string q = $"select * from oper join Post on oper.[Должность id]=Post.id where Логин = '{login}' and Пароль = '{pass}' and Статус = 1";
            sql = new SqlCommand(q, conn);
            using (DbDataReader reader = sql.ExecuteReader())
            {
                if (reader.HasRows) // если имеется одна строка
                {
                    while (reader.Read())
                    {
                        post1 = reader.GetString(6);
                        id = reader.GetInt32(0);
                        for (int i = 0; i < count; i++)
                        {
                            raz1[i] = reader.GetBoolean(7 + i);
                        }
                        conn.Close();
                        block = true;
                        Console.WriteLine("Unblocked");
                        return true;  
                    }
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return false;
        }
        public bool Client(string f, string i, string o, string t) //форма добавление клиента
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string sql = "Insert into Person (Фамилия, Имя, Отчество) " + " values (@f, @i, @o) ";
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add("@f", SqlDbType.VarChar).Value = f;
            cmd.Parameters.Add("@i", SqlDbType.VarChar).Value = i;
            cmd.Parameters.Add("@o", SqlDbType.VarChar).Value = o;
            int rowCount = cmd.ExecuteNonQuery();

            SqlCommand sqlCommand = new SqlCommand("select max(id) from Person", conn);
            string idm = sqlCommand.ExecuteScalar().ToString();
            sql = "Insert into Client (id, Телефон, Скидка) " + " values (@id, @tel, @s) ";
            cmd.CommandText = sql;
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idm;
            cmd.Parameters.Add("@tel", SqlDbType.VarChar).Value = t;
            cmd.Parameters.Add("@s", SqlDbType.Int).Value = 0;
            rowCount = cmd.ExecuteNonQuery();
            conn.Close();
            block=true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Dolj(out string[] dol, out int c) //комбо боксв с должностями
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string sql = $"SELECT count(*) FROM Post";
            SqlCommand q = new SqlCommand(sql, conn);
            Int32 count = (Int32)q.ExecuteScalar();
            c = count;
            dol = new string[count];
            int i = 0;
            sql = $"select DISTINCT Должность from Post";
            SqlCommand sqlCommand1 = new SqlCommand(sql, conn);
            using (DbDataReader reader = sqlCommand1.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dol[i]=reader.GetString(0).ToString();
                        i++;
                    }
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Operator(string f, string i, string o, string t, string p, string d, string log, string pas) //запись оператора
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string sql = $"select id from Post where Должность = '{d}'";
            SqlCommand sqlCommand1 = new SqlCommand(sql, conn);
            int id_d = new int();
            using (DbDataReader reader = sqlCommand1.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        id_d = reader.GetInt32(0); //id выбранной должности
                    }
                }
            }
            sql = $"Select * from Oper where Логин = '{log}' and Статус = 1";
            SqlCommand sqlCommand = new SqlCommand(sql, conn);
            bool l = false;
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    l = true;
                    conn.Close();
                    block = true;
                    Console.WriteLine("Unlocked");
                    return false;
                }
            }
            if(!l)
            {
                sql = "Insert into Person (Фамилия, Имя, Отчество, [Номер паспорта]) " + " values (@f, @i, @o, @pas) ";
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@f", SqlDbType.VarChar).Value = f;
                cmd.Parameters.Add("@i", SqlDbType.VarChar).Value = i;
                cmd.Parameters.Add("@o", SqlDbType.VarChar).Value = o;
                cmd.Parameters.Add("@pas", SqlDbType.VarChar).Value = p;
                //записываем
                int rowCount = cmd.ExecuteNonQuery();
                //узнраём ид
                sqlCommand = new SqlCommand("select max(id) from Person", conn);
                string idm = sqlCommand.ExecuteScalar().ToString();
                //заполняем клиента
                sql = "Insert into Client (id, Телефон, Скидка) " + " values (@id, @tel, @s) ";
                cmd.CommandText = sql;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idm;
                cmd.Parameters.Add("@tel", SqlDbType.VarChar).Value = t;
                cmd.Parameters.Add("@s", SqlDbType.Int).Value = 0;
                rowCount = cmd.ExecuteNonQuery();
                //заполняем Oper
                sql = "Insert into Oper (id, Логин, Пароль, [Должность id], Статус) " + " values (@idp, @log, @pass, @post, @status) ";
                cmd.CommandText = sql;
                cmd.Parameters.Add("@idp", SqlDbType.Int).Value = idm;
                cmd.Parameters.Add("@log", SqlDbType.VarChar).Value = log;
                cmd.Parameters.Add("@pass", SqlDbType.VarChar).Value = pas;
                cmd.Parameters.Add("@post", SqlDbType.Int).Value = id_d;
                cmd.Parameters.Add("@status", SqlDbType.Bit).Value = 1;
                rowCount = cmd.ExecuteNonQuery();
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool FC(string cm2, string tel, out string[] cl, out int c) //комбо боксв клиентами
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            cl = new string[1];
            c = 0;
            string sql;
            SqlCommand q;
            Int32 count;
            string query = "";
            if (tel == "+7-(   )-   -  -")
            {
                sql = $"SELECT count(*) FROM Client join Person on Client.id=Person.id where LOWER (Фамилия) like '%{cm2.ToLower()}%'";
                q = new SqlCommand(sql, conn);
                count = (Int32)q.ExecuteScalar();
                c = count;
                cl = new string[count];
                query = $"SELECT * FROM Client join Person on Client.id=Person.id where LOWER (Фамилия) like '%{cm2.ToLower()}%'";
            }
            if (tel != "+7-(   )-   -  -")
            {
                sql = $"SELECT count(*) FROM Client join Person on Client.id=Person.id where Телефон = '{tel}'";
                q = new SqlCommand(sql, conn);
                count = (Int32)q.ExecuteScalar();
                c = count;
                cl = new string[count];
                query = $"SELECT * FROM Client join Person on Client.id=Person.id where Телефон = '{tel}'";
            }
            int i = 0;
            //создаём объект
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read()) //Читаем пока есть данные
                { //добавляем клиентов в combobox
                    cl[i] = ("id " + reader.GetValue(0).ToString() + " : " + reader.GetValue(4).ToString() + " " + reader.GetValue(5).ToString() + " " + reader.GetValue(6).ToString());
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool FO(string cm1, out string[] op, out int c) //комбо боксв клиентами
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string sql = $"SELECT count(*) FROM Oper join Person on Oper.id=Person.id where LOWER (Фамилия) like '%{cm1.ToLower()}%' and Статус = 1";
            SqlCommand q = new SqlCommand(sql, conn);
            Int32 count = (Int32)q.ExecuteScalar();
            c = count;
            op = new string[count];
            sql = $"SELECT * FROM Oper join Person on Oper.id=Person.id where LOWER (Фамилия) like '%{cm1.ToLower()}%' and Статус = 1";
            int i = 0;
            q = new SqlCommand(sql, conn);
            using (DbDataReader reader = q.ExecuteReader())
            {
                while (reader.Read()) //Читаем пока есть данные
                { //добавляем операторов в combobox
                    op[i] = ("id " + reader.GetValue(5).ToString() + " : " + reader.GetValue(6).ToString() + " " + reader.GetValue(7).ToString() + " " + reader.GetValue(8).ToString()); //Добавляем данные в лист итем
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool FO2(int id, out string n) //комбо боксв клиентами
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string sql = $"SELECT concat('id ', Oper.id, ' : ', Person.Фамилия, ' ', Person.Имя, ' ', Person.Отчество) FROM Oper join Person on Oper.id=Person.id where Oper.id = {id}";
            int i = 0;
            n = "";
            SqlCommand q = new SqlCommand(sql, conn);
            using (DbDataReader reader = q.ExecuteReader())
            {
                while (reader.Read()) //Читаем пока есть данные
                { //добавляем операторов в combobox
                    n = reader.GetValue(0).ToString();
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Pizza(string prod, out string[] p, out int c) //комбо боксв с пиццами
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string sql = $"select count(distinct Наименование) from Pizza where Тесто in ({prod})";
            SqlCommand q = new SqlCommand(sql, conn);
            Int32 count = (Int32)q.ExecuteScalar();
            c = count;
            p = new string[count];
            int i = 0;
            //sql = $"select DISTINCT Должность from Post";
            sql = $"select distinct Наименование from Pizza where Тесто in ({prod})";
            q = new SqlCommand(sql, conn);
            using (DbDataReader reader = q.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        p[i] = reader.GetString(0).ToString();
                        i++;
                    }
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }

        public bool ListPizza(string prod, string piz, int sel, out string[] p, out int c) //лист с пиццами
        {
            c = 0;
            p = new string[1];
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            int i = 0;
            if ((sel == 0) & (prod == "'тон', 'ср', 'тол'"))
            {
                string query = $"SELECT count(*) FROM Pizza where Тесто in ({prod})";
                SqlCommand que = new SqlCommand(query, conn);
                Int32 coun = (Int32)que.ExecuteScalar();
                p = new string[coun];
                c = coun;
                query = $"SELECT id, Наименование, [Диаметр/Объём(мл)], Тесто, Цена FROM Pizza where Тесто in ({prod})";
                que = new SqlCommand(query, conn);
                using (DbDataReader reader = que.ExecuteReader())
                {
                    while (reader.Read()) //Читаем пока есть данные
                    {
                        p[i] = ("id " + reader.GetValue(0).ToString() + " : " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString() + " " + reader.GetValue(3).ToString() + " " + reader.GetValue(4).ToString());
                        i++;
                    }
                }
            }
            if ((sel == 0) & (prod == "'мл'"))
            {
                string query = $"SELECT count(*) FROM Pizza where Тесто in ({prod})";
                SqlCommand que = new SqlCommand(query, conn);
                Int32 coun = (Int32)que.ExecuteScalar();
                p = new string[coun];
                c = coun;
                query = $"SELECT id, Наименование, [Диаметр/Объём(мл)], Цена FROM Pizza where Тесто in ({prod})";
                que = new SqlCommand(query, conn);
                using (DbDataReader reader = que.ExecuteReader())
                {
                    while (reader.Read()) //Читаем пока есть данные
                    {
                        p[i] = ("id " + reader.GetValue(0).ToString() + " : " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString() +" мл"+ " " + reader.GetValue(3).ToString());
                        i++;
                    }
                }
            }
            if ((sel != 0) & (prod == "'тон', 'ср', 'тол'"))
            {
                string query = $"SELECT count(*) FROM Pizza where Наименование = '{piz}' and Тесто in ({prod})";
                SqlCommand que = new SqlCommand(query, conn);
                Int32 coun = (Int32)que.ExecuteScalar();
                p = new string[coun];
                c = coun;
                query = $"SELECT id, Наименование, [Диаметр/Объём(мл)], Тесто, Цена FROM Pizza where Наименование = '{piz}' and Тесто in ({prod})";
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                using (DbDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read()) //Читаем пока есть данные
                    {
                        p[i]=("id " + reader.GetValue(0).ToString() + " : " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString() + " " + reader.GetValue(3).ToString() + " " + reader.GetValue(4).ToString());
                        i++;
                    }
                }
            }
            if ((sel != 0) & (prod == "'мл'"))
            {
                string query = $"SELECT count(*) FROM Pizza where Наименование = '{piz}' and Тесто in ({prod})";
                SqlCommand que = new SqlCommand(query, conn);
                Int32 coun = (Int32)que.ExecuteScalar();
                p = new string[coun];
                c = coun;
                query = $"SELECT id, Наименование, [Диаметр/Объём(мл)], Цена FROM Pizza where Наименование = '{piz}' and Тесто in ({prod})";
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                using (DbDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read()) //Читаем пока есть данные
                    {
                        p[i] = ("id " + reader.GetValue(0).ToString() + " : " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString() + " мл" + " " + reader.GetValue(3).ToString());
                        i++;
                    }
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool InsOrder(int countPi, int[,] masP, string id_op, string id_cl, string adress, double price, string tel, string stol, double price2, string dis) //запись заказа
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            float sum=0;
            string ord = "Insert into [Order] (Дата, [Вид (Столик)], Статус, [id оператора], [id клиента], Комментарий, Цена, Скидка, Сумма) " + " values (@date, @vid ,@status, @id_op, @id_cl, @address, @price2, @dis, @price)";
            SqlCommand cmd2 = conn.CreateCommand();
            cmd2.CommandText = ord;
            DateTime date = DateTime.Now; //узнаём дату
            cmd2.Parameters.Add("@date", SqlDbType.DateTime).Value = date; //записываем дату
            cmd2.Parameters.Add("@vid", SqlDbType.VarChar).Value = stol; //записываем вид заказа
            cmd2.Parameters.Add("@status", SqlDbType.Int).Value = 1; //Статус
            cmd2.Parameters.Add("@id_op", SqlDbType.Int).Value = id_op; //ид оператора
            cmd2.Parameters.Add("@id_cl", SqlDbType.Int).Value = id_cl; //ид клиента
            cmd2.Parameters.Add("@address", SqlDbType.VarChar).Value = adress; //комментарий
            cmd2.Parameters.Add("@price2", SqlDbType.Float).Value = price2;
            cmd2.Parameters.Add("@dis", SqlDbType.Int).Value = dis;//записываем
            cmd2.Parameters.Add("@price", SqlDbType.Float).Value = price;
            int row = cmd2.ExecuteNonQuery();

            SqlCommand sqlCommand = new SqlCommand("select max(id) from [Order]", conn);
            string idm = sqlCommand.ExecuteScalar().ToString();
            for (int i = 0; i < countPi; i++)
            {
                string sql = "Insert into [Pizza in order] ([id заказа], [id пиццы], Количество) " + " values (@idz, @idp, @kol)";
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@idz", SqlDbType.Int).Value = int.Parse(idm); //id заказа                                            
                cmd.Parameters.Add("@idp", SqlDbType.Int).Value = masP[i,0]; //ид пиццы
                cmd.Parameters.Add("@kol", SqlDbType.Int).Value = masP[i, 1]; //количество                                                                               //записываем
                int rowCount = cmd.ExecuteNonQuery();
            }
            if (tel != "Телефон: +7-(   )-   -  -")
            {
                ord = $"select sum(Цена) from[Order] where [id клиента] = {id_cl}";
                sqlCommand = new SqlCommand(ord, conn);
                using (DbDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            sum = float.Parse(reader.GetDouble(0).ToString());
                        }
                    }
                }
                if (sum > 10000)
                {
                    string sql = $"update [Client] set Скидка=5 where id = {id_cl}";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();
                }
                if (sum > 15000)
                {
                    string sql = $"update [Client] set Скидка=10 where id = {id_cl}";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();
                }
                if (sum > 20000)
                {
                    string sql = $"update [Client] set Скидка=15 where id = {id_cl}";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Stat(out string[] stat, out int c) //статусы на листе заказы
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string query = "select count(distinct Статус) from [Status]";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            Int32 count = (Int32)sqlCommand.ExecuteScalar();
            c = count;
            stat = new string[count];
            int i = 0;
            query = "select distinct Статус from [Status]";
            sqlCommand = new SqlCommand(query, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        stat[i] = reader.GetString(0).ToString();
                        i++;
                    }
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Orders(string st, out string[,] ord, out int cou)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string query = $"select count(*) from [Order] join [Status] on [Order].Статус=[Status].id where [Status].Статус in ({st})";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            Int32 count = (Int32)sqlCommand.ExecuteScalar();
            cou = count;
           // ord = new string[count, reader.FieldCount];
            query = $@"select [Order].id, [Order].Дата, [Order].[Вид (Столик)], Комментарий, concat(pp.Фамилия, ' ', pp.Имя,' ', pp.Отчество) Клиент, concat(Person.Фамилия, ' ', Person.Имя, ' ', Person.Отчество) Оператор, [Order].Цена, [Status].Статус 
from [Order]
join [Status] on [Order].Статус=[Status].id
join Person on[Order].[id оператора] = Person.id
join Person pp on[Order].[id клиента] = pp.id
where [Status].Статус in ({st})";
            sqlCommand = new SqlCommand(query, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                ord = new string[count+1, reader.FieldCount];
                for (int g = 0; g < reader.FieldCount; g++)
                {
                    ord[0,g] = reader.GetName(g);
                }
                int i = 1;
                while (reader.Read()) //Читаем пока есть данные
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        ord[i, j] = reader.GetValue(j).ToString();
                    }
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool ChangeStat(string stat, int id)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string quere = $"select [Status].Статус from[Order] join [Status] on [Status].id = [Order].Статус where [Order].id = {id}"; //Статус в таблице
            string st = $"select id from [Status] where Статус = '{stat}'"; //id статуса
            int sta = new int();
            SqlCommand sqlCommand = new SqlCommand(st, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    sta = (reader.GetInt32(0)); //id нового статуса
                }
            }
            if (quere != stat)
                {
                    string sql = $"update [Order] set Статус={sta} where id = {id}";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();
                }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool ViewingOrd(int id, out string text)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            text = "";
            conn.Open();
            string quere2 = $"select CONCAT(Pizza.Наименование, ' ', Pizza.[Диаметр/Объём(мл)], ' ', Pizza.Тесто, ' ', Pizza.Цена, ' * ' ,[Pizza in order].Количество) Название from [Pizza in order] join Pizza on [Pizza in order].[id пиццы] = [Pizza].id where [id заказа] = {id}";
            SqlCommand sqlCommand2 = new SqlCommand(quere2, conn);
            using (DbDataReader reader = sqlCommand2.ExecuteReader())
            {
                while (reader.Read()) //Читаем пока есть данные
                {  //добавляем данные из БД на лист item
                    text = text + reader.GetValue(0).ToString() + "\n";
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool DelOrd(int id)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string sql = $"Delete from [Pizza in order] where [id заказа]={id}";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            int rowCount = cmd.ExecuteNonQuery();
            sql = $"Delete from [Order] where id={id}";
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            rowCount = cmd.ExecuteNonQuery();
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool VieKit(out string[,] ord, out int cou)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string query = "select count(*) from [Order] join [Status] on [Status].id=[Order].Статус where [Order].Статус in (1,2) and [Вид (Столик)] <> 'На вынос'";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            Int32 count = (Int32)sqlCommand.ExecuteScalar();
            cou = count;
            query = "select count(*) from [Order] join [Status] on [Status].id=[Order].Статус where [Order].Статус in (2,4) and [Вид (Столик)] = 'На вынос'";
            sqlCommand = new SqlCommand(query, conn);
            count = (Int32)sqlCommand.ExecuteScalar();
            cou = cou + count;
            int i = 1;
            query = @"select [Order].id, [Order].Дата, [Order].[Вид (Столик)], [Status].Статус, [Order].Комментарий 
from [Order] 
join [Status] on [Status].id=[Order].Статус 
where [Order].Статус in (1,2) and [Order].[Вид (Столик)] <> 'На вынос'";
            sqlCommand = new SqlCommand(query, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader()) //записываекм заказы в заведении
            {
                ord = new string[cou + 1, reader.FieldCount];
                for (int g = 0; g < reader.FieldCount; g++)
                {
                    ord[0, g] = reader.GetName(g);
                }
                while (reader.Read()) //Читаем пока есть данные
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        ord[i, j] = reader.GetValue(j).ToString();
                    }
                    i++;
                }
            }
            query = @"select [Order].id, [Order].Дата, [Order].[Вид (Столик)], [Status].Статус, [Order].Комментарий 
from [Order] 
join [Status] on [Status].id=[Order].Статус 
where [Order].Статус in (2,4) and [Order].[Вид (Столик)] = 'На вынос'";
            sqlCommand = new SqlCommand(query, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader()) //записываем заказы на вынос
            {
                while (reader.Read()) //Читаем пока есть данные
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        ord[i, j] = reader.GetValue(j).ToString();
                    }
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool ChangeStatK(int id)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string quere = $"select [Order].Статус, [Order].[Вид (Столик)] from[Order] join [Status] on [Status].id = [Order].Статус where [Order].id = {id}"; //Статус в таблице цифрой
            int sta = new int();
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {   
                    if ((reader.GetValue(1).ToString()== "На вынос") & (reader.GetInt32(0) == 4))
                    {
                        sta = (reader.GetInt32(0) - 2);
                    }
                    if ((reader.GetValue(1).ToString() == "На вынос") & (reader.GetInt32(0) == 2))
                    {
                        sta = (reader.GetInt32(0) + 4);
                    }
                    if (reader.GetValue(1).ToString() != "На вынос")
                    {
                        sta = (reader.GetInt32(0) + 1);
                    }
                     //id нового статуса
                }
            }
           // if (sta <= 3)
           // {
                string sql = $"update [Order] set Статус={sta} where id = {id}";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                int rowCount = cmd.ExecuteNonQuery();
                conn.Close();
                block = true;
                Console.WriteLine("Unblocked");
                return true;
           // }
            //conn.Close();
            //block = true;
            //Console.WriteLine("Unblocked");
            //return false;
        }
        public bool ViewPiz(int id, out string[] piz, out int c)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string quere = $"select count(*) from [Pizza in order] join Pizza on [Pizza in order].[id пиццы] = Pizza.id where [id заказа] = {id}"; //количество пицц
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            Int32 count = (Int32)sqlCommand.ExecuteScalar();
            c = count;
            piz = new string[count];
            int i = 0;
            quere = $"Select CONCAT('id: ', [Pizza in order].[id пиццы], ' ', Наименование,' ', [Диаметр/Объём(мл)], ' ', Тесто, ' * ', Количество) as Товар from [Pizza in order] join Pizza on [Pizza in order].[id пиццы] = Pizza.id where [id заказа] = {id}"; //товары в заказе
            sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    piz[i] = (reader.GetString(0)); //полное наименование товара
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Ingr(int id_p, out string ing)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            bool f = true;
            conn.Open();
            ing = "";
            string quere = $"select Ингредиенты, [Количество (гр)] from Ingredients  where [id пиццы]={id_p}"; //товары в заказе
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (f)
                    {
                        ing = ing + reader.GetName(0) + ":" + "\n";
                        f = false;
                    }
                    ing = ing + reader.GetValue(0).ToString()+ " - "+ reader.GetValue(1).ToString()+" гр." + "\n";
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool TMap(int id_p, out string tmap)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            bool f = true;
            int i = 1;
            conn.Open();
            tmap = "";
            string quere = $"select [Этапы производства] from [Technological map]  where [id пиццы]={id_p}"; //товары в заказе
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (f)
                    {
                        tmap = tmap + reader.GetName(0) + ":" + "\n";
                        f = false;
                    }
                    tmap = tmap + i+") " + reader.GetValue(0).ToString() + "\n";
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool FCTelDis(int id_c, out string tel, out string dis)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            tel = "";
            dis = "";
            string quere = $"select * from Client where id ={id_c}";
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    tel = reader.GetValue(1).ToString(); //Телефон
                    dis = reader.GetValue(2).ToString(); //скидка
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Payment(out string[] orders, out int c)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            int i = 0;
            string quere = $"select count(*) from [Order]  where Статус in (1, 2, 3)"; //количество пицц
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            Int32 count = (Int32)sqlCommand.ExecuteScalar();
            c = count;
            orders = new string[count];
            quere = "select [Order].id, [Order].Дата, [Order].[Вид (Столик)], CONCAT(Фамилия, ' ', Имя, ' ', Отчество) as 'Клиент', [Order].Цена from [Order] join [Person] on [Order].[id клиента]=[Person].id where Статус in (1, 2, 3)";
            sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    orders[i] = ("id: " + reader.GetValue(0).ToString() + " Дата: " + reader.GetValue(1).ToString() + " вид: " + reader.GetValue(2).ToString() + " клиент: " + reader.GetValue(3).ToString() + " Цена: "+ reader.GetValue(4).ToString()); //Телефон
                    i++;
                }
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Pay(int id_ord)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            int i = 0;
            string quere = $"select count(*) from [Pizza in order] where [id заказа] = {id_ord}"; //количество пицц
            string path =  $@"C:\Users\madke\Desktop\ДИПЛОМ\Чеки\{id_ord}.txt";
            StreamWriter file = new StreamWriter(path);
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            Int32 count = (Int32)sqlCommand.ExecuteScalar();
            int c = count;
            bool f = true;
            quere = $"select [Order].id, [Order].Дата, CONCAT(Наименование,' ', [Диаметр/Объём(мл)],' ', Тесто,'............... ', [Pizza].Цена,' * ', Количество), [Order].Цена, [Order].Скидка, [Order].Сумма from [Order] join [Pizza in order] on [Pizza in order].[id заказа]=[Order].id join [Pizza] on [Pizza].id=[Pizza in order].[id пиццы] where [id заказа] = {id_ord}";
            sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (f)
                    {
                        file.Write("ООО РОГА И КОПЫТА" + "\n");
                        file.Write("Чек: " + reader.GetValue(0).ToString() + "\n");
                        file.Write("Дата: " + reader.GetValue(1).ToString() + "\n");
                        file.Write("____________________________" + "\n");
                        f = false;
                    }
                    file.Write(reader.GetValue(2).ToString() + "\n");
                    if (i == (c-1))
                    {
                        file.Write("____________________________" + "\n");
                        file.Write("Итого без скидки: "+ reader.GetValue(3).ToString() + " рублей" +"\n");
                        file.Write("Скидка: " + reader.GetValue(4).ToString() + " %" + "\n");
                        file.Write("Сумма с учётом скидки: " + reader.GetValue(5).ToString() + " рублей" + "\n");
                    }
                    i++;
                }
            }
            file.Close();
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool PayOrd(int id_ord)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string quere = $"select Статус, [Вид (Столик)] from [Order] where [Order].id = {id_ord}"; //Статус в таблице
            int sta = new int();
            string vid = "";
            bool f = true;
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    vid = reader.GetValue(1).ToString();
                    sta = reader.GetInt32(0);
                    if ((sta !=3) & (vid != "На вынос"))
                    {
                        conn.Close();
                        f = false;
                        block = true;
                        Console.WriteLine("Unblocked");
                        return false;
                    }
                }
            }
            if ((sta == 1) & (vid == "На вынос"))
            {
                sta = sta + 3;
                string sql = $"update [Order] set Статус={sta} where id = {id_ord}";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                int rowCount = cmd.ExecuteNonQuery();
                conn.Close();
                block = true;
                Console.WriteLine("Unblocked");
                return true;
            }
            if ((sta == 3) & (vid != "На вынос"))
            {
                sta = sta + 3;
                string sql = $"update [Order] set Статус={sta} where id = {id_ord}";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                int rowCount = cmd.ExecuteNonQuery();
                conn.Close();
                block = true;
                Console.WriteLine("Unblocked");
                return true;
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return false;
        }
        public bool AddPr(string name, string size, string dough, string price, string log, string pass, string[,] ing, string[] tmap, out int error)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            error = 0;
            conn.Open();
            string s = size.Split(' ')[0];
            string quere = $"select * from Pizza where Наименование = '{name}' and [Диаметр/Объём(мл)] = '{s}' and Тесто = '{dough}'";
            SqlCommand sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    error = 1;
                    conn.Close();
                    block = true;
                    Console.WriteLine("Unblocked");
                    return false;
                }
            }
            quere = $"select [Должность id] from Oper where Логин = '{log}' and Пароль = '{pass}'";
            sqlCommand = new SqlCommand(quere, conn);
            using (DbDataReader reader = sqlCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) != 1)
                        {
                            error = 2;
                            conn.Close();
                            block = true;
                            Console.WriteLine("Unblocked");
                            return false;
                        }
                    }
                }
               else
                {
                    error = 2;
                    conn.Close();
                    block = true;
                    Console.WriteLine("Unblocked");
                    return false;
                }
            }
            quere = "Insert into Pizza (Наименование, [Диаметр/Объём(мл)], Тесто, Цена) " + " values (@n, @d, @t, @p) ";
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = quere;
            cmd.Parameters.Add("@n", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@d", SqlDbType.VarChar).Value = size.Split(' ')[0];
            cmd.Parameters.Add("@t", SqlDbType.VarChar).Value = dough.Split(' ')[0];
            cmd.Parameters.Add("@p", SqlDbType.Decimal).Value = Decimal.Parse(price);
            int rowCount = cmd.ExecuteNonQuery(); //добавили пиццу
            sqlCommand = new SqlCommand("select max(id) from Pizza", conn); //узнаем ид новой пиццы
            string idm = sqlCommand.ExecuteScalar().ToString();
            quere = "Insert into Ingredients ([id пиццы], Ингредиенты, [Количество (гр)]) " + " values (@idp, @ing, @c) "; //записываем Ингредиенты
            cmd.CommandText = quere;
            for (int i = 0; i < ing.GetLength(0); i++)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@idp", SqlDbType.Int).Value = idm;
                cmd.Parameters.Add("@ing", SqlDbType.VarChar).Value = ing[i,0];
                cmd.Parameters.Add("@c", SqlDbType.Int).Value = ing[i, 1];
                rowCount = cmd.ExecuteNonQuery();
            }
            quere = "Insert into [Technological map] ([id пиццы], [Этапы производства]) " + " values (@idp, @t) "; //записываем Ингредиенты
            cmd.CommandText = quere;
            for (int i = 0; i < tmap.Length; i++)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@idp", SqlDbType.Int).Value = idm;
                cmd.Parameters.Add("@t", SqlDbType.VarChar).Value = tmap[i];
                rowCount = cmd.ExecuteNonQuery();
            }
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
        public bool Statistic(string s,out string[,] stat)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            string query = "";
            Int32 count;
            conn.Open();
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            if (s== "Операторы")
            {
                query = "select count(distinct([id оператора])) from [Order]";
                sqlCommand = new SqlCommand(query, conn);
                count = (Int32)sqlCommand.ExecuteScalar();
                query = @"select concat(Person.Фамилия, ' ', Person.Имя, ' ', Person.Отчество) AS 'Оператор',count(*) as 'Заказов', sum(Цена) as 'Сумма'
from [Order] 
join [Person] on [Person].id = [Order].[id оператора]
group by Person.Фамилия, Person.Имя, Person.Отчество";
                sqlCommand = new SqlCommand(query, conn);
                using (DbDataReader reader = sqlCommand.ExecuteReader())
                {
                    stat = new string[count + 1, reader.FieldCount];
                    for (int g = 0; g < reader.FieldCount; g++)
                    {
                        stat[0, g] = reader.GetName(g);
                    }
                    int i = 1;
                    while (reader.Read()) //Читаем пока есть данные
                    {
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            stat[i, j] = reader.GetValue(j).ToString();
                        }
                        i++;
                    }
                }
                conn.Close();
                block = true;
                Console.WriteLine("Unblocked");
                return true;
            }
            if (s == "Клиенты")
            {
                query = "select count(distinct([id клиента])) from [Order]";
                sqlCommand = new SqlCommand(query, conn);
                count = (Int32)sqlCommand.ExecuteScalar();
                query = @"select concat(Фамилия, ' ', Имя, ' ', Отчество) AS 'Клиент',count(*) as 'Заказов', sum(Цена) as 'Сумма'
from [Order] 
join Person on Person.id = [Order].[id клиента]
group by Person.Фамилия, Person.Имя, Person.Отчество";
                sqlCommand = new SqlCommand(query, conn);
                using (DbDataReader reader = sqlCommand.ExecuteReader())
                {
                    stat = new string[count + 1, reader.FieldCount];
                    for (int g = 0; g < reader.FieldCount; g++)
                    {
                        stat[0, g] = reader.GetName(g);
                    }
                    int i = 1;
                    while (reader.Read()) //Читаем пока есть данные
                    {
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            stat[i, j] = reader.GetValue(j).ToString();
                        }
                        i++;
                    }
                }
                conn.Close();
                block = true;
                Console.WriteLine("Unblocked");
                return true;
            }
            if (s == "Товары")
            {
                query = "select count(distinct([id пиццы])) from [Pizza in order]";
                sqlCommand = new SqlCommand(query, conn);
                count = (Int32)sqlCommand.ExecuteScalar();
                query = @"select CONCAT(Наименование, ' ', [Диаметр/Объём(мл)], ' ', Тесто) as 'Пицца', sum(Количество) as 'Количество', Цена
from [Pizza in order]
join [Pizza] on [Pizza].id = [Pizza in order].[id пиццы]
group by  Наименование, [Диаметр/Объём(мл)], Тесто, Цена";
                sqlCommand = new SqlCommand(query, conn);
                using (DbDataReader reader = sqlCommand.ExecuteReader())
                {
                    stat = new string[count + 1, reader.FieldCount];
                    for (int g = 0; g < reader.FieldCount; g++)
                    {
                        stat[0, g] = reader.GetName(g);
                    }
                    int i = 1;
                    while (reader.Read()) //Читаем пока есть данные
                    {
                        
                        stat[i, 0] = reader.GetValue(0).ToString(); // название
                        stat[i, 1] = reader.GetValue(1).ToString(); // количество
                        decimal sum = reader.GetInt32(1) * reader.GetDecimal(2);
                        sum = Math.Round(Convert.ToDecimal(sum), 2);
                        stat[i, 2] = sum.ToString();
                        i++;
                    }
                }
                conn.Close();
                block = true;
                Console.WriteLine("Unblocked");
                return true;
            }
            else
            {
                conn.Close();
                stat = new string[0, 0];
                block = true;
                Console.WriteLine("Unblocked");
                return false;
            }
        }
        public bool Dismissal(string id)
        {
            while (!block)
            {
                Console.WriteLine("The process is blocked by another user");
            }
            block = false;
            Console.WriteLine("Blocked");
            conn.Open();
            string query = $"update Oper set Статус=0 where id = {id} ";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            int rowCount = sqlCommand.ExecuteNonQuery();
            conn.Close();
            block = true;
            Console.WriteLine("Unblocked");
            return true;
        }
    }


    class DBUtils
    {
        public static SqlConnection GetDBConnection()
        {
            string datasource = @"DESKTOP-9E64BSA\SQLEXPRESS";
            string database = "dostavka";
            string username = "sa";
            string password = "1111";
            return DBSQLServerUtils.GetDBConnection(datasource, database, username, password);
        }
    }
    class DBSQLServerUtils
    {
        public static SqlConnection
                 GetDBConnection(string datasource, string database, string username, string password)
        {
            //
            // Data Source=TRAN-VMWARE\SQLEXPRESS;Initial Catalog=simplehr;Persist Security Info=True;User ID=sa;Password=1111
            //
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;
            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
