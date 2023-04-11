using System.Net;
using System.Net.Sockets;

internal class Client
{
    private static void Main(string[] args)
    {
		try
		{
			Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));

			Console.WriteLine($"Подключен к серверу: {client.RemoteEndPoint}");

			NetworkStream stream = new(client);
			StreamWriter writer = new(stream);
			StreamReader reader = new(stream);

			string message = reader.ReadLine()!;
			Console.WriteLine(message);

			while (true)
			{
                Console.Write(">");
				string input = Console.ReadLine()!;

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
				{
					break;
				}

                writer.WriteLine(input);
				writer.Flush();
            }
            writer.Close();
            reader.Close();
            stream.Close();
            client.Close();
        }
		catch (Exception ex)
		{
            Console.WriteLine($"Error: {ex.GetType()} {ex.Message}");
        }
    }
}