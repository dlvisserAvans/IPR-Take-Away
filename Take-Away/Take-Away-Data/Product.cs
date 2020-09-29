﻿using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Data;

namespace Take_Away_Data
{
    class Product
    {
        private string productName { get; set; }
        private double price { get; set; }
        private ProductType productType { get; set; }

        public Product()
        {
            productName = "AardappelFlap";
            price = 5.0;
            productType = productType.Dessert;
        }

        public Product(string productName, double price, ProductType productType)
        {
            this.productName = productName;
            this.price = price;
            this.productType = productType;
        }


        public override string ToString()
        {
            return "ProductName: " + productName + "\tProductType: " + productType + "\tPrice: " + price;
        }
    }
}
