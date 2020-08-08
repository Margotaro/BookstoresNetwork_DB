using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        DAO dao;
        WorkerChangeManager changeManager;
        WorkerFactory workerFactory;
        ObservableCollection<BookstoresListItem> bookstores;
        ObservableCollection<CatalogueListItem> catalogue;
        ObservableCollection<StaffListItem> staff;

        public OwnerWindow()
        {
            InitializeComponent(); 
            dao = Connect();
            changeManager = new WorkerChangeManager(dao);
            workerFactory = new WorkerFactory(changeManager, dao);

            bookstores = new BookstoresViewList((List<Bookstore>)dao.getAllBookstores()).list;
            ownerBookstores.ItemsSource = bookstores;

            staff = new StaffViewList((List<Worker>)dao.getAllWorkers()).list;
            ownerStaff.ItemsSource = staff;

            catalogue = new CatalogueViewList((List<Book>)dao.getCatalogueContents()).list;
            ownerCatalogue.ItemsSource = catalogue;
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
        }
    }
}
