using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        UdpClient client;
        ObservableCollection<MessageInfo> messages;
        public MainWindow()
        {
            InitializeComponent();
            string serverAddress = ConfigurationManager.AppSettings["serverAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["serverPort"]!);
            serverPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            client = new UdpClient();
            messages = new ObservableCollection<MessageInfo>();
            this.DataContext = messages;
        }

        private  void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = msgTextBox.Text;
            msgTextBox.Text = "";
            SendMessage(message);
        }

        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "$<Join>";
            SendMessage(message);
            Listener();
        }
        private async void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            await client.SendAsync(data, data.Length, serverPoint);
        }
        private async void Listener()
        {
            while (true)
            {
                var res = await client.ReceiveAsync();
                string message = Encoding.Unicode.GetString(res.Buffer);
                messages.Add(new MessageInfo(message));
            }
        }
    }
    public class MessageInfo
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public MessageInfo(string message)
        {
            Message = message;
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Message,-20} {Time.ToShortTimeString()}";
        }
    }
}
