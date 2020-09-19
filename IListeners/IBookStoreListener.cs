using BookStore;

public interface IBookstoreListener
{
    Bookstore getBookstore(Id<IBookstore> bookstoreId);
    void bookstoreChanged(Bookstore bookstore);
}