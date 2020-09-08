using System;
/*
    public string Hiring_date { get; set; }
    public string Dehiring_date { get; set; }
    public string Job_type { get; set; }
    public string Salary { get; set; }
    public string ShiftID { get; set; }
    public string BookstoreID { get; set; }
     */
public class DateTime
{
    int _year;
    int _month;
    int _day;
    public int Year
    {
        get => _year;
        set 
        { 
            if(value < 0 || value > System.DateTime.Now.Year)
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
            if (value < 0 || value >= 13)
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
            if (value < 1 || value >= 30)
            {
                if(this.Month == 2)
                    throw new Exception("Exceeded maximum date of February");
                if(value == 31 && (this.Month == 1 || this.Month == 2 || this.Month == 4 || this.Month == 6 || this.Month == 9 || this.Month == 11))
                    throw new Exception("Exceeded maximum date of 30-day month");
                if(value > 31)
                    throw new Exception("Exceeded maximum possible date");
            }
            _day = value;
        }
    }
    public DateTime(int year, int month, int day)
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