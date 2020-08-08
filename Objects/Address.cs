class Address
{
    public Address(string city, string street, int houseNumber)
    {
        this.city = city;
        this.houseNumber = houseNumber;
        this.street = street;
    }
    public override string ToString()
    {
        return city + ", " + street + ", " + houseNumber.ToString();
    }
    public string city;
    public string street;
    public int houseNumber;
}
