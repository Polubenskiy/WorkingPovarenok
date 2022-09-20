using OOOPovaryonok.Classes;
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

namespace OOOPovaryonok
{
    public partial class Authorization : Form
    {
        public Authorization() => InitializeComponent();

        SQLHelper sqlDatabase = new SQLHelper();

        private void Authorization_Load(object sender, EventArgs e)
        {
            try
            {
                sqlDatabase.OpenConnection();
            }
            catch (SqlException exeception)
            {
                switch (exeception.Number)
                {
                    case 17: MessageBox.Show("Неверное имя сервера", "Подключение к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error); break;
                    case 4060: MessageBox.Show("Неверное имя Базы данных", "Подключение к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error); break;
                    case 18456: MessageBox.Show("Неверное имя пользователя или пароль", "Подключение к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error); break;
                }
                MessageBox.Show(exeception.Message + "Уровень ошибки " + exeception.Class, "Подключение к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            catch (Exception exeception)
            {
                MessageBox.Show("Ошибка к подключению " + exeception.Message, "Подключение к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            MessageBox.Show("Связь с сервером установлена", "Подключение к серверу", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Authorization_FormClosing(object sender, FormClosingEventArgs e)
        {
            sqlDatabase.CloseConnection();
        }
    }
}
