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
        private ObservableCollection<Product> mSelectedProducts;
        private Product mAllSelectedProduct = null;

        public TakeAwayViewModel()
        {
            mAllProducts = new ObservableCollection<Product>
            {
                //code to implement products, but for now some hardcode products :)
                new Product{Name = "Burger", Price = 1.0, Type = ProductType.Burger},
                new Product{Name = "Fries", Price = 2.0, Type = ProductType.Fries},
                new Product{Name = "Milkshake", Price = 3.0, Type = ProductType.Milkshake},
                new Product{Name = "Soda", Price = 4.0, Type = ProductType.Soda},
                new Product{Name = "Dessert", Price = 5.0, Type = ProductType.Dessert}
            };

            mSelectedProducts = new ObservableCollection<Product>();
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

        public ObservableCollection<Product> SelectedProducts
        {
            get
            {
                return mSelectedProducts;
            }
            set
            {
                mSelectedProducts = value;
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
            SelectedProducts.Add(SelectedProduct);
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
