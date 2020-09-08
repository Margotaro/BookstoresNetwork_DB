using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BookStore;

class CatalogueViewList
{
    public ObservableCollection<CatalogueListItem> list { get; set; }
    public CatalogueViewList(List<Book> catalogue)
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
                book.warehouse_quantity.ToString()
            ));
        }
    }
}

