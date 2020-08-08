namespace BookStore
{
    class Book : IPropertyListener, IBook
    {

        public Book(IBookListener listener, Id<IBook> id, string title, string author, string publisher, string language, string genre)
        {
            this.listener = listener;

            _id = new Property<Id<IBook>>(id, this);
            _title = new Property<string>(title, this);
            _author = new Property<string>(author, this);
            _publisher = new Property<string>(publisher, this);
            _language = new Property<string>(language, this);
            _genre = new Property<string>(genre, this);
        }

        IBookListener listener;

        Property<Id<IBook>> _id;
        Property<string> _title;
        Property<string> _author;
        Property<string> _publisher;
        Property<string> _language;
        Property<string> _genre;

        public Id<IBook> id
        {
            get => _id.value;
        }
        public string title
        {
            get => _title.value;
            set => _title.value = value;
        }

        public string author
        {
            get => _author.value;
            set => _author.value = value;
        }
        public string publisher
        {
            get => _publisher.value;
            set => _publisher.value = value;
        }
        public string language
        {
            get => _language.value;
            set => _language.value = value;
        }
        public string genre
        {
            get => _genre.value;
            set => _genre.value = value;
        }
        public void propertyChanged()
        {
            listener?.bookChanged(this);
        }
    }
}
