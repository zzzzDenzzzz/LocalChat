using System.Net;
using System.Net.Sockets;
using System.Text;

internal class ProgramServer
{
    delegate void ConnectDelegate(Socket s);
    delegate void StartNetwork(Socket s);

    Socket? socket;
    readonly IPEndPoint endP;
    readonly List<string> messages = new();

    public ProgramServer(string strAddr, int port)
    {
        endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
    }

    static void ServerConnect(Socket s)
    {
        s.Send(Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString("G")));
        s.Shutdown(SocketShutdown.Both);
        s.Close();
    }

    void ServerBegin(Socket s)
    {
        try
        {
            while (s != null)
            {
                Socket ns = s.Accept();
                Console.WriteLine($"request IP and port {ns.RemoteEndPoint}");
                Console.WriteLine($"Available data: {ns.Available}");
                byte[] responseData = new byte[ns.Available];

                ns.Receive(responseData);

                string requestMessage = Encoding.UTF8.GetString(responseData);

                Console.WriteLine("Received message:" + requestMessage);
                messages.Add(requestMessage);

                ServerConnect(ns);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    void Start()
    {
        socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endP);
        socket.Listen(10);
        StartNetwork start = ServerBegin;
        start.Invoke(socket);
    }

    void Stop()
    {
        if (socket != null)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        Environment.Exit(0);
    }

    public static void Main()
    {
        ProgramServer s = new("127.0.0.1", 8080);
        s.Start();
        s.Stop();
    }
}