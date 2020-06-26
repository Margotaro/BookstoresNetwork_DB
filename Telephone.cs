using System;
using System.Text.RegularExpressions;

class Telephone
{
    public Telephone(string number)
    {
        if (!Regex.Match(number, "^\\d{12}$").Success)
        {
            throw new Exception("Telephone number input does not match standard rules");
        }
        _telephonenumber = number;
    }
    string _telephonenumber;
    public string telephonenumber
    {
        get => this._telephonenumber;
        set
        {
            if (!Regex.Match(value, "^\\d{12}$").Success)
            {
                throw new Exception("Telephone number does not match standard rules");
            }
            _telephonenumber = value; 
        }
    }
}
