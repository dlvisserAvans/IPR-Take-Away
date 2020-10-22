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
            string productdata = CountedProducts();

            return $"Thank you for your order! The ordered food from {restaurantName} will arrive shortly\n" +
                   $"\nName: {buyername}" +
                   $"\nPostal Code: {buyerPostalCode}" +
                   $"\nHouse number: {buyerHouseNumber}" +
                   $"\n====================================" +
                   $"{productdata}" +
                   $"\n====================================" +
                   $"\nThe total price of your order is : {totalPrice:##0.00}";
        }

        // This method loops through the boughtproducts list to see how many items of a product are ordered by the customer.
        // It then puts these amounts under the same product name so there will not be any duplicates on the receipt.
        private string CountedProducts()
        {
            string productData = "";
            while (boughtProducts.Count > 0)
            {
                int amountInList = 1;
                Product product = boughtProducts[boughtProducts.Count - 1];
                for (int j = (boughtProducts.Count - 1); j > 0; j--)
                {
                    if (product.name.Equals(boughtProducts[j - 1].name) && product.price.Equals(boughtProducts[j - 1].price))
                    {
                        amountInList++;
                        boughtProducts.Remove(boughtProducts[j - 1]);
                    }
                }
                productData += $"\n{amountInList} {product.name} {product.price * amountInList}";
                boughtProducts.Remove(product);
            }
            return productData;
        }
    }
}