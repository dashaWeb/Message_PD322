using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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

namespace MessageClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*const string serverAddress = "127.0.0.1";
        const short serverPort = 4040;*/
        IPEndPoint serverPoint;
        //UdpClient client;
        TcpClient client;
        NetworkStream ns = null;
        StreamReader sr = null;
        StreamWriter sw = null;
        ObservableCollection<MessageInfo> messages;
        public MainWindow()
        {
            InitializeComponent();
            string serverAddress = ConfigurationManager.AppSettings["serverAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["serverPort"]!);
            serverPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            client = new TcpClient();
            messages = new ObservableCollection<MessageInfo>();
            this.DataContext = messages;
        }

        private  async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            MessageInfo client_content = new MessageInfo(msgTextBox.Text,nameClient.Text);
            msgTextBox.Text = "";
            string message = JsonSerializer.Serialize<MessageInfo>(client_content);

            await sw.WriteLineAsync(message);
            sw.Flush();

            Listener();
        }

        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            /*if(nameClient.Text == "" || nameClient.Text == " ")
            {
                MessageBox.Show("Error");
                return;
            }
            nameClient.Visibility = Visibility.Hidden;*/

            try
            {
                client.Connect(serverPoint);
                ns = client.GetStream();

                sr = new StreamReader(ns);
                sw = new StreamWriter(ns); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void Listener()
        {
            while (true)
            {
                string? message = await sr.ReadLineAsync();
                //MessageInfo client_message = JsonSerializer.Deserialize<MessageInfo>(message)!;
                //messages.Add(client_message);
                MessageBox.Show($"{message}");
            }
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await sw.WriteLineAsync("close");

            ns.Close();
            client.Close();
        }
    }
    public class MessageInfo
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public MessageInfo(string message,string name)
        {
            Message = message;
            Name = name;
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Name} -- {Message,-20} {Time.ToShortTimeString()}";
        }
    }
}
