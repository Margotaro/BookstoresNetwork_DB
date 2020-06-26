using BookStore;

interface IBookstoreListener
{
    Bookstore getBookstore(Id<Bookstore> bookstoreId);
    void bookstoreChanged(Bookstore bookstore);
}