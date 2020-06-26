namespace BookStore
{
    class BookChangeManager : IBookListener
    {

        public BookChangeManager(IDAO dao)
        {
            this.dao = dao;
        }

        IDAO dao;

        public void bookChanged(Book book)
        {
            dao.updateBook(book);
        }
    }
}
