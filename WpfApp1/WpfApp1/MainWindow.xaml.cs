using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using LiveCharts;
using SeSkarpApplikation;
using System.IO;
using System.Timers;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial  class MainWindow : Window
    {
       static SeSkarptServer server = new SeSkarptServer();
        public const int SLEEP_TIME_SEC = 10;
        private Timer aTimer;
        

        public MainWindow() // MainWindow for the WPF App used to initalize everything, for the first time. 
        {
            InitializeComponent();
            Filldatagrid(); // Filling out the Grid of the DataTable
            Console.SetOut(new MultiTextWriter(new ControlWriter(console), Console.Out));
            WpfAddData(); // Addding any old data that may be in the Database
            SetTimer();
           
        }

        private void WpfAddData() // Adding all the old data from the database. 
        {
            InitializeComponent(); 
            DataTable local_DataTable = server.databaseObject.Filldata(); // creating local dataTable for the old data 
            if (local_DataTable.Rows.Count == 0) // Making sure that there is data 
            {
                return;
            }
            // Filling in the data to the gauge form the datatable
             LightGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["light"]); //Angular Gauge
             TempGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["temp"]); //Angular Gauge
                
            
            // Temp chart variables 
            ChartValues<double> tempList = new ChartValues<double>();
            ChartValues<double> lightList = new ChartValues<double>();
            ChartValues<string> timeList = new ChartValues<string>();
            // Filling out all the data in the datatable 
            for (int i = 0; i < local_DataTable.Rows.Count; i++)
            {
                tempList.Add(Convert.ToDouble(local_DataTable.Rows[i]["temp"]));
                lightList.Add(Convert.ToDouble(local_DataTable.Rows[i]["light"]));
                timeList.Add(DateTime.FromOADate((Double)local_DataTable.Rows[(i)]["datetime"] - 2415018.5).ToString("g")); // Following the convertion Julian -> OA -> Normale De timeDate.
            }
            // Loading all the data in the chart with value from the datatable
            TempChart.Values = tempList;
            LightChart.Values = lightList;
            TimeDateChart.Labels = timeList;
            // temp chart end 
            DataContext = this; // livechart 
        }

        private void UpdateData()
        {   
                DataTable local_DataTable = server.databaseObject.Filldata(); // Creating a Local Datatable,
                TempChart.Values.Add(Convert.ToDouble(local_DataTable.Rows[local_DataTable.Rows.Count - 1]["temp"])); // Temp - Chart
                LightChart.Values.Add(Convert.ToDouble(local_DataTable.Rows[local_DataTable.Rows.Count - 1]["light"])); // Light - Chart
                TimeDateChart.Labels.Add(DateTime.FromOADate((Double)local_DataTable.Rows[local_DataTable.Rows.Count - 1]["datetime"] - 2415018.5).ToString("g")); // TimeDate - Chart 
                LightGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["light"]); //Light - Angular Gauge
                TempGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["temp"]); //Temp - Angular Gauge
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
             aTimer = new Timer(SLEEP_TIME_SEC*1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += TimerMethod;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void TimerMethod(Object source, ElapsedEventArgs e) {
            if (server.connected == true)
            {
                
                Sample_Click(null, null);
            }
        }

        private void Filldatagrid()
        {
            Dataset.ItemsSource = server.databaseObject.Filldata().DefaultView; // Load the data in to the grid with a datatable 
        }
        // All the Buttons 
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
            this.Dispatcher.Invoke(() =>
            { 
               server.SendCommand(SeSkarptServer.Command.GetData);
                server.ReadSensorData();
                server.databaseObject.Filldata();
                Filldatagrid();
                UpdateData();
             });
           
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            server.DisconnectDevice();


        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            server.ConnectDevice();
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
