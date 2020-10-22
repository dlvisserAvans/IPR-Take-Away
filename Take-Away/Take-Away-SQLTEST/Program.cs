using System;
using System.Collections.Generic;
using Take_Away_Data;
using Take_Away_SQLConnection;

namespace Take_Away_SQLTEST
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            List<Receipt> receipts = new List<Receipt>();
            
            
            //List<Product> products = databaseManager.getProductsFromRestaurantIntoList("Het Hoekje");
            List<Restaurant> restaurants = databaseManager.getAllRestaurantsIntoList();

            //foreach (Product p in products)
            //{
            //    Console.WriteLine(p);
            //}
            Console.WriteLine("Gathering all restaurants");
            foreach (Restaurant r in restaurants)
            {
                List<Product> products = databaseManager.getProductsFromRestaurantIntoList(r.restaurantName);
                Console.WriteLine("All products of restaurant: " + r.restaurantName);
                foreach (Product p in products)
                {
                    Console.WriteLine(p);
                }
                receipts.Add(new Receipt { restaurantName = r.restaurantName, buyername = "Dave", buyeraddress = "Het Blok 19", totalPrice = CalculateTotalPrice(products), boughtProducts = products});
            }

            foreach(Receipt r in receipts)
            {
                Console.WriteLine(r);
            }

        }

        public static double CalculateTotalPrice(List<Product> boughtProducts)
        {
            double totalPrice = 0.0;
            foreach (Product p in boughtProducts)
            {
                totalPrice += p.price;
            }
            return totalPrice;
        }


    }
}
