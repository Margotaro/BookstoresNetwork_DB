using System.Collections;
using System.Collections.Generic;

namespace BookStore
{
    public interface IBookListener
    {
        void bookChanged(Book book);
        void bookCreated(BookViewModel book);
        IEnumerable<Book> bookBeingSearched(string title, string author, string publisher, string language, IEnumerable genres);
    }
}
