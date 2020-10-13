using System;
using System.Collections.Generic;
using System.Text;

namespace Take_Away_Data
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostalCode { get; set; }
        public string HouseNumber { get; set; }
        public string FullName
        {
            get
            {
                return String.Format($"{FirstName} {LastName}");
            }
        }

        public override string ToString()
        {
            return "First name: " + FirstName + "\tLast name: " + LastName + "\tPostal Code: " + PostalCode + "\tHousenumber: " + HouseNumber;
        }
    }
}
