using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

namespace BookStore
{
    interface IDAO
    {
        void updateWorker(Worker worker);
        void updateBook(Book book);
        void createWorker(Worker worker);
        IEnumerable<Bookstore> getAllBookstores();
        IEnumerable<Book> getCatalogueContents();
        IEnumerable<Worker> getAllWorkers();
        Bookstore getBookstore(Id<IBookstore> bookstoreId);
    }


}
