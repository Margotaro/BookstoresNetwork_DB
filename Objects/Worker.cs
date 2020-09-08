using System.Collections.Generic;

namespace BookStore
{
    public enum Position
    {
        cashier, manager, deliverer
    }
    public class Worker : IPropertyListener, IWorker
    {

        public Worker(IWorkerListener listener, string name, Position position, double salary, WorkingHours hours, string login, string password, string id, DateTime hiringdate, DateTime dehiringdate, Bookstore bookstore)
        {
            this.listener = listener;

            _name = new Property<string>(name, this);
            _position = new Property<Position>(position, this);
            _salary = new Property<double>(salary, this);
            _hours = new Property<WorkingHours>(hours, this);
            _login = new Property<string>(login, this);
            _password = new Property<string>(password, this);
            _id = new Property<Id<IWorker>>(new Id<IWorker>(id), this);
            _hiring_date = new Property<DateTime>(hiringdate, this);
            _dehiring_date = new Property<DateTime>(dehiringdate, this);
            _working_place = new Property<Bookstore>(bookstore, this);
            //this.listener.workerCreated(this);
        }

        IWorkerListener listener;

        Property<string> _name;
        Property<Position> _position;
        Property<double> _salary;
        Property<WorkingHours> _hours;
        Property<string> _login;
        Property<string> _password;
        Property<Id<IWorker>> _id;
        Property<DateTime> _hiring_date;
        Property<DateTime> _dehiring_date;
        Property<Bookstore> _working_place;

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
        public DateTime hiring_date
        {
            get => _hiring_date.value;
            set => _hiring_date.value = value;
        }
        public DateTime dehiring_date
        {
            get => _dehiring_date.value;
            set => _dehiring_date.value = value;
        }
        public Bookstore working_place
        {
            get => _working_place.value;
            set => _working_place.value = value;

        }
        public void  propertyChanged()
        {
            listener?.workerChanged(this);
        }
    }    
}
