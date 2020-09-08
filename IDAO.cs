using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

namespace BookStore
{
    public interface IDAO
    {
        List<Bookstore> CachedBookstores { get; }
        List<Worker> CachedWorkers { get; }
        List<Book> CachedBooks { get; }
        void updateWorker(Worker worker);
        void updateBook(Book book);
        void createWorker(string name, double salary, Position position, string login, string password, WorkingHours hours, DateTime hiring_date, Bookstore working_place);
        void createBook(string title, string author, string publisher, string language, IEnumerable genres,
                               string electronic_ver_cost, string electronic_ver_storage_route,
                               string retail_printed_ver_cost, string wholesail_printed_ver_cost,
                               string warehouse_printed_ver_quantity);
        IEnumerable<Bookstore> getAllBookstores();
        IEnumerable<Book> getCatalogueContents();
        IEnumerable<Worker> getAllWorkers();
        Bookstore getBookstore(Id<IBookstore> bookstoreId);
        void deleteBook(Id<IBook> book_id);
        void fireWorker(Id<IWorker> id, DateTime firingDate);
        IEnumerable<Book> findBooks(string title, string author, string publisher, string language, IEnumerable genres);

        //void setDiscount
    }


}
