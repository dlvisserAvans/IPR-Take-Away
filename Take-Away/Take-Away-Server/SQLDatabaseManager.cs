using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Data;
using MySqlConnector;
using System.Data;

namespace Take_Away_SQLConnection
{
    class SQLDatabaseManager
    {
        private string databaseName;
        private string connstring;

        private string DBDave = "!\\P8QYii@*Ss*3E@4jMo5aXbXEJP";
        private string DBJK = "Jkbiseenjongen";
        private MySqlConnection context { get; set; }
       
        public SQLDatabaseManager(string databaseName)
        {
            this.databaseName = databaseName;
            connstring = string.Format("Server=localhost; database={0}; UID=root;" + $"password={DBJK}", this.databaseName);
            context = new MySqlConnection(connstring);
            context.Open();
        }

        public List<Product> getProductsFromRestaurantIntoList(string restaurantName)
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
                    productList.Add(new Product { Name = productName, Price = productPrice, Type = productType});
                }
            }
                   
            return productList;
        }

        public List<Restaurant> getAllRestaurantsIntoList()
        {
            List<Restaurant> restaurantList = new List<Restaurant> { };
            var cmd = new MySqlCommand("SELECT DISTINCT * FROM restaurants", context);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string restaurantName = reader.GetString(0);
                    string restaurantAddress = reader.GetString(1);
                    restaurantList.Add(new Restaurant { Name = restaurantName, Address = restaurantAddress });
                }
            }
            return restaurantList;
        }
    }
}
