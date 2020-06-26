namespace BookStore
{
    enum Position
    {
        cashier, manager
    }
    class Worker : IPropertyListener, IWorker
    {

        public Worker(IWorkerListener listener, string name, Position position, double salary, WorkingHours hours, string login, string password, string id)
        {
            this.listener = listener;

            _name = new Property<string>(name, this);
            _position = new Property<Position>(position, this);
            _salary = new Property<double>(salary, this);
            _hours = new Property<WorkingHours>(hours, this);
            _login = new Property<string>(login, this);
            _password = new Property<string>(password, this);
            _id = new Property<Id<IWorker>>(new Id<IWorker>(id), this);
            this.listener.workerCreated(this);
        }

        IWorkerListener listener;

        Property<string> _name;
        Property<Position> _position;
        Property<double> _salary;
        Property<WorkingHours> _hours;
        Property<string> _login;
        Property<string> _password;
        Property<Id<IWorker>> _id;
        public string name
        {
            get => _name.value;
            set => _name.value = value;
        }

        public Position position
        {
            get => _position.value;
            set => _position.value = value;
        }
        public double salary
        {
            get => _salary.value;
            set => _salary.value = value;
        }
        public WorkingHours hours
        {
            get => _hours.value;
            set => _hours.value = value;
        }
        public string login
        {
            get => _login.value;
            set => _login.value = value;
        }
        public string password
        {
            get => _password.value;
            set => _password.value = value;
        }
        public Id<IWorker> id
        {
            get => _id.value;
            set  => _id.value = value;
        }

        public void  propertyChanged()
        {
            listener?.workerChanged(this);
        }
    }
    class Bookstore : IBookstore, IPropertyListener
    {

        public Bookstore(IBookstoreListener listener, Address address, Telephone cashierTelephone, Telephone managertelephone)
        {
            this.listener = listener;
            _address = new Property<Address>(address, this);
            _cashierTelephone = new Property<Telephone>(cashierTelephone, this);
            _managertelephone = new Property<Telephone>(managertelephone, this);
        }

        IBookstoreListener listener;

        Property<Address> _address;
        Property<Telephone> _cashierTelephone;
        Property<Telephone> _managertelephone;
        Property<Id<IBookstore>> _id;
        public Address address {  get;  }
        public Telephone cashierTelephone { get; }
        public Telephone managerTelephone { get; }
        public Id<IBookstore> id { get; }
        public void propertyChanged()
        {
            listener?.bookstoreChanged(this);
        }
    }
    
}
