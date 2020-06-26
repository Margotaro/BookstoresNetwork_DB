using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Diagnostics;
using System.Windows;

namespace BookStore
{
    class DatabaseConnection
    {
        //"Host=127.0.0.1;Port=5432;Username=allprivileged_admin;Password=superadmin;Database=bookstores_network";

        private static DatabaseConnection NewConnection = null;
        public NpgsqlConnection Connection { get; }
        public DatabaseConnection(NpgsqlConnection Connection)
        {
            this.Connection = Connection;
        }

        public DatabaseConnection CreateConnection => NewConnection = new DatabaseConnection(Connection);
        public void OpenConnection()
        {
            try
            {
                Connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }
        public void CloseConnection()
        {
            try
            {
                Connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }
    }
}
