using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using SeSkarpApplikation;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SeSkarptServer server = new SeSkarptServer();

        public MainWindow()
        {
            InitializeComponent();
            Filldatagrid();
            Console.SetOut(new MultiTextWriter(new ControlWriter(console), Console.Out));
            WpfAddData();
        }

        private void WpfAddData()
        {
            InitializeComponent();
            DataTable local_DataTable = server.databaseObject.Filldata();
            LightGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["humit"]); //Angular Gauge
            TempGauge.Value = Convert.ToDouble(local_DataTable.Rows[(local_DataTable.Rows.Count - 1)]["temp"]); //Angular Gauge

            // Temp chat
            ChartValues<double> tempList = new ChartValues<double>();
            ChartValues<double> lightList = new ChartValues<double>();
            ChartValues<string> timeList = new ChartValues<string>();

            for (int i = 0; i < local_DataTable.Rows.Count; i++)
            {
                tempList.Add(Convert.ToDouble(local_DataTable.Rows[i]["temp"]));
                lightList.Add(Convert.ToDouble(local_DataTable.Rows[i]["humit"]));
                timeList.Add(DateTime.FromOADate((Double)local_DataTable.Rows[(i)]["datetime"] - 2415018.5).ToString("g"));
            }
            TempChart.Values = tempList;
            LightChart.Values = lightList;
            TimeDateChart.Labels = timeList;
            // temp chart end 
            DataContext = this; // livechart 
        }


        private void Filldatagrid()
        {
            Dataset.ItemsSource = server.databaseObject.Filldata().DefaultView;
        }
        private void SayHello_Click(object sender, RoutedEventArgs e)
        {
            // server.SendCommand(SeSkarptServer.Command.SayHello);
            //server.ReadStringData();
            server.databaseObject.OpenConnection();
            server.databaseObject.sendData(2000, 34);
            server.databaseObject.CloseConnection();
            //SectionsCollection[1].
            server.databaseObject.Filldata();
            //server.databaseObject.CloseConnection();
            Filldatagrid();
            WpfAddData();
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
            Filldatagrid();
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            server.DisconnectDevice();

        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            server.ConnectDevice();
        }
        public ChartValues<double> Values1 { get; set; }
        public ChartValues<double> Values2 { get; set; }


        // temp chart 
        public SeriesCollection SeriesCollection { get; set; }
        public AxesCollection AxisYCollection { get; set; }
        public AxesCollection AxisXCollection { get; set; }

        public string[] Labels { get; set; }
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
