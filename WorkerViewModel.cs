using System.Collections.ObjectModel;

public class WorkerViewModel
{
    public string name;
    public DateTime hiringdate;
    public string salary;
    public string t_shift;
    public string bs_address;
    public string login;
    public string password;
    public ObservableCollection<BookstoresListItem> bookstores;
    public WorkerViewModel(string name, DateTime hiringdate, string salary, string t_shift, string bs_address, string login, string password, ObservableCollection<BookstoresListItem> bookstores)
    {
        this.name = name; 
        this.hiringdate = hiringdate;
        this.salary = salary;
        this.t_shift = t_shift;
        this.bs_address = bs_address;
        this.login = login;
        this.password = password;
        this.bookstores = bookstores;
    }
}
