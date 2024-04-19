using System.Net;
using System.Net.Sockets;
using System.Text;


public class ChatServer
{
    const short port = 4040;

    TcpListener server = null;
    IPEndPoint clientEndPoint = null;
    StreamReader sr = null;
    StreamWriter sw = null;
    public ChatServer()
    {
        server = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"),port));
    }
    public  void Start()
    {
        server.Start();
        Console.WriteLine("Connected !!!!");
        
        //server.AcceptSocket();
        TcpClient client = server.AcceptTcpClient();

        while (true)
        {
          NetworkStream ns = client.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
          string? message = sr.ReadLine();
          Console.WriteLine($"Message :: {message} from : {client.Client.LocalEndPoint}");

            if(message == "close")
            {
                sw.WriteLine("Good");
                ns.Close();
                server.Stop();
                return;
            }

          sw.WriteLine("Send");
          sw.Flush();
        }
    }



}
internal class Program
{

    private static void Main(string[] args)
    {
        ChatServer server = new ChatServer();
        server.Start();
    }
}