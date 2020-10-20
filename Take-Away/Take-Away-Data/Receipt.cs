using System;
using System.Collections.Generic;
using System.Text;

namespace Take_Away_Data
{
    class Receipt
    {
        public string restaurantName { get; set; }
        public string buyername { get; set; }
        public string buyerPostalCode { get; set; }
        public string buyerHouseNumber { get; set; }
        public double totalPrice { get; set; }
        public List<Product> boughtProducts { get; set; }

        public override string ToString()
        {
            string productdata = countedProducts();

            return $"Thank you for your order! The ordered food from {restaurantName} will arrive shortly\n" + 
                   $"\nName: {buyername}" + 
                   $"\nPostal Code: {buyerPostalCode}" + 
                   $"\nHouse number: {buyerHouseNumber}" +
                   $"\n====================================" + 
                   $"{productdata}" +
                   $"\n====================================" +
                   $"\nThe total price of your order is : {totalPrice:##0.00}";
        }

        private string countedProducts()
        {
            string productData = "";
            while (boughtProducts.Count > 0)
            {
                int amountInList = 1;
                Product product = boughtProducts[boughtProducts.Count - 1];
                for (int j = (boughtProducts.Count - 1); j > 0; j--)
                {
                    if (product.Name.Equals(boughtProducts[j - 1].Name) && product.Price.Equals(boughtProducts[j - 1].Price))
                    {
                        amountInList++;
                        boughtProducts.Remove(boughtProducts[j - 1]);
                    }
                }
                productData += $"\n{amountInList} {product.Name} {product.Price * amountInList}";
                boughtProducts.Remove(product);
            }

            return productData;
        }
    }
}
