namespace BookStore
{
    class BookstoreChangeManager : IBookstoreListener {

        public BookstoreChangeManager(IDAO dao)
        {
            this.dao = dao;
        }

        IDAO dao;
        public Bookstore getBookstore(Id<Bookstore> bookstoreId)
        {
            return dao.getBookstore(bookstoreId);
        }

        public void bookstoreChanged(Bookstore bookstore)
        {
            throw new System.NotImplementedException();
        }
    }
    
}
