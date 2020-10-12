using System;
using System.Collections.Generic;
using System.Text;

namespace Take_Away_Data
{
    struct Restaurant
    {
        public string restaurantName { get; set; }
        private string restaurantAddress { get; set; }

        public Restaurant(string restaurantName, string restaurantAddress)
        {
            this.restaurantName = restaurantName;
            this.restaurantAddress = restaurantAddress;
        }


        public override string ToString()
        {
            return "RestaurantName: " + restaurantName + "\tAddress: " + restaurantAddress;
        }
    }
}
