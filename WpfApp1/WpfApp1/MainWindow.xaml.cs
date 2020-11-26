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
            ChartValues<double> lightList = new ChartValues<double>();
            string[] datetimearray = new string[databaseObject.Filldata().Rows.Count];
            DateTime[] datetimestringarray = new DateTime[databaseObject.Filldata().Rows.Count];

            for (int i = 0; i < databaseObject.Filldata().Rows.Count; i++)
            {
                tempList.Add(Convert.ToDouble(databaseObject.Filldata().Rows[i]["temp"]));
                lightList.Add(Convert.ToDouble(databaseObject.Filldata().Rows[i]["humit"]));
                datetimearray[i] = DateTime.FromOADate((Double)databaseObject.Filldata().Rows[(databaseObject.Filldata().Rows.Count - 1)]["datetime"] - 2415018.5).ToString("g");

                SeriesCollection = new SeriesCollection
            {
                new LineSeries { Title = "Temp", Values = tempList, ScalesYAt = 0 },
                new LineSeries { Title = "Light", Values = lightList, ScalesYAt = 1 },
            };
                AxisYCollection = new AxesCollection
            {
                new Axis { Title = "Y Axis 1 Temp", Foreground = Brushes.Gray },
                new Axis { Title = "Y Axis 2 Light", Foreground = Brushes.Red },
            };
                AxisXCollection = new AxesCollection
            {
                new Axis { Title = "X Axis 1 DateTime", Foreground = Brushes.Gray, Labels = datetimearray   },
            };

                // temp chart end 
                DataContext = this; // livechart 

            }
        }

        private void If(bool v)
        {
            throw new NotImplementedException();
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
        public AxesCollection AxisYCollection { get; set; }
        public AxesCollection AxisXCollection { get; set; }

        public string[] Labels { get; set; }
    }
}
