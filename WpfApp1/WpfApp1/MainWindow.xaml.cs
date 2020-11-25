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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        sqLitedatabase databaseObject = new sqLitedatabase();

        
        public MainWindow()
        {
            InitializeComponent();
            Filldata();
            ChartValues<double> tempList = new ChartValues<double>();
            for(int i = 0; i < databaseObject.Filldata().Rows.Count; i++)
            {
                tempList.Add(Convert.ToDouble(databaseObject.Filldata().Rows[i]["temp"]));   
            }
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Temp",
                    Values = tempList
                },
            };

            
            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            //Temp = value => value.ToString("C");
            /*
            //modifying the series collection will animate and update the chart
            SeriesCollection.Add(new LineSeries
            {
                Title = "Series 4",
                Values = new ChartValues<double> { 5, 3, 2, 4 },
                LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                //PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
                PointGeometrySize = 50,
                PointForeground = Brushes.Gray
            });

            //modifying any series values will also animate and update the chart
            SeriesCollection[1].Values.Add(5d);
            */
            DataContext = this;
            
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
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        //public Func<double, string> Temp { get; set; }
    }
}
