using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ExceptionServices;
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
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window, IBookstoreWindow
    {
        DAO dao;
        Id<IWorker> CurrentManagerId;
        Id<IBookstore> CurrentBookstoreId;
        WorkerChangeManager workerChangeManager;
        ObservableCollection<StaffListItem> staff;
        HashSet<string> editedWorkers;
        List<Worker> staff_inWorkerType;
        public ManagerWindow()
        {
            InitializeComponent();
            dao = new DAO(this);
            CurrentBookstoreId = new Id<IBookstore>("1");
            CurrentManagerId = new Id<IWorker>("22");
            workerChangeManager = new WorkerChangeManager(dao, this);

            staff_inWorkerType = (List<Worker>)dao.getAllWorkers().Where(i => i.working_place.id.ToString() == CurrentBookstoreId.ToString()).ToList();
            staff = new StaffViewList(staff_inWorkerType).list;
            managerSubordinatesDataGrid.ItemsSource = staff;
            shiftComboBox.ItemsSource = WorkingHours.Shifts.Select(i => i.shiftname);
            positionComboBox.ItemsSource = new List<string> { "cashier", "deliverer", "manager" };


            editedWorkers = new HashSet<string>() { };
            this.ContentRendered += (s, e) =>
            {
                managerSubordinatesDataGrid.Columns[0].IsReadOnly = true;
                managerSubordinatesDataGrid.Columns[2].IsReadOnly = true;
                managerSubordinatesDataGrid.Columns[3].IsReadOnly = true;
                managerSubordinatesDataGrid.Columns[4].IsReadOnly = true;
                managerSubordinatesDataGrid.Columns[7].IsReadOnly = true;
            };
        }

        public void dataGridBookUpdate()
        {

        }
        public void dataGridWorkerUpdate()
        {

        }

        private void hireButton_Click(object sender, RoutedEventArgs e)
        {
            var name = workerNameTextBox.Text;
            var salary = salaryTextBox.Text;
            var login = loginTextBox.Text;
            var password = passwordTextBox.Text;
            var position = positionComboBox.SelectedItem;
            var shift = shiftComboBox.SelectedItem;

            if (name == "" || salary == "" || login == "" || password == "" || position == null || shift == null)
            {
                MessageBox.Show("Fill out all information, please");
            }
            else
            {
                var hiringdate = System.DateTime.Now;
                var bs_address = dao.CachedBookstores;
                //сделать создание работника?

            }    
        }

        private void saveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var ew in editedWorkers)
            {
                foreach(var subordinate in staff_inWorkerType)
                {
                    if(subordinate.id.ToString() == ew)
                    {
                        var worker_info = staff.Where(i => i.EmployeeID.ToString() == ew).ToList().ElementAt(0);
                        if (!Regex.IsMatch(worker_info.Full_name, @"^[a-zA-Z ]+$") || worker_info.Full_name == "")
                            MessageBox.Show("The name is incorrect, it must contain only letters and spaces.");
                        else 
                        {
                            subordinate.name = worker_info.Full_name;
                        }
                        if (!Regex.IsMatch(worker_info.Salary, @"^[0-9.]+$") || Convert.ToDouble(worker_info.Salary) == 0)
                            MessageBox.Show("The salary is incorrect, it must have numbers only.");
                        else
                        {
                            subordinate.salary = Convert.ToDouble(worker_info.Salary);
                        }
                        if (worker_info.ShiftID != "first" && worker_info.ShiftID != "second" || worker_info.ShiftID == "")
                            MessageBox.Show("The shift is incorrect, it can be either \'first\' or \'second\' for now.");
                        else
                        {
                            subordinate.hours = WorkingHours.Shifts.Single(p => p.shiftname == worker_info.ShiftID);
                        }
                    }
                }
            }
        }
        private void managerSubordinatesDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (managerSubordinatesDataGrid.SelectedItem != null)
            {
                editedWorkers.Add((e.Row.Item as StaffListItem).EmployeeID.ToString());
                return; 
            }
            else return;
        }
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            staff = new StaffViewList((List<Worker>)dao.getAllWorkers().Where(i => i.working_place.id.ToString() == CurrentBookstoreId.ToString()).ToList()).list;
            managerSubordinatesDataGrid.ItemsSource = null;
            managerSubordinatesDataGrid.ItemsSource = staff;
            editedWorkers.Clear();
        }
    }
}
