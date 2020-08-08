using System.Collections.Generic;
using System.Collections.ObjectModel;
using BookStore;
class BookstoresViewList
{
    public ObservableCollection<BookstoresListItem> list { get; set; }
    public BookstoresViewList(List<Bookstore> bookstores)
    {
        list = new ObservableCollection<BookstoresListItem>();
        foreach (var bookstore in bookstores)
        {
            list.Add(new BookstoresListItem(
                bookstore.id.ToString(), 
                bookstore.address.ToString(), 
                bookstore.cashierTelephone.ToString(), 
                bookstore.managerTelephone.ToString()));
        }
    }
}

class StaffViewList
{
    public ObservableCollection<StaffListItem> list { get; set; }
    public StaffViewList(List<Worker> staff)
    {
        list = new ObservableCollection<StaffListItem>();
        foreach(var worker in staff)
        {
            list.Add(new StaffListItem(
                worker.id.ToString(),
                worker.name.ToString(),
                worker.hir.ToString(),//hiringdate
                worker.id.ToString(),//
                worker.position.ToString(),
                worker.salary.ToString(),
                worker.hours.ToString(),
                worker..ToString()
                ));
        }
    }
}
