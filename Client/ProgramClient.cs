using System.Net.Sockets;
using System.Net;
using System.Text;

internal class ProgramClient
{
    Socket? socket;
    readonly IPEndPoint point;

    ProgramClient()
    {
        point = new IPEndPoint(IPAddress.Loopback, 8080);
    }

    void SendMessage()
    {
        socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(point);
            do
            {
                Console.WriteLine("Request:");
                string message = Console.ReadLine()!;
                if (message != null)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    socket?.Send(messageBytes, SocketFlags.None);
                    Console.WriteLine($"Socket client send message: \"{message}\"");
                }
            } while (true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    void CloseConnection()
    {
        try
        {
            socket?.Shutdown(SocketShutdown.Both);
            socket?.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static string GetServerResponse(Socket s)
    {
        byte[] fromServer = new byte[s.Available];
        try
        {
            Console.WriteLine(s.Available);
            s.Receive(fromServer);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return Encoding.UTF8.GetString(fromServer);
    }

    public static void Main()
    {
        ProgramClient client = new();
        client.SendMessage();

        if (client.socket != null) Console.WriteLine(GetServerResponse(client.socket));

        client.CloseConnection();
    }
}