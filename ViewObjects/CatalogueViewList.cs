using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using BookStore;

class CatalogueViewList
{
    public ObservableCollection<CatalogueListItem> list { get; set; }
    public CatalogueViewList(List<Book> catalogue, List<Bookstore> bookstores, Dictionary<Id<IBook>, List<Tuple<Id<IBookstore>, int>>> bookstoresPerBook)
    {
        list = new ObservableCollection<CatalogueListItem>();
        foreach (var book in catalogue) {
            var genresList = new List<string>();
            foreach (var genre in book.genre)
            {
                if (genre is string)
                {
                    genresList.Add(genre as string);
                }
            }
        var genres = String.Join(",", genresList);

        var bookstores_list = "";

        if (bookstoresPerBook.ContainsKey(book.id))
        {
            foreach (var t_bookstore_q in bookstoresPerBook[book.id])
            {
                var address = "";
                foreach(var i in bookstores)
                {
                    if(i.id.ToString() == t_bookstore_q.Item1.ToString())
                    {
                            address = i.address.ToString();
                    }
                }
                bookstores_list += "[ магазин: " + t_bookstore_q.Item1.ToString() + "; адрес: " + address + "; количество единиц: " + t_bookstore_q.Item2.ToString() + "; ]";
            }
        }
        else
        {
                bookstores_list = "Нет в наличии печатных версий";
        }
    
        list.Add(new CatalogueListItem (
            book.id.ToString(),
            book.title.ToString(),
            book.author.ToString(),
            book.publisher.ToString(),
            book.language.ToString(),
            genres,
            book.electronic_version_cost.ToString(),
            book.electronic_version_storage_route,
            book.wholesale_printed_version_cost.ToString(),
            book.reatil_printed_version_cost.ToString(),
            book.warehouse_quantity.ToString(),
            bookstores_list
        ));
        }

    }
}

