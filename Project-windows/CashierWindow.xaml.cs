using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using Npgsql;

namespace BookStore
{
    /// <summary>
    /// Логика взаимодействия для CashierWindow.xaml
    /// </summary>
    public partial class CashierWindow : Window, IBookstoreWindow
    {

        DAO dao;
        Id<IWorker> CurrentCashierId;
        Id<IBookstore> CurrentBookstoreId;
        BookChangeManager bookChangeManager;
        PurchaseManager purchaseManager;
        ObservableCollection<CatalogueListItem> catalogue;

        ObservableCollection<string> selectedGenresList;
        public CashierWindow()
        {
            InitializeComponent();
            CurrentCashierId = new Id<IWorker>("6");
            CurrentBookstoreId = new Id<IBookstore>("3");
            dao = new DAO(this);
            bookChangeManager = new BookChangeManager(dao, this);
            purchaseManager = new PurchaseManager(dao, this);
            catalogue = new CatalogueViewList((List<Book>)dao.getCatalogueContents(), (List<Bookstore>)dao.getAllBookstores(), 
                                              dao.CachedBookstoresOnBooksAvailability).list;
            cashierCatalogue.ItemsSource = catalogue;

            //bookWindow
            languageComboBox.ItemsSource = Book.Languages;
            genreSelectComboBox.ItemsSource = dao.uploadGenres();
            selectedGenresList = new ObservableCollection<string>();
            selectedGenresListBox.Items.Clear();
            selectedGenresList.CollectionChanged += catalogueTabContentCollectionChanged;

            //purchaseWindow
            soldBookTypeComboBox.ItemsSource = new List<string>() { "electronic", "printed" };
            soldBookTypeComboBoxOnReturn.ItemsSource = new List<string>() { "electronic", "printed" };
            purchaseCatalogueDataGrid.ItemsSource = catalogue;
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
        public void dataGridBookUpdate()//спросить у коли
        {
            cashierCatalogue.ItemsSource = new CatalogueViewList(dao.CachedBooks, dao.CachedBookstores, dao.CachedBookstoresOnBooksAvailability).list;
            purchaseCatalogueDataGrid.ItemsSource = new CatalogueViewList(dao.CachedBooks, dao.CachedBookstores, dao.CachedBookstoresOnBooksAvailability).list;
        }

        private void catalogueTabContentChangedEventHandler(object sender, RoutedEventArgs e)
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
                var language = languageComboBox.SelectedItem == null ? "" : languageComboBox.SelectedItem.ToString();
                var genres = selectedGenresListBox.Items;

                cashierCatalogue.ItemsSource = new CatalogueViewList((List<Book>)bookChangeManager.bookBeingSearched(title, author, publisher, language, genres), 
                                                                      dao.CachedBookstores,
                                                                      dao.CachedBookstoresOnBooksAvailability).list;
            }
        }
        private void purchaseCatalogueTabContentChangedEventHandler(object sender, RoutedEventArgs e)
        {
            if (purchaseTitleTextBox.Text == "" && purchaseAuthorTextBox.Text == "" && purchasePublisherTextBox.Text == "")
            {
                purchaseCatalogueDataGrid.ItemsSource = new CatalogueViewList((List<Book>)dao.getCatalogueContents(), (List<Bookstore>)dao.getAllBookstores(),
                                              dao.CachedBookstoresOnBooksAvailability).list;
            }
            else
            {
                var title = purchaseTitleTextBox.Text;
                var author = purchaseAuthorTextBox.Text;
                var publisher = purchasePublisherTextBox.Text;

                purchaseCatalogueDataGrid.ItemsSource = new CatalogueViewList((List<Book>)bookChangeManager.bookBeingSearched(title, author, publisher),
                                                                      dao.CachedBookstores,
                                                                      dao.CachedBookstoresOnBooksAvailability).list;
            }
        }
        private void catalogueTabContentCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            catalogueTabContentChangedEventHandler(sender, new RoutedEventArgs());
        }
        private void purchaseCatalogueComboBoxSelected(object sender, RoutedEventArgs e)
        {
            purchaseCatalogueTabContentChangedEventHandler(sender, new RoutedEventArgs());

            if (soldBookTypeComboBox.SelectedItem == null)
            {
                return;
            }
            else if (soldBookTypeComboBox.SelectedItem.ToString() == "printed")
            {
                purchaseCatalogueDataGrid.ItemsSource = purchaseCatalogueDataGrid.Items.Cast<object>().Where(i => (i as CatalogueListItem).Wholesale_cost != "0" && (i as CatalogueListItem).Retail_cost != "0" && (i as CatalogueListItem).Bookstores.Contains("магазин: " + CurrentBookstoreId.ToString() + ";"));
            }
            else
            {
                purchaseCatalogueDataGrid.ItemsSource = purchaseCatalogueDataGrid.Items.Cast<object>().Where(i => (i as CatalogueListItem).Electronic_version_path != "" && (i as CatalogueListItem).Electronic_version_cost != "0");
            }
            
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
                var language = languageComboBox.SelectedItem.ToString();
                var genres = selectedGenresListBox.Items;

                cashierCatalogue.ItemsSource = new CatalogueViewList((List<Book>)bookChangeManager.bookBeingSearched(title, author, publisher, language, genres), 
                                                                      dao.CachedBookstores,
                                                                      dao.CachedBookstoresOnBooksAvailability).list;
            }
        }

        private void clearLangButton_Click(object sender, RoutedEventArgs e)
        {
            languageComboBox.SelectedIndex = -1;
        }

        private void WholeNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]+");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void confirmPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if(soldBookTypeComboBox.SelectedItem == null || purchaseCatalogueDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Пожалуйста, заполните поле с типом продаваемых книг и выделите необходимые книги с использованием Ctrl+клик");
            }
            else if(quantityOfSoldItemsTextBox.Text == "" && soldBookTypeComboBox.SelectedItem.ToString() == "printed")
            {
                MessageBox.Show("Пожалуйста, введите количество книг, которые хотите вернуть");
            }
            else 
            {
                try
                {
                    var selected_books = purchaseCatalogueDataGrid.SelectedItems.Cast<object>().Select(p => new PurchaseParameters(CurrentCashierId, CurrentBookstoreId, new Id<IBook>((p as CatalogueListItem).CatalogueID), soldBookTypeComboBox.SelectedItem.ToString(), Convert.ToInt32(quantityOfSoldItemsTextBox.Text))).ToList();
                    receipt_number_cashWindow.Content = purchaseManager.MakePurchase(selected_books);
                }
                catch
                {
                    MessageBox.Show("Кажется, вы попытались выбрать больше книг чем можно в этом магазине");
                }
            }
            
        }

        private void returnBooksButton_Click(object sender, RoutedEventArgs e)
        {
            if(receiptIDtextbox.Text == "" || bookIDTextBox.Text == "" || itemsToReturnQuantityTextBox.Text == "" || itemsToReturnQuantityTextBox.Text == "0" || soldBookTypeComboBoxOnReturn.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните корректные данные для возврата книги");
            }
            else
            {
                purchaseManager.ReturnBook(new Id<IBook>(bookIDTextBox.Text), Convert.ToInt32(itemsToReturnQuantityTextBox.Text), new Id<IReceipt>(receiptIDtextbox.Text), soldBookTypeComboBoxOnReturn.SelectedItem.ToString());                   
            }
        }
    }
}
