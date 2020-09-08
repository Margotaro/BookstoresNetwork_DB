using BookStore;

public interface IBookstore
{
    Address address { get; }
    Telephone cashierTelephone { get; }
    Telephone managerTelephone { get; }
    Id<IBookstore> id { get; }
}