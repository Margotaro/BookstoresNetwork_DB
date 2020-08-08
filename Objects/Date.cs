using System;
/*
    public string Hiring_date { get; set; }
    public string Dehiring_date { get; set; }
    public string Job_type { get; set; }
    public string Salary { get; set; }
    public string ShiftID { get; set; }
    public string BookstoreID { get; set; }
     */
class Date
{
    int _year;
    int _month;
    int _day;
    public int Year
    {
        get => _year;
        set 
        { 
            if(value < 0 || value >= DateTime.Now.Year)
            { 
                throw new Exception("Year is not correct"); 
            }
            _year = value;
        }        
    }
    public int Month
    {
        get => _month;
        set
        {
            if (value < 1 || value >= DateTime.Now.Month)
            {
                throw new Exception("Month is not correct");
            }
            _month = value;
        }
    }
    public int Day
    {
        get => _day;
        set
        {
            if (value < 1 || value >= DateTime.Now.Day)
            {
                throw new Exception("Day is not correct");
            }
            _day = value;
        }
    }
    public Date(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }
    public override string ToString()
    {
        return Year.ToString() + "-" + Month.ToString() + "-" + Day.ToString();
    }
}