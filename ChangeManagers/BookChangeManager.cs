using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace BookStore
{
    public class BookChangeManager : IBookListener
    {

        public BookChangeManager(IDAO dao, IBookstoreWindow window)
        {
            this.dao = dao;
            this.window = window;
        }

        IDAO dao;
        IBookstoreWindow window;

        public void bookChanged(Book book)
        {
            dao.updateBook(book);
        }
        public void bookCreated(BookViewModel bvM)
        {
            var cachedBooks = dao.CachedBooks;
            foreach(var b in cachedBooks)
            {
                if(bvM.title == b.title && bvM.author == b.author && bvM.publisher == b.publisher && bvM.language == b.language)
                { return; }
            }
            dao.createBook(bvM.title, bvM.author, bvM.publisher, bvM.language, bvM.genre, bvM.electronic_ver_cost, bvM.electronic_ver_storage_route,
                           bvM.retail_printed_ver_cost, bvM.wholesail_printed_ver_cost, bvM.warehouse_printed_ver_quantity);
            window.dataGridBookUpdate();
        }
        public IEnumerable<Book> bookBeingSearched(string title, string author, string publisher, string language = "", IEnumerable genres = null)
        {
            var found_books = dao.findBooks(title, author, publisher, language, genres);
            window.dataGridBookUpdate();
            return found_books;
        }
        public void bookDeleted(BookViewModel bvM)
        {
            var cachedBooks = dao.CachedBooks;
            foreach (var b in cachedBooks)
            {
                if (bvM.title == b.title && bvM.author == b.author && bvM.publisher == b.publisher && bvM.language == b.language)
                {
                    var book_id = b.id;
                    dao.deleteBook(book_id);
                    window.dataGridBookUpdate();
                    return; 
                }
            }

        }
    }
}
