﻿using Newtonsoft.Json;
using Swordfish.NET.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private ObservableCollection<Product> mSelectedProducts;
        private Product mAllSelectedProduct = null;
        private Product mChosenSelectedProduct = null;

        private TcpClient client;
        private NetworkStream networkStream;
        private byte[] buffer = new byte[1024];
        private string totalBuffer;
        private bool connected = false;

        private string username = "JKB";
        private string password = "1234";
        private int mProductAmount = 1;

        public TakeAwayViewModel()
        {
            mAllProducts = new ConcurrentObservableCollection<Product>();

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
            Write($"login\r\n{username}\r\n{password}");
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
                case "requestRestaurant":
                    dynamic json = packetData[1];
                    List<Product> products = JsonConvert.DeserializeObject<List<Product>>(json);

                    Task.Run(() => Parallel.ForEach(products, product =>
                    {
                        mAllProducts.Add(new Product { Name = product.Name, Price = product.Price, Type = product.Type});
                    }));
                    break;
                case "sendList":
                    int size = int.Parse(packetData[1]);
                    for (int i = 0; i < size; i++)
                    {

                    }
                    break;
            }
        }

        public void Write(string data)
        {
            var dataAsBytes = Encoding.ASCII.GetBytes(data + "\r\n\r\n");
            networkStream.Write(dataAsBytes, 0, dataAsBytes.Length);
            networkStream.Flush();
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
            Write($"sendOrder\r\n{list}");
            SelectedProducts.Clear();
        }
    }
}
