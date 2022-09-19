using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OOOPovaryonok.Classes
{
    public class SQLHelper
    {
        public static string ConnectionString = @"Data Source=ILUVATAR\MSSQLMYSERVER;Initial Catalog=Trade; Integrated Security = True";

        public static SqlConnection connection = new SqlConnection(ConnectionString);

        public void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }
    }
}
