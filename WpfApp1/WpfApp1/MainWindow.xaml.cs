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
        public const int THREAD_SLEEP_TIME_SEC = 10;
        private Timer aTimer;
        

        public MainWindow()
        {
            InitializeComponent();
            Filldatagrid();
            Console.SetOut(new MultiTextWriter(new ControlWriter(console), Console.Out));
            WpfAddData();
            /*Thread thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();*/
            SetTimer();
           
        }

        private void WpfAddData()
        {
            //InitializeComponent(); // ikke sikker på om den kan fjernes tjek med data ajs
            DataTable local_DataTable = server.databaseObject.Filldata(); // creating a local datatable with data from the database 
            if (local_DataTable.Rows.Count == 0) // making sure there is data to fill 
            {
                return;
            }

             LightGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count)]["light"]); //Angular Gauge - light data
             TempGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count)]["temp"]); //Angular Gauge - temp data 
                
            
            // Temp chat
            // Creating ChartValues, which is a collection of data - from livechart 
            ChartValues<double> tempList = new ChartValues<double>();
            ChartValues<double> lightList = new ChartValues<double>();
            ChartValues<string> timeList = new ChartValues<string>();

            for (int i = 0; i < local_DataTable.Rows.Count; i++) // Checking the whole datatable 
            {
                tempList.Add(Convert.ToDouble(local_DataTable.Rows[i]["temp"]));
                lightList.Add(Convert.ToDouble(local_DataTable.Rows[i]["light"]));
                // The time is convertede from: real(Julian) -> OE -> UTC in DE format 
                timeList.Add(DateTime.FromOADate((Double)local_DataTable.Rows[(i)]["datetime"] - 2415018.5).ToString("g")); 
            }
            // loading the value in the the 2 Y Axis with temp/light and the 1 X with timedate
            TempChart.Values = tempList;
            LightChart.Values = lightList;
            TimeDateChart.Labels = timeList;
            // temp chart end 
            DataContext = this; // livechart 
        }

        private void UpdateData() // Function that only loads the last entry in the database 
        {
                DataTable local_DataTable = server.databaseObject.Filldata(); // Creating a Local Datatable
                TempChart.Values.Add(Convert.ToDouble(local_DataTable.Rows[local_DataTable.Rows.Count - 1]["temp"])); // Temp - Chart
                LightChart.Values.Add(Convert.ToDouble(local_DataTable.Rows[local_DataTable.Rows.Count - 1]["light"])); // Light - Chart
                TimeDateChart.Labels.Add(DateTime.FromOADate((Double)local_DataTable.Rows[local_DataTable.Rows.Count - 1]["datetime"] - 2415018.5).ToString("g")); // TimeDate - Chart 
                LightGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["light"]); //Light - Angular Gauge
                TempGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["temp"]); //Temp - Angular Gauge
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
             aTimer = new Timer(THREAD_SLEEP_TIME_SEC*1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += ThreadMethod;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void ThreadMethod(Object source, ElapsedEventArgs e) {
            if (server.connected == true)
            {
                
                Sample_Click(null, null);
            }
        }

        private void Filldatagrid()
        {
            Dataset.ItemsSource = server.databaseObject.Filldata().DefaultView;
        }
        private void SayHello_Click(object sender, RoutedEventArgs e)
        {
             server.SendCommand(SeSkarptServer.Command.SayHello);
            server.ReadStringData();
          
        }

        /// <summary>
        /// All the buttons functions, sending the command/data
        /// </summary>
     
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
