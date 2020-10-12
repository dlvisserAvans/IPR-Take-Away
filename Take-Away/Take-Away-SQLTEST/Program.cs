using System;
using System.Collections.Generic;
using Take_Away_Data;

namespace Take_Away_SQLTEST
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            
            
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

            }

        }


    }
}
