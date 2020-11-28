using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;

using SeSkarpApplikation;

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
            Filldata();
            Console.SetOut(new MultiTextWriter(new ControlWriter(console), Console.Out));
           
        }

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
