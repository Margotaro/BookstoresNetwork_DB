public class StaffListItem
{
    public string EmployeeID { get; set; }
    public string Full_name { get; set; }
    public string Hiring_date { get; set; }
    public string Dehiring_date { get; set; }
    public string Job_type { get; set; }
    public string Salary { get; set; }
    public string ShiftID { get; set; }
    public string BookstoreID { get; set; }
    public StaffListItem(string id, string fullname, string hiringdate, string dehiringdate, string jobtype, string salary, string shiftid, string bookstoreid)
    {
        EmployeeID = id;
        Full_name = fullname;
        Hiring_date = hiringdate;
        Dehiring_date = dehiringdate;
        Job_type = jobtype;
        Salary = salary;
        ShiftID = shiftid;
        BookstoreID = bookstoreid;
    }

}