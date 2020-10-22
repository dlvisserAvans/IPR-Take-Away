using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Data;
using Take_Away_SQLConnection;

namespace Take_Away_Server
{
    [TestClass()]
    public class TakeAwayServerTest
    {
        [TestMethod()]
        public void restaurantTest()
        {
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            List<Restaurant> restaurants =  databaseManager.GetAllRestaurantsIntoList();
            
            int expectedRestaurants = 4;
            int restaurantAmount = restaurants.Count;

            Assert.AreEqual(expectedRestaurants, restaurantAmount);

        }



    }
}
