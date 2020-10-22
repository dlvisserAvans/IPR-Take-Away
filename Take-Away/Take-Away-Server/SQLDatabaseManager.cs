using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Data;
using MySqlConnector;
using System.Data;

namespace Take_Away_SQLConnection
{
    public class SQLDatabaseManager
    {
        private string databaseName;
        private string connstring;
        private string dbDave = "!\\P8QYii@*Ss*3E@4jMo5aXbXEJP";
        private string dbJanKees = "Jkbiseenjongen";
        private MySqlConnection context { get; set; }

        public SQLDatabaseManager(string databaseName)
        {
            this.databaseName = databaseName;
            connstring = string.Format("Server=localhost; database={0}; UID=root;" + $"password={dbJanKees}", this.databaseName);
            context = new MySqlConnection(connstring);
            context.Open();
        }

        //The server creates a sql command and sends it to the database. When the data has been received succesfully the productlist will be filled.
        public List<Product> GetProductsFromRestaurantIntoList(string restaurantName)
        {
            List<Product> productList = new List<Product>();
            var cmd = new MySqlCommand("SELECT DISTINCT products.productname, products.productprice, products.producttype " +
                                       "FROM products, restaurants, restaurants_products WHERE restaurants.restaurantname = " +
                                       $"(\"{restaurantName}\") AND restaurants.restaurantname = restaurants_products.restaurantname AND products.productname = restaurants_products.productname", context);



            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string productName = reader.GetString(0);
                    double productPrice = reader.GetDouble(1);
                    string productTypeString = reader.GetString(2);
                    ProductType productType = Enum.Parse<ProductType>(productTypeString);
                    productList.Add(new Product { name = productName, price = productPrice, type = productType });
                }
            }
            return productList;
        }

        //The server creates a sql command and sends it to the database. When the data has been received succesfully the restaurantlist will be filled.
        public List<Restaurant> GetAllRestaurantsIntoList()
        {
            List<Restaurant> restaurantList = new List<Restaurant> { };
            var cmd = new MySqlCommand("SELECT DISTINCT * FROM restaurants", context);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string restaurantName = reader.GetString(0);
                    string restaurantAddress = reader.GetString(1);
                    restaurantList.Add(new Restaurant { name = restaurantName, address = restaurantAddress });
                }
            }
            return restaurantList;
        }

        public int GetRestaurantAmount()
        {
            List<Restaurant> restaurants = GetAllRestaurantsIntoList();
            return restaurants.Count;
        }

        public int GetProductsFromRestaurant(string restaurantName)
        {
            List<Product> products = GetProductsFromRestaurantIntoList(restaurantName);
            return products.Count;
        }
    }
}