namespace BookStore
{
    interface IDAO
    {
        void updateWorker(Worker worker);
        void updateBook(Book book);
        void createWorker(Worker worker);
        Bookstore getBookstore(Id<Bookstore> id);
    }


}
