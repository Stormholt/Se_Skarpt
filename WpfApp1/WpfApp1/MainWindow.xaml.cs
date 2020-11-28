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
using SeSkarpApplikation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //sqLitedatabase databaseObject = new sqLitedatabase();
        private double _value; // Angular Gauge
        SeSkarptServer server = new SeSkarptServer();

        public MainWindow()
        {
            InitializeComponent();
            Filldata();
            Console.SetOut(new MultiTextWriter(new ControlWriter(console), Console.Out));

            Value = 30; //Angular Gauge
            // Temp chat
            ChartValues<double> tempList = new ChartValues<double>();
            ChartValues<double> lightList = new ChartValues<double>();
            string[] datetimearray = new string[server.databaseObject.Filldata().Rows.Count];
            DateTime[] datetimestringarray = new DateTime[server.databaseObject.Filldata().Rows.Count];

            for (int i = 0; i < server.databaseObject.Filldata().Rows.Count; i++)
            {
                tempList.Add(Convert.ToDouble(server.databaseObject.Filldata().Rows[i]["temp"]));
                lightList.Add(Convert.ToDouble(server.databaseObject.Filldata().Rows[i]["humit"]));
                datetimearray[i] = DateTime.FromOADate((Double)server.databaseObject.Filldata().Rows[(server.databaseObject.Filldata().Rows.Count - 1)]["datetime"] - 2415018.5).ToString("g");

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
        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            databaseObject.OpenConnection();
            databaseObject.sendData(1, 2);
            databaseObject.CloseConnection();
            Filldata();
        }
        */

        private void Filldata()
        {
            Dataset.ItemsSource = server.databaseObject.Filldata().DefaultView;
        }
        private void SayHello_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SayHello);
            server.ReadStringData();
        }

        private void LEDred_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.Red);
        }

        private void LEDgreen_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.Green);
        }

        private void LEDblue_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.Blue);
        }

        private void LEDmagenta_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.Magenta);
        }

        private void LEDcyan_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.Cyan);
        }

        private void LEDyellow_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.Yellow);
        }

        private void LEDwhite_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.White);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.SetData);
            server.SendLEDCommand(SeSkarptServer.Ledcolor.Off);
        }

        private void Sample_Click(object sender, RoutedEventArgs e)
        {
            server.SendCommand(SeSkarptServer.Command.GetData);
            server.ReadSensorData();
            Filldata();
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            server.DisconnectDevice();

        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            server.ConnectDevice();
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

        private void AngularGauge_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
    public class MultiTextWriter : TextWriter
    {
        private IEnumerable<TextWriter> writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers)
        {
            this.writers = writers.ToList();
        }
        public MultiTextWriter(params TextWriter[] writers)
        {
            this.writers = writers;
        }

        public override void Write(char value)
        {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Write(string value)
        {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Flush()
        {
            foreach (var writer in writers)
                writer.Flush();
        }

        public override void Close()
        {
            foreach (var writer in writers)
                writer.Close();
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
    public class ControlWriter : TextWriter
    {
        private TextBox textbox;
        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Text += value;

        }

        public override void Write(string value)
        {
            textbox.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
