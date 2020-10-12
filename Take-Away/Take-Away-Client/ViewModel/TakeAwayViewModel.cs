using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Input;
using Take_Away_Client.Utils;
using Take_Away_Data;

namespace Take_Away_Client.ViewModel
{
    class TakeAwayViewModel : ObservableObject
    {
        private ObservableCollection<Product> mAllProducts;
        private Product mAllSelectedProduct = null;

        public TakeAwayViewModel()
        {
            mAllProducts = new ObservableCollection<Product>
            {
                //code to implement products, but for now some hardcode products :)
                new Product{productName = "Burger", productPrice = 1.0, productType = ProductType.Burger},
                new Product{productName = "Fries", productPrice = 2.0, productType = ProductType.Fries},
                new Product{productName = "Milkshake", productPrice = 3.0, productType = ProductType.MilkShake},
                new Product{productName = "Soda", productPrice = 4.0, productType = ProductType.Soda},
                new Product{productName = "Dessert", productPrice = 5.0, productType = ProductType.Dessert}
            };          
        }

        public ObservableCollection<Product> Products
        {
            get 
            { 
                return mAllProducts; 
            }
            set
            {
                mAllProducts = value;
                NotifyPropertyChanged();
            }
        }

        public Product SelectedProduct
        {
            get 
            { 
                return mAllSelectedProduct; 
            }
            set
            {
                if (mAllSelectedProduct != value)
                {
                    if (mAllSelectedProduct == null)
                    {
                        mAllSelectedProduct = new Product();
                    }
                    mAllSelectedProduct = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ICommand mAddCommand;
        public ICommand AddCommand
        {
            get
            {
                if(mAddCommand == null)
                {
                    mAddCommand = new RelayCommand(
                        param => AddProduct(), 
                        param => (true));
                }
                return mAddCommand;
            }
        }

        private void AddProduct()
        {
            
        }

        private ICommand mDeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if(mDeleteCommand == null)
                {
                    mDeleteCommand = new RelayCommand(
                        param => DeleteProduct(),
                        param => (SelectedProduct != null));
                }
                return mDeleteCommand;
            }
        }

        private void DeleteProduct()
        {

        }
    }
}
