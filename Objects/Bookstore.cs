namespace BookStore
{
    class Bookstore : IBookstore, IPropertyListener
    {

        public Bookstore(IBookstoreListener listener, Id<IBookstore> id, Address address, Telephone cashierTelephone, Telephone managertelephone)
        {
            this.listener = listener;
            _id = new Property<Id<IBookstore>>(id, this);
            _address = new Property<Address>(address, this);
            _cashierTelephone = new Property<Telephone>(cashierTelephone, this);
            _managerTelephone = new Property<Telephone>(managertelephone, this);
        }

        IBookstoreListener listener;

        Property<Address> _address;
        Property<Telephone> _cashierTelephone;
        Property<Telephone> _managerTelephone;
        Property<Id<IBookstore>> _id;
        public Address address { get => _address.value; } 
        public Telephone cashierTelephone { get => _cashierTelephone.value; } 
        public Telephone managerTelephone { get => _managerTelephone.value; }
        public Id<IBookstore> id { get => _id.value; } 
        public void propertyChanged()
        {
            listener?.bookstoreChanged(this);
        }
    }
    
}
