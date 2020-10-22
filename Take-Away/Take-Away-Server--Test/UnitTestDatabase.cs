using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Take_Away_Data;
using Take_Away_SQLConnection;

namespace Take_Away_Test
{
    [TestClass]
    public class UnitTestDatabase
    {
        [TestMethod]
        public void TestRestaurantAmount()
        {
            //arrange
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            int expectedRestaurantAmount = 4;

            //act
            int restaurantAmount = databaseManager.GetRestaurantAmount();

            //assert
            Assert.AreEqual(expectedRestaurantAmount, restaurantAmount);
        }

        [TestMethod]
        public void TestProductAmountMacDonalds()
        {
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            int expectedProductAmount = 5;

            //act
            int productAmount = databaseManager.GetProductsFromRestaurant("MacDonalds");

            //assert
            Assert.AreEqual(expectedProductAmount, productAmount);
        }

        [TestMethod]
        public void TestProductAmountHetHoekje()
        {
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            int expectedProductAmount = 6;

            //act
            int productAmount = databaseManager.GetProductsFromRestaurant("Het Hoekje");

            //assert
            Assert.AreEqual(expectedProductAmount, productAmount);
        }

        [TestMethod]
        public void TestProductAmountFlappyItaly()
        {
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            int expectedProductAmount = 6;

            //act
            int productAmount = databaseManager.GetProductsFromRestaurant("Flappy Italy");

            //assert
            Assert.AreEqual(expectedProductAmount, productAmount);
        }

        [TestMethod]
        public void TestProductAmountDominos()
        {
            SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");
            int expectedProductAmount = 5;

            //act
            int productAmount = databaseManager.GetProductsFromRestaurant("Domino's");

            //assert
            Assert.AreEqual(expectedProductAmount, productAmount);
        }
    }
}
