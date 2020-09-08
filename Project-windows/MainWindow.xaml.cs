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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BookStore.Properties;
using Npgsql;

namespace BookStore
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void proceedButton_Click(object sender, RoutedEventArgs e)
        {
            var info = new CashierWindow();
            info.Show();
            this.Close();

            // deploy first db connection
            // fetch role
            // deploy second db connection
            // init needed dao

            // model loader
        }
    }
}
