using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using global::MySql.Data.MySqlClient;

namespace Аунтефикация_Курсач_
{
    public partial class AuthTo : Form
    {
        // строка подключения к БД
        string connStr = "server=caseum.ru;port=33333;user=st_2_20_19;database=is_2_20_st19_KURS;password=69816309;";

        //Переменная соединения
        MySqlConnection conn;
        //Логин и пароль к данной форме Вы сможете посмотреть в БД db_test таблице t_user

        //Класс необходимый для хранения состояния авторизации во время работы программы
        static class Auth
        {
            //Статичное поле, которое хранит значение статуса авторизации
            public static bool auth = false;

            //Статичное поле, которое хранит значения ID пользователя
            public static string auth_id = null;

            //Статичное поле, которое хранит значения ФИО пользователя
            public static string auth_fio = null;

            //Статичное поле, которое хранит количество привелегий пользователя
            public static int auth_role = 0;
        }
        public void GetUserInfo(string login_user)
        {
            //Объявлем переменную для запроса в БД
            string selected_id_stud = textBox1.Text;

            // устанавливаем соединение с БД
            conn.Open();

            // запрос
            string sql = $"SELECT * FROM driver WHERE login='{login_user}'";

            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(sql, conn);

            // объект для чтения ответа сервера
            MySqlDataReader reader = command.ExecuteReader();

            // читаем результат
            while (reader.Read())
            {
                // элементы массива [] - это значения столбцов из запроса SELECT
                Auth.auth_id = reader[0].ToString();
                Auth.auth_fio = reader[1].ToString();
                Auth.auth_role = Convert.ToInt32(reader[4].ToString());
            }

            reader.Close(); // закрываем reader
            // закрываем соединение с БД
            conn.Close();
        }

        static string sha256(string randomString)
        {
            //Тут происходит криптографическая магия. Смысл данного метода заключается в том, что строка залетает в метод
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public AuthTo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Запрос в БД на предмет того, если ли строка с подходящим логином и паролем
            string sql = "SELECT * FROM driver WHERE login = @un and password= @up";

            //Открытие соединения
            conn.Open();

            //Объявляем таблицу
            DataTable table = new DataTable();

            //Объявляем адаптер
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            //Объявляем команду
            MySqlCommand command = new MySqlCommand(sql, conn);

            //Определяем параметры
            command.Parameters.Add("@un", MySqlDbType.VarChar, 25);
            command.Parameters.Add("@up", MySqlDbType.VarChar, 25);

            //Присваиваем параметрам значение
            command.Parameters["@un"].Value = textBox1.Text;
            command.Parameters["@up"].Value = sha256(textBox2.Text);

            //Заносим команду в адаптер
            adapter.SelectCommand = command;

            //Заполняем таблицу
            adapter.Fill(table);

            //Закрываем соединение
            conn.Close();
            //Если вернулась больше 0 строк, значит такой пользователь существует

            if (table.Rows.Count > 0)
            {
                //Присваеваем глобальный признак авторизации
                Auth.auth = true;

                //Достаем данные пользователя в случае успеха
                GetUserInfo(textBox1.Text);

                //Закрываем форму
                this.Close();
            }
            else
            {
                //Отобразить сообщение о том, что авторизаия неуспешна
                MessageBox.Show("Неверные данные авторизации!");
            }
        }

        private void AuthTo_Load(object sender, EventArgs e)
        {
            //Сокрытие текущей формы
            //this.Hide();

            //Инициализируем и вызываем форму диалога авторизации
            AuthTo auth = new AuthTo();

            //Вызов формы в режиме диалога
           // auth.ShowDialog();

            //Если авторизации была успешна и в поле класса хранится истина, то делаем движуху:
            if (Auth.auth)
            {
                //Отображаем рабочую форму
                this.Show();

                //Вытаскиваем из класса поля в label'ы
                //label5.Text = Auth.auth_id;
                //label4.Text = Auth.auth_fio;
                //label6.Text = "Успешна!";

                //Красим текст в label в зелёный цвет
                //label6.ForeColor = Color.Green;

                //Вызываем метод управления ролями
                //ManagerRole(Auth.auth_role);
            }
            //иначе
            else
            {
                //Закрываем форму
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
