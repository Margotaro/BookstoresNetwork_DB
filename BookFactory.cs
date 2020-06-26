namespace BookStore
{
    class BookFactory
    {

        public BookFactory(BookChangeManager changeManager)
        {
            this.changeManager = changeManager;
        }

        BookChangeManager changeManager;

        public IBook makeBook(string title, string author, string publisher, string language, string genre)
        {
            return new Book(changeManager, title, author, publisher, language, genre);
        }

    }
}
