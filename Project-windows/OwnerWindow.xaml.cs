using Npgsql;
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

namespace BookStore
{
    /// <summary>
    /// Логика взаимодействия для OwnerWindow.xaml
    /// </summary>
    public partial class OwnerWindow : Window, IBookstoreWindow
    {
        DAO dao;
        WorkerChangeManager workerChangeManager;
        BookChangeManager bookChangeManager;
        ObservableCollection<BookstoresListItem> bookstores;
        ObservableCollection<CatalogueListItem> catalogue;
        ObservableCollection<StaffListItem> staff;

        ObservableCollection<string> selectedGenresList;

        public OwnerWindow()
        {
            InitializeComponent(); 
            dao = new DAO(this);
            workerChangeManager = new WorkerChangeManager(dao, this);
            bookChangeManager = new BookChangeManager(dao, this);

            //content
            bookstores = new BookstoresViewList((List<Bookstore>)dao.getAllBookstores()).list;
            ownerBookstores.ItemsSource = bookstores;

            staff = new StaffViewList((List<Worker>)dao.getAllWorkers()).list;
            ownerStaff.ItemsSource = staff;

            catalogue = new CatalogueViewList((List<Book>)dao.getCatalogueContents(), (List<Bookstore>)dao.getAllBookstores(), dao.CachedBookstoresOnBooksAvailability).list;
            ownerCatalogue.ItemsSource = catalogue;

            //staff window
            shiftComboBox.ItemsSource = WorkingHours.Shifts.Select(x => x.shiftname).ToList();
            workplaceComboBox.ItemsSource = bookstores.Select(x => x.Address.ToString()).ToList();

            //bookWindow
            languageComboBox.ItemsSource = Book.Languages;
            genreSelectComboBox.ItemsSource = dao.uploadGenres();
            selectedGenresList = new ObservableCollection<string>();
            selectedGenresListBox.Items.Clear();
        }

        public void dataGridWorkerUpdate() 
        {
            ownerStaff.ItemsSource = new StaffViewList(dao.CachedWorkers).list;
        }
        public void dataGridBookUpdate()
        {
            ownerCatalogue.ItemsSource = new CatalogueViewList(dao.CachedBooks, dao.CachedBookstores, dao.CachedBookstoresOnBooksAvailability).list; 
        }

        private void addWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            var name = workerNameTextBox.Text;
            var salary = salaryTextBox.Text;
            var t_shift = shiftComboBox.Text;
            var login = usernameTextBox.Text;
            var password = passwordTextBox.Text;
            if(name == "" || salary == "" || t_shift == "" || login == "" || password == "" || hiringDatePicker.SelectedDate == null || workplaceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please fill out all the fields in the form");
            }
            else
            {
                var hiringdate = Convert.ToDateTime(hiringDatePicker.SelectedDate.Value);
                var bs_address = workplaceComboBox.SelectedItem.ToString();
                WorkerViewModel wvModel = new WorkerViewModel(name, new DateTime(hiringdate.Year, hiringdate.Month, hiringdate.Day), salary, t_shift, bs_address, login, password, bookstores);
                workerChangeManager.workerCreated(wvModel);
            }

        }
        private void addBookButton_Click(object sender, RoutedEventArgs e)
        {
            //общие параметры
            var title = titleTextBox.Text;
            var author = authorTextBox.Text;
            var publisher = publisherTextBox.Text;
            var language = languageComboBox.Text;
            var genre = selectedGenresListBox.Items;//проверить
            //электронная
            var electronic_ver_cost = electronicVersionCostTextBox.Text;
            var electronic_ver_storage_route = electronicVersionPDFRouteTextBox.Text;

            //печатная
            var wholesale_printed_ver_cost = wholesalePrintedVersionCostTextBox.Text;
            var retail_printed_ver_cost = retailPrintedVersionCostTextBox.Text;
            var warehouse_printed_ver_quantity = warehouseQuantityPrintedVersionTextBox.Text;

            if (title == "" || author == "" || publisher == "" || language == "" || genre == null || genre.Count == 0 ||
                ((electronic_ver_cost == "" || electronic_ver_storage_route == "") && (wholesale_printed_ver_cost == "" || retail_printed_ver_cost == "" || warehouse_printed_ver_quantity == "")))
            {
                MessageBox.Show("Please fill out all the fields in the form");
            }
            else
            {
                BookViewModel wvModel = new BookViewModel(title, author, publisher, language, genre, electronic_ver_cost, electronic_ver_storage_route, retail_printed_ver_cost, wholesale_printed_ver_cost, warehouse_printed_ver_quantity);
                bookChangeManager.bookCreated(wvModel);
            }
        }

        private void genreSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(var v in e.AddedItems)
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
            foreach(var v in e.AddedItems)
            {
                selectedGenresList.Remove(v.ToString());
                selectedGenresListBox.ItemsSource = selectedGenresList;
            }
        }
        private void DecimalNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]*(\.[0-9]*)?$");
            var u = ((TextBox)sender).Text.Length - ((TextBox)sender).Text.IndexOf('.');
            e.Handled = !(regex.IsMatch(e.Text) && !(u > 2 && ((TextBox)sender).Text.Contains('.')) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)) && !(((TextBox)sender).Text == "" && e.Text == "."));
        }
        private void WholeNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]+");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void percentValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]+");
            e.Handled = !(regex.IsMatch(e.Text) && (Convert.ToInt32(((TextBox)sender).Text + e.Text) < 100 && Convert.ToInt32(((TextBox)sender).Text + e.Text) > 0));
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected_books = ownerCatalogue.SelectedItems;
            if(selected_books.Count == 0)
            {
                MessageBox.Show("Please select books to delete");
            }
            else
            {
                for (var i = selected_books.Count - 1; i >= 0; i--)
                {
                    var book = (CatalogueListItem)selected_books[i];
                    BookViewModel wvModel = new BookViewModel(book.Title, book.Author, book.Publisher, book.Language,
                                                              book.Genres.Split(',').ToList(), book.Electronic_version_cost,
                                                              book.Electronic_version_path, book.Retail_cost, book.Wholesale_cost,
                                                              book.Warehouse_quantity);
                    bookChangeManager.bookDeleted(wvModel);
                }
            }
        }

        private void fireWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            var selected_workers = ownerStaff.SelectedItems;
            if (selected_workers.Count == 0)
            {
                MessageBox.Show("Please select books to delete");
            }
            else
            {
                if(firingDatePicker.SelectedDate == null)
                { MessageBox.Show("Please select date of firing"); }
                else
                {
                    for (var i = selected_workers.Count - 1; i >= 0; i--)
                    { 
                        var worker = (StaffListItem)selected_workers[i];
                        var firingdate = Convert.ToDateTime(firingDatePicker.SelectedDate.Value);
                        workerChangeManager.workerFired(new Id<IWorker>(worker.EmployeeID), new DateTime(firingdate.Year, firingdate.Month, firingdate.Day));
                    }
                }
            }
                        
        }
    }
}
