namespace BookStore
{
    public class Receipt : IReceipt, IPropertyListener
    {

        public Receipt(IReceiptListener listener, Id<IReceipt> id, Id<IWorker> cashier_id, int cost, DateTime purchase_timestamp)
        {
            this.listener = listener;
            _id = new Property<Id<IReceipt>>(id, this);
            _cashier_id = new Property<Id<IWorker>>(cashier_id, this);
            _cost = new Property<int>(cost, this);
            _purchase_timestamp = new Property<DateTime>(purchase_timestamp, this);
        }

        IReceiptListener listener;

        Property<Id<IReceipt>> _id;
        Property<Id<IWorker>> _cashier_id;
        Property<int> _cost;
        Property<DateTime> _purchase_timestamp;
        public int cost { get => _cost.value; set => _cost.value = value; }
        public DateTime purchase_timestamp { get => _purchase_timestamp.value; set => _purchase_timestamp.value = value;  }
        public Id<IReceipt> id { get => _id.value; }
        public Id<IWorker> cashier_id { get => _cashier_id.value; }
        public void propertyChanged()
        {
            //listener?.receiptChanged(this);
        }
    }

    public interface IReceiptListener
    {
        //void bookChanged(Book book);
        //void bookCreated(BookViewModel book);
        //IEnumerable<Book> bookBeingSearched(string title, string author, string publisher, string language, IEnumerable genres);
    }
}
