using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Client.Utils;

namespace Take_Away_Data
{
    public class Restaurant : ObservableObject
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return "RestaurantName: " + Name + "\tAddress: " + Address;
        }
    }
}
