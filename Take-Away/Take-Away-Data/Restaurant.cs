using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Client.Utils;

namespace Take_Away_Data
{
    public class Restaurant : ObservableObject
    {
        public string name { get; set; }
        public string address { get; set; }

        public override string ToString()
        {
            return "RestaurantName: " + name + "\tAddress: " + address;
        }
    }
}
