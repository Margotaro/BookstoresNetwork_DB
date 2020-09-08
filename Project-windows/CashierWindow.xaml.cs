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
    /// Логика взаимодействия для CashierWindow.xaml
    /// </summary>
    public partial class CashierWindow : Window, IBookstoreWindow
    {

        DAO dao;
        BookChangeManager bookChangeManager;
        ObservableCollection<CatalogueListItem> catalogue;

        ObservableCollection<string> selectedGenresList;
        public CashierWindow()
        {
            InitializeComponent();
            dao = new DAO(this);
            bookChangeManager = new BookChangeManager(dao, this);
            catalogue = new CatalogueViewList((List<Book>)dao.getCatalogueContents()).list;
            cashierCatalogue.ItemsSource = catalogue;

            //bookWindow
            languageComboBox.ItemsSource = Book.Languages;
            genreSelectComboBox.ItemsSource = dao.uploadGenres();
            selectedGenresList = new ObservableCollection<string>();
            selectedGenresListBox.Items.Clear();
        }
        private void genreSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var v in e.AddedItems)
            {
                if (!selectedGenresList.Contains(v.ToString()))
                {
                    selectedGenresList.Add(v.ToString());
                    selectedGenresListBox.ItemsSource = selectedGenresList;
                }
            }
        }
        private void selectedGenresListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var v in e.AddedItems)
            {
                selectedGenresList.Remove(v.ToString());
                selectedGenresListBox.ItemsSource = selectedGenresList;
            }
        }
        public void dataGridWorkerUpdate()
        {
        }
        public void dataGridBookUpdate()
        {
            cashierCatalogue.ItemsSource = new CatalogueViewList(dao.CachedBooks).list;
        }

        private void findBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (titleTextBox.Text == "" && authorTextBox.Text == "" && publisherTextBox.Text == "" && selectedGenresList.Count == 0 && languageComboBox.SelectedIndex <= -1)
            {
                cashierCatalogue.ItemsSource = catalogue;
            }
            else
            {
                var title = titleTextBox.Text;
                var author = authorTextBox.Text;
                var publisher = publisherTextBox.Text;
                var language = languageComboBox.Text;
                var genres = selectedGenresListBox.Items;

                cashierCatalogue.ItemsSource = bookChangeManager.bookBeingSearched(title, author, publisher, language, genres);
            }
        }
    }
}
