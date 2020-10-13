using Newtonsoft.Json;
using Swordfish.NET.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Take_Away_Client.Utils;
using Take_Away_Data;

namespace Take_Away_Client.ViewModel
{
    class TakeAwayViewModel : ObservableObject
    {
        private ConcurrentObservableCollection<Product> mAllProducts;
        private ConcurrentObservableCollection<Restaurant> mAllRestaurants;

        private ObservableCollection<Product> mSelectedProducts;

        private Product mAllSelectedProduct = null;
        private Product mChosenSelectedProduct = null;
        private Restaurant mChosenRestaurant = null;

        private TcpClient client;
        private NetworkStream networkStream;
        private byte[] buffer = new byte[1024];
        private string totalBuffer;
        private bool connected = false;

        private string username = "JKB";
        private string password = "1234";
        private int mProductAmount = 1;

        private User user;
        private string mFirstName;
        private string mLastName;
        private string mPostalCode;
        private string mHouseNumber;

        public TakeAwayViewModel()
        {
            mAllProducts = new ConcurrentObservableCollection<Product>();
            mAllRestaurants = new ConcurrentObservableCollection<Restaurant>();

            user = new User();

            client = new TcpClient();
            client.BeginConnect("localhost", 12345, new AsyncCallback(OnConnect), null);
            while (!connected) { }
            Write("requestRestaurant");

            mSelectedProducts = new ObservableCollection<Product>();
        }

        public void OnConnect(IAsyncResult ar)
        {
            client.EndConnect(ar);
            networkStream = client.GetStream();
            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            connected = true;
            Write($"login");
        }

        public void OnRead(IAsyncResult ar)
        {
            int recievedBytes = networkStream.EndRead(ar);
            string recievedText = Encoding.ASCII.GetString(buffer, 0, recievedBytes);
            totalBuffer += recievedText;

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));
                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                string[] packetData = Regex.Split(packet, "\r\n");
                handleData(packetData);
            }
            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        public void handleData(string[] packetData)
        {
            switch (packetData[0])
            {
                case "login": //message type 'login'
                    if (packetData[1] == "ok")
                    {
                        Console.WriteLine("Logged in");
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                    break;
                case "sendOrder": //message type 'sendOrder'

                    // Code to recieve the order of the customer
                    break;
                case "requestProducts":
                    dynamic productJson = packetData[1];
                    List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productJson);

                    Task.Run(() => Parallel.ForEach(products, product =>
                    {
                        mAllProducts.Add(new Product { Name = product.Name, Price = product.Price, Type = product.Type});
                    }));
                    break;
                case "requestRestaurant":
                    dynamic restaurantJson = packetData[1];
                    List<Restaurant> restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(restaurantJson);

                    Task.Run(() => Parallel.ForEach(restaurants, restaurant =>
                    {
                        mAllRestaurants.Add(new Restaurant { Name = restaurant.Name, Address = restaurant.Address });
                    }));
                    break;
            }
        }

        public void Write(string data)
        {
            var dataAsBytes = Encoding.ASCII.GetBytes(data + "\r\n\r\n");
            networkStream.Write(dataAsBytes, 0, dataAsBytes.Length);
            networkStream.Flush();
        }

        public string FirstName
        {
            get
            {
                return mFirstName;
            }
            set
            {
                mFirstName = value;
                NotifyPropertyChanged();
                user.FirstName = mFirstName;
            }
        }

        public string LastName
        {
            get
            {
                return mLastName;
            }
            set
            {
                mLastName = value;
                NotifyPropertyChanged();
                user.LastName = mLastName;
            }
        }

        public string PostalCode
        {
            get
            {
                return mPostalCode;
            }
            set
            {
                mPostalCode = value;
                NotifyPropertyChanged();
                user.PostalCode = mPostalCode;
            }
        }

        public string HouseNumber
        {
            get
            {
                return mHouseNumber;
            }
            set
            {
                mHouseNumber = value;
                NotifyPropertyChanged();
                user.HouseNumber = mHouseNumber;
            }
        }
        
        public int productAmount
        {
            get
            {
                return mProductAmount;
            }
            set
            {
                mProductAmount = value;
                NotifyPropertyChanged();
            }
        }

        public ConcurrentObservableCollection<Product> Products
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

        public ConcurrentObservableCollection<Restaurant> Restaurants
        {
            get
            {
                return mAllRestaurants;
            }
            set
            {
                mAllRestaurants = value;
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

        public Restaurant SelectedRestaurant
        {
            get
            {
                return mChosenRestaurant;
            }
            set
            {
                if(mChosenRestaurant != value)
                {
                    if(mChosenRestaurant == null)
                    {
                        mChosenRestaurant = new Restaurant();
                    }
                    mChosenRestaurant = value;
                    mAllProducts.Clear();
                    mSelectedProducts.Clear();
                    Write($"requestProducts\r\n{mChosenRestaurant.Name}");
                    NotifyPropertyChanged();
                }
            }
        }

        public Product AllSelectedProduct
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

        public Product ChosenSelectedProduct
        {
            get
            {
                return mChosenSelectedProduct;
            }
            set
            {
                if(mChosenSelectedProduct != value)
                {
                    if (mChosenSelectedProduct == null)
                    {
                        mChosenSelectedProduct = new Product();
                    }
                    mChosenSelectedProduct = value;
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
            //if product exists, add just the amount
            for (int i = 0; i < productAmount; i++)
            {
                SelectedProducts.Add(AllSelectedProduct);
            }
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
                        param => (ChosenSelectedProduct != null));
                }
                return mDeleteCommand;
            }
        }

        private void DeleteProduct()
        {
            SelectedProducts.Remove(ChosenSelectedProduct);
        }

        private ICommand mSendCommand;
        public ICommand SendCommand
        {
            get
            {
                if(mSendCommand == null)
                {
                    mSendCommand = new RelayCommand(
                        param => SendProducts(),
                        param => (true));
                }
                return mSendCommand;
            }
        }

        private void SendProducts()
        {
            string list = JsonConvert.SerializeObject(SelectedProducts);
            string userJson = JsonConvert.SerializeObject(user);
            Write($"sendOrder\r\n{list}\r\n{userJson}");
            SelectedProducts.Clear();
        }

        private ICommand mImportCommand;
        public ICommand ImportCommand
        {
            get
            {
                if (mImportCommand == null)
                {
                    mImportCommand = new RelayCommand(
                        (dialogType) => {
                            var dialog = Activator.CreateInstance(dialogType as Type) as IFileDialogWindow;
                            var fileNames = dialog?.ExecuteFileDialog(null, "JSON|*.json");
                            if (fileNames.Count > 0)
                            {
                                ImportData(fileNames[0]);
                            }
                        },
                        (param) => (true)
                        );
                }
                return mImportCommand;
            }
        }

        private void ImportData(string filename)
        {

            string input = File.ReadAllText(filename);
            SelectedProducts = JsonConvert.DeserializeObject<ObservableCollection<Product>>(input);
        }

        private ICommand mExportCommand;
        public ICommand ExportCommand
        {
            get
            {
                if (mExportCommand == null)
                {
                    mExportCommand = new RelayCommand(
                        (dialogType) => {
                            var dlgObj = Activator.CreateInstance(dialogType as Type) as IFileDialogWindow;
                            var fileNames = dlgObj?.ExecuteFileDialog(null, "JSON|*.json");
                            if (fileNames.Count > 0)
                            {
                                ExportData(fileNames[0]);
                            }
                        },
                    param => (SelectedProducts.Count > 0));
                }
                return mExportCommand;
            }
        }

        private void ExportData(string filename)
        {
            string products = JsonConvert.SerializeObject(SelectedProducts);
            File.WriteAllText(filename, products);
            SelectedProducts.Clear();
        }
    }
}
