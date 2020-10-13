using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Data;
using Take_Away_Client.Utils;

namespace Take_Away_Data
{
    public class Product : ObservableObject
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public ProductType Type { get; set; }

        public override string ToString()
        {
            return "ProductName: " + Name + "\tProductType: " + Type + "\tPrice: " + Price;
        }
    }
}
