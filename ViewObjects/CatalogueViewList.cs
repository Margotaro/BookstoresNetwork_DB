using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BookStore;

class CatalogueViewList
{
    public ObservableCollection<CatalogueListItem> list { get; set; }
    public CatalogueViewList(List<Book> catalogue)
    {
        list = new ObservableCollection<CatalogueListItem>();
        foreach (var book in catalogue) {
            list.Add(new CatalogueListItem (
                book.id.ToString(),
                book.title.ToString(),
                book.author.ToString(),
                book.publisher.ToString(),
                book.language.ToString(),
                book.genre.ToString())
            );
        }
    }
}

