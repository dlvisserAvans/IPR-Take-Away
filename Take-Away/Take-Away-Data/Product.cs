using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Client.Utils;
using Take_Away_Data;

namespace Take_Away_Data
{
    public class Product : ObservableObject
    {
        public string productName { get; set; }
        public double productPrice { get; set; }
        public ProductType productType { get; set; }

        public override string ToString()
        {
            return "ProductName: " + productName + "\tProductType: " + productType + "\tPrice: " + price;
        }
    }
}
