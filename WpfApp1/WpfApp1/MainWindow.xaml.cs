using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
//using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        sqLitedatabase databaseObject = new sqLitedatabase();
        private double _value; // Angular Gauge


        public MainWindow()
        {
            InitializeComponent();
            Filldata();

            Value = 160; //Angular Gauge
            // Temp chat
            ChartValues<double> tempList = new ChartValues<double>();
            ChartValues<double> datetimeList = new ChartValues<double>();
            for(int i = 0; i < databaseObject.Filldata().Rows.Count; i++)
            {
                tempList.Add(Convert.ToDouble(databaseObject.Filldata().Rows[i]["temp"]));
                //datetimeList.Add(Convert.ToDouble(databaseObject.Filldata().Rows[i]["datetime"]));
            }
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Temp",
                    Values = tempList
                   // Labels = datetimeList
                },
            };

            
            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            // temp chart end 
            DataContext = this; // livechart 
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            databaseObject.OpenConnection();
            databaseObject.sendData(1, 2);
            databaseObject.CloseConnection();
            Filldata();
        }

        private void Filldata()
        {
            Dataset.ItemsSource = databaseObject.Filldata().DefaultView;
        }



        //Angular Gauge
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private void ChangeValueOnClick(object sender, RoutedEventArgs e)
        {
            Value = new Random().Next(50, 250);
        }


            public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //Angular Gauge end 

        // temp chart 
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
    }
}
