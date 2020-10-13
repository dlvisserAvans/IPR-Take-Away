using System;
using System.Collections.Generic;
using System.Text;

namespace Take_Away_Data
{
    class Receipt
    {
        public string restaurantName { get; set; }
        public string buyername { get; set; }
        public string buyeraddress { get; set; }
        public double totalPrice { get; set; }
        public List<Product> boughtProducts { get; set; }

        public override string ToString()
        {
            string productdata = "";

            foreach (Product p in boughtProducts)
            {
                productdata += $"\n{p.Name}";
            }


            return $"Thankyou for your order! The ordered food from {restaurantName} will arrive shortly" + 
                   $"\nName: {buyername}" + 
                   $"\nAddress: {buyeraddress}" + 
                   $"\n====================================" + 
                   $"{productdata}" +
                   $"\n====================================" +
                   $"\nThe total price of their order is : {totalPrice}";
        }
    }
}
