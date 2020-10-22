using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Take_Away_Client.ViewModel;
using Take_Away_Data;

namespace Take_Away_Client
{
    [TestClass()]
    public class TakeAwayClientTest
    {
        [TestMethod()]
        public void countHoekjeItemsTest()
        {
            TakeAwayViewModel tw = new TakeAwayViewModel();
            Restaurant restaurant = tw.restaurants[0];
            tw.selectedRestaurant = restaurant;

            int productAmount = tw.products.Count;
            int expectedAmount = 6;

            Assert.AreEqual(expectedAmount, productAmount);
        
        }
    }
}
