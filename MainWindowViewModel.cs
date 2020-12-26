﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using FmpDataTool.Model;
using System.Linq;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace FmpDataTool
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty UrlStockListProperty;
        public static readonly DependencyProperty ResultsStockListProperty;
        public static readonly DependencyProperty LogStocksProperty;
        public static readonly DependencyProperty LogFinancialsProperty;
        public static readonly DependencyProperty StockListProperty;
        public static readonly DependencyProperty ProgressValueProperty;
        public static readonly DependencyProperty FileNameStockListProperty;
        public static readonly DependencyProperty ConnectionStringProperty;
        public static readonly DependencyProperty BatchSizeProperty;
        public static readonly DependencyProperty UrlIncomeProperty;
        public static readonly DependencyProperty UrlBalanceProperty;
        public static readonly DependencyProperty UrlCashFlowProperty;

        public RelayCommand CommandRequestNavigate { get; set; }
        public RelayCommand CommandGetStockList { get; set; }
        public RelayCommand CommandSelectFile { get; set; }
        public RelayCommand CommandSaveInFile { get; set; }
        public RelayCommand CommandLoadFromFile { get; set; }
        public RelayCommand CommandSaveToDatabase { get; set; }


        private DispatcherTimer timer;

        /// <summary>
        /// MainWindowViewModel - Static
        /// </summary>
        static MainWindowViewModel()
        {
            UrlStockListProperty = DependencyProperty.Register("UrlStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ResultsStockListProperty = DependencyProperty.Register("ResultsStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            LogStocksProperty = DependencyProperty.Register("LogStocks", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            LogFinancialsProperty = DependencyProperty.Register("LogFinancials", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            StockListProperty = DependencyProperty.Register("StockList", typeof(Stock[]), typeof(MainWindowViewModel), new PropertyMetadata(new Stock[0]));
            ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            FileNameStockListProperty = DependencyProperty.Register("FileNameStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            BatchSizeProperty = DependencyProperty.Register("BatchSize", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            UrlIncomeProperty = DependencyProperty.Register("UrlIncome", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            UrlBalanceProperty = DependencyProperty.Register("UrlBalance", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            UrlCashFlowProperty = DependencyProperty.Register("UrlCashFlow", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));

    }

    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public MainWindowViewModel()
        {
            UrlStockList = Configuration.Instance["UrlStockList"];
            FileNameStockList = Configuration.Instance["FileNameStockList"];
            ConnectionString = Configuration.Instance["ConnectionString"];
            BatchSize = Convert.ToInt32(Configuration.Instance["BatchSize"]);
            UrlIncome = Configuration.Instance["UrlIncome"];
            UrlBalance= Configuration.Instance["UrlBalance"];
            UrlCashFlow = Configuration.Instance["UrlCashFlow"];

            CommandRequestNavigate = new RelayCommand(p => { Process.Start(new ProcessStartInfo(((Uri)p).AbsoluteUri) { UseShellExecute = true }); });
            CommandGetStockList = new RelayCommand(async (p) => await GetStockList(p));
            CommandSelectFile = new RelayCommand((p) => SelectFile(p));
            CommandSaveInFile = new RelayCommand((p) => SaveInFile(p));
            CommandLoadFromFile = new RelayCommand((p) => LoadFromFile(p));
            CommandSaveToDatabase = new RelayCommand((p) => SaveToDatabase(p));

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
        }

        /// <summary>
        /// UrlStockList
        /// </summary>
        public string UrlStockList
        {
            get { return (string)GetValue(UrlStockListProperty); }
            set { SetValue(UrlStockListProperty, value); }
        }

        /// <summary>
        /// ResultsStockList
        /// </summary>
        public string ResultsStockList
        {
            get { return (string)GetValue(ResultsStockListProperty); }
            set { SetValue(ResultsStockListProperty, value); }
        }

        /// <summary>
        /// LogStocks
        /// </summary>
        public string LogStocks
        {
            get { return (string)GetValue(LogStocksProperty); }
            set { SetValue(LogStocksProperty, value); }
        }

        /// <summary>
        /// LogFinancials
        /// </summary>
        public string LogFinancials
        {
            get { return (string)GetValue(LogFinancialsProperty); }
            set { SetValue(LogFinancialsProperty, value); }
        }

        /// <summary>
        /// StockList
        /// </summary>
        public Stock[] StockList
        {
            get { return (Stock[])GetValue(StockListProperty); }
            set { SetValue(StockListProperty, value); }
        }

        /// <summary>
        /// ProgressValue
        /// </summary>
        public int ProgressValue
        {
            get { return (int)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        /// <summary>
        /// FileNameStockList
        /// </summary>
        public string FileNameStockList
        {
            get { return (string)GetValue(FileNameStockListProperty); }
            set { SetValue(FileNameStockListProperty, value); }
        }

        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        /// <summary>
        /// BatchSize
        /// </summary>
        public int BatchSize
        {
            get { return (int)GetValue(BatchSizeProperty); }
            set { SetValue(BatchSizeProperty, value); }
        }

        /// <summary>
        /// UrlIncome
        /// </summary>
        public string UrlIncome
        {
            get { return (string)GetValue(UrlIncomeProperty); }
            set { SetValue(UrlIncomeProperty, value); }
        }

        /// <summary>
        /// UrlBalance
        /// </summary>
        public string UrlBalance
        {
            get { return (string)GetValue(UrlBalanceProperty); }
            set { SetValue(UrlBalanceProperty, value); }
        }

        /// <summary>
        /// UrlCashFlow
        /// </summary>
        public string UrlCashFlow
        {
            get { return (string)GetValue(UrlCashFlowProperty); }
            set { SetValue(UrlCashFlowProperty, value); }
        }

        /// <summary>
        /// GetStockList
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task GetStockList(object param)
        {
            Array.Clear(StockList, 0, StockList.Length);
            ResultsStockList = string.Empty;
            LogStocks = "Requesting stock list...";
            timer.Start();

            using var httpClient = new HttpClient();
            await httpClient.GetAsync(UrlStockList).ContinueWith((r) => OnRequestStockListCompleteAsync(r));
        }

        /// <summary>
        /// OnRequestStockListCompleteAsync
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        private async Task OnRequestStockListCompleteAsync(Task<HttpResponseMessage> requestTask)
        {
            var contentStream = await requestTask.Result.Content.ReadAsStreamAsync();
            Stock[] stockList = await JsonSerializer.DeserializeAsync<Stock[]>(contentStream);
            Dispatcher.Invoke(() => SetDataStockList(stockList));
        }

        /// <summary>
        /// SetDataStockList
        /// </summary>
        /// <param name="stockList"></param>
        private void SetDataStockList(Stock[] stockList)
        {
            StockList = stockList;
            ResultsStockList = JsonSerializer.Serialize(StockList);
            timer.Stop();
            LogStocks += "\r\nOK! stock list recieved.";
            ProgressValue = 0;
        }

        /// <summary>
        /// Timer_Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            ProgressValue++;
        }

        /// <summary>
        /// SelectFile
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private void SelectFile(object p)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                FileNameStockList = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// SaveInFile
        /// </summary>
        /// <param name="p"></param>
        private void SaveInFile(object p)
        {
            if (File.Exists(FileNameStockList))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("The file exists already. Do you want to overwrite it?", "Warning! File exists!", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    File.WriteAllText(FileNameStockList, ResultsStockList);
                }
            }
        }

        /// <summary>
        /// LoadFromFile
        /// </summary>
        /// <param name="p"></param>
        private void LoadFromFile(object p)
        {
            if (!File.Exists(FileNameStockList))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("The file doesn't exist. Selct another one.", "Warning! File doesn't exist!", MessageBoxButton.OK);
                return;
            }

            ResultsStockList = File.ReadAllText(FileNameStockList);
            StockList = JsonSerializer.Deserialize<Stock[]>(ResultsStockList);
        }

        /// <summary>
        /// SaveToDatabase
        /// </summary>
        /// <param name="p"></param>
        private void SaveToDatabase(object p)
        {
            if (DataContext.Instance.Stocks.Any())
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Database table 'Stocks' has already data. Do you want to overwrite it?", "Warning! Data exists!", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DataContext.Instance.Stocks.RemoveRange(DataContext.Instance.Stocks);
                }
                else
                {
                    return;
                }
            }

            LogStocks += "Saving to database...";
            DataContext.Instance.Stocks.AddRange(StockList);
            DataContext.Instance.SaveChanges();
            LogStocks += "\r\nOK! Saved to database.";

        }
    }
}
