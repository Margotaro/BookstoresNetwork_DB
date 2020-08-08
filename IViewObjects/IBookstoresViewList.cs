using System.Collections.Generic;
using System.Collections.ObjectModel;
using BookStore;
interface IBookstoresViewList
{
    ObservableCollection<BookstoresListItem> list { get; set; }

}
