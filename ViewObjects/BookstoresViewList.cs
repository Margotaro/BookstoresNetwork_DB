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
