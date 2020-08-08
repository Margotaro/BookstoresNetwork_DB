namespace BookStore
{
    class BookFactory
    {

        public BookFactory(BookChangeManager changeManager)
        {
            this.changeManager = changeManager;
        }

        BookChangeManager changeManager;

        public IBook makeBook(string id, string title, string author, string publisher, string language, string genre)
        {
            return new Book(changeManager, new Id<IBook>(id), title, author, publisher, language, genre);
        }

    }
}
