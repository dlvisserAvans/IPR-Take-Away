using System;
using System.Collections.Generic;
using System.Text;

namespace Take_Away_Data
{
    public class User
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string postalCode { get; set; }
        public string houseNumber { get; set; }
        public string fullName
        {
            get
            {
                return String.Format($"{firstName} {lastName}");
            }
        }

        public override string ToString()
        {
            return "First name: " + firstName + "\tLast name: " + lastName + "\tPostal Code: " + postalCode + "\tHousenumber: " + houseNumber;
        }
    }
}
