using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace BookStore
{
    /// <summary>
    /// Логика взаимодействия для OwnerWindow.xaml
    /// </summary>
    public partial class OwnerWindow : Window
    {
        private string _workerName = "not set yet";
        private string _position = "not set yet";
        private string _salary = "not set yet";
        private string _shift = "not set yet";
        private string _login = "not set yet";
        private string _password = "not set yet";
        DAO dao;
        WorkerChangeManager changeManager;
        WorkerFactory workerFactory;

        public OwnerWindow()
        {
            InitializeComponent(); 
            dao = Connect();
            changeManager = new WorkerChangeManager(dao);
            workerFactory = new WorkerFactory(changeManager);

            dao.hui();
        }
        private DAO Connect()
        {
            string ConnectionString = "Host=127.0.0.1;Port=5432;Username=allprivileged_admin;Password=superadmin;Database=bookstores_network";
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
            DatabaseConnection databaseConnection = new DatabaseConnection(npgsqlConnection);
            databaseConnection.OpenConnection();
            return new DAO(databaseConnection);
        }

        private void addWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            var name = workerNameTextBox.Text;
            var position = positionTextBox.Text;
            var salary = salaryTextBox.Text;

            var worker1 = workerFactory?.makeModelWorker("qwen", "manager", 100, "first", "irapap", "228");
        }
    }
}
