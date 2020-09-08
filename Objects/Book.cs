using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;

namespace BookStore
{
    public class Book : IPropertyListener, IBook
    {

        public Book(IBookListener listener, Id<IBook> id, string title, string author, string publisher, string language, IEnumerable genre, double electronic_version_cost = 0, string electronic_version_storage_route = "", double reatil_printed_version_cost = 0, double wholesale_printed_version_cost = 0, int warehouse_quantity = 0)
        {
            this.listener = listener;

            //id's
            _id = new Property<Id<IBook>>(id, this);

            //basic
            _title = new Property<string>(title, this);
            _author = new Property<string>(author, this);
            _publisher = new Property<string>(publisher, this);
            _language = new Property<string>(language, this);
            _genre = new Property<IEnumerable>(genre, this);

            //electronic
            _electronic_version_cost = new Property<double>(electronic_version_cost, this);
            _electronic_version_storage_route = new Property<string>(electronic_version_storage_route, this);

            //printed
            _warehouse_quantity = new Property<int>(warehouse_quantity, this);
            _reatil_printed_version_cost = new Property<double>(reatil_printed_version_cost, this);
            _wholesale_printed_version_cost = new Property<double>(wholesale_printed_version_cost, this);

            Languages = new List<string> { "english", "russian", "french", "chinese", "portugese", "italian", "japanese", "korean" };
        }

        IBookListener listener;
        public static List<string> Languages;

        Property<Id<IBook>> _id;
        Property<string> _title;
        Property<string> _author;
        Property<string> _publisher;
        Property<string> _language;
        Property<IEnumerable> _genre;
        Property<double> _electronic_version_cost;
        Property<string> _electronic_version_storage_route;
        Property<double> _reatil_printed_version_cost;
        Property<double> _wholesale_printed_version_cost;
        Property<int> _warehouse_quantity;

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
        public IEnumerable genre
        {
            get => _genre.value;
            set => _genre.value = value;
        }
        public double electronic_version_cost
        {
            get => _electronic_version_cost.value;
            set => _electronic_version_cost.value = value;
        } 
        public string electronic_version_storage_route
        {
            get => _electronic_version_storage_route.value;
            set => _electronic_version_storage_route.value = value;
        }
        public double reatil_printed_version_cost
        {
            get => _reatil_printed_version_cost.value;
            set => _reatil_printed_version_cost.value = value;
        }
        public double wholesale_printed_version_cost
        {
            get => _wholesale_printed_version_cost.value;
            set => _wholesale_printed_version_cost.value = value;
        } 
        public int warehouse_quantity
        {
            get => _warehouse_quantity.value;
            set => _warehouse_quantity.value = value;
        }
        public void propertyChanged()
        {
            listener?.bookChanged(this);
        }
    }
}
