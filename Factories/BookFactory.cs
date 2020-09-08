using System.Collections;

namespace BookStore
{
    class BookFactory
    {

        public BookFactory(BookChangeManager changeManager)
        {
            this.changeManager = changeManager;
        }

        BookChangeManager changeManager;

        public IBook makeBook(string id, string title, string author, string publisher, string language, IEnumerable genres, double electronic_version_cost = 0, string electronic_version_storage_route = "", double reatil_printed_version_cost = 0, double wholesale_printed_version_cost = 0, int warehouse_quantity = 0)
        {
            return new Book(changeManager, new Id<IBook>(id), title, author, publisher, language, genres, electronic_version_cost, electronic_version_storage_route, reatil_printed_version_cost, wholesale_printed_version_cost, warehouse_quantity);
        }

    }
}
