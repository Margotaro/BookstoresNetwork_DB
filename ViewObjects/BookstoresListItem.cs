using System.Windows.Documents;
using System.Collections.Generic;
using BookStore;

public class BookstoresListItem
{
    public string BookstoreID { get; set; }
    public string Address { get; set; }
    public string CashierTelephone { get; set; }
    public string ManagerTelephone { get; set; }

    public BookstoresListItem(string b_id, string address, string cashier_telephone, string manager_telephone)
    {
        BookstoreID = b_id;
        Address = address;
        CashierTelephone = cashier_telephone;
        ManagerTelephone = manager_telephone;
    }
    //???
    static List<BookstoresListItem> ConvertBookstorestToBookstoresListItem(List<Bookstore> bookstores)
    {
        var itemList = new List<BookstoresListItem>();
        foreach (Bookstore b in bookstores)
        {
            itemList.Add(new BookstoresListItem(b.id.ToString(), b.address.ToString(), b.cashierTelephone.ToString(), b.managerTelephone.ToString()));
        }
        return itemList;
    }
}
