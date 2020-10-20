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
using System.Windows.Media;
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

        private int mProductAmount = 1;
        private double mProductPrice = 0.00d;
        private string mProductPriceString = "0,00";

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
            mSelectedProducts = new ObservableCollection<Product>();

            client.BeginConnect("localhost", 12345, new AsyncCallback(OnConnect), null);
            while (!connected) { } 
            Write("requestRestaurant"); //request a list of restaurants to load in combobox
        }

        public void OnConnect(IAsyncResult ar) 
        {
            client.EndConnect(ar);
            networkStream = client.GetStream();
            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            connected = true;
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
                case "requestProducts": //list of products received
                    dynamic productJson = packetData[1];
                    List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productJson);

                    Task.Run(() => Parallel.ForEach(products, product =>
                    {
                        mAllProducts.Add(new Product { Name = product.Name, Price = product.Price, Type = product.Type});
                    }));
                    break;

                case "requestRestaurant": //list of restaurants received
                    dynamic restaurantJson = packetData[1];
                    List<Restaurant> restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(restaurantJson);

                    //because of multi-threading, we have to add the restaurant parallel
                    Task.Run(() => Parallel.ForEach(restaurants, restaurant =>
                    {
                        mAllRestaurants.Add(new Restaurant { Name = restaurant.Name, Address = restaurant.Address });
                    }));
                    break;

                case "getReceipt": //receipt received
                    string receiptJson = packetData[1];
                    string path = $@"{Environment.CurrentDirectory}\receipt-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.txt";
                    File.WriteAllText(path, receiptJson); //write the received receipt to a textfile
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(path)
                    {
                        UseShellExecute = true
                    };
                    p.Start(); //open the receipt textfile
                    break;
            }
        }

        public void Write(string data)
        {
            var dataAsBytes = Encoding.ASCII.GetBytes(data + "\r\n\r\n");
            networkStream.Write(dataAsBytes, 0, dataAsBytes.Length);
            networkStream.Flush();
        }

        public string FirstName //First name of the customer, binded with its textbox in the GUI
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

        public string LastName //surname of the customer, binded with its textbox in the GUI
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

        public string PostalCode //Postal Code of the customer, binded with its textbox in the GUI
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

        public string HouseNumber //Housenumber of the customer, binded with its textbox in the GUI
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
        
        public string productPriceString //Price of all selected products in string type, binded with its label in the GUI
        {
            get
            {
                return mProductPriceString;
            }
            set
            {
                mProductPriceString = value;
                NotifyPropertyChanged();
            }
        }

        public double productPrice //Price of all selected products in double type
        {
            get
            {
                return mProductPrice;
            }
            set
            {
                mProductPrice = value;
                NotifyPropertyChanged();
            }
        }

        public int productAmount //The amount of products the customer want to add, binded with its textbox in the GUI
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

        public ConcurrentObservableCollection<Product> Products //List of all products from a restaurant received from the server (database), binded with its listview (left) in the GUI
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

        public ConcurrentObservableCollection<Restaurant> Restaurants //List of all restaurants received from de server (database), binded with the combobox in the GUI
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

        public ObservableCollection<Product> SelectedProducts //List of all products the customer want to order binded with its listview (right) in the GUI
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

        public Restaurant SelectedRestaurant //The restaurant selected by the customer, binded with te combobox in the GUI
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
                    mAllProducts.Clear(); //if selected restaurant is changed, clear the left listview
                    mSelectedProducts.Clear(); //if selected restaurant is changed, clear the right listview
                    UpdatePrice(); //set the price to 0
                    Write($"requestProducts\r\n{mChosenRestaurant.Name}"); //if selected restaurant is changed, ask the products of that restaurant again
                    NotifyPropertyChanged();
                }
            }
        }

        public Product AllSelectedProduct //Selected product in the listview of all products (left listview)
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

        public Product ChosenSelectedProduct //Selected product in the listview of the products to order (right listview)
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

        private void AddProduct() //add button clicked
        {
            //if product exists, add just the amount
            for (int i = 0; i < productAmount; i++)
            {
                SelectedProducts.Add(AllSelectedProduct); //add the selected products as much as the customer has specified
            }
            productAmount = 1; //set the text in the textbox of productamount to 1
            UpdatePrice(); //update the price label
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

        private void DeleteProduct() //delete button clicked
        {
            SelectedProducts.Remove(ChosenSelectedProduct); //remove the selected product in the right listview
            UpdatePrice(); //update the price label
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

        private void SendProducts() //send button clicked
        {
            if (!(user.FirstName == null || user.LastName == null || user.PostalCode == null || user.HouseNumber == null)) //can't send the order if information is not filled in by the customer
            {
                string list = JsonConvert.SerializeObject(SelectedProducts);
                string userJson = JsonConvert.SerializeObject(user);
                Write($"sendOrder\r\n{list}\r\n{userJson}\r\n{SelectedRestaurant.Name}\r\n{productPrice}"); //write this commando to the server
                SelectedProducts.Clear(); //clear the right listview
                UpdatePrice(); //set the price to 0
            }            
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

        private void ImportData(string filename) //import button clicked
        {
            string input = File.ReadAllText(filename);
            string separator = "-"; //separator between the restaurant information and the products
            string[] content = input.Split(separator.ToCharArray());

            SelectedRestaurant = JsonConvert.DeserializeObject < Restaurant > (content[0]); //restaurant from the file is the selected restaurant
            SelectedProducts = JsonConvert.DeserializeObject<ObservableCollection<Product>>(content[1]); //products from the file are filled in the right listview

            UpdatePrice(); //update the price
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
                            var dialog = Activator.CreateInstance(dialogType as Type) as IFileDialogWindow;
                            var fileNames = dialog?.ExecuteFileDialog(null, "JSON|*.json");
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

        private void ExportData(string filename) //export button clicked
        {
            string restaurant = JsonConvert.SerializeObject(SelectedRestaurant);
            string products = JsonConvert.SerializeObject(SelectedProducts);
            string data = $"{restaurant}-{products}";
            File.WriteAllText(filename, data); //write the selected restaurant and selected products to a file to store
            SelectedProducts.Clear(); //clear the right listview
        }

        private void UpdatePrice() // calculate and show total price
        {
            productPrice = 0;
            foreach (Product product in SelectedProducts)
            {
                productPrice += product.Price; 
            }
            productPriceString = $"{productPrice:##0.00}"; //two decimal price displayed in label 
        }
    }
}
