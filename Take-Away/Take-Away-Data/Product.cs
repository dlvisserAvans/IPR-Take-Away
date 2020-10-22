using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Data;
using Take_Away_Client.Utils;

namespace Take_Away_Data
{
    public class Product : ObservableObject
    {
        public string name { get; set; }
        public double price { get; set; } 
        public ProductType type { get; set; }

        public override string ToString()
        {
            return "ProductName: " + name + "\tProductType: " + type + "\tPrice: " + price;
        }
    }
}
