using System.Net;
using System.Net.Sockets;

internal class Server
{
	static readonly List<Socket> clients = new();

    static void Main()
    {
		try
		{
			Socket listner = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			listner.Bind(new IPEndPoint(IPAddress.Any, 8080));
			listner.Listen(100);

			Console.WriteLine("Server started. Waiting for clients...");

			while (true)
			{
                Socket client = listner.Accept();

				clients.Add(client);

				Console.WriteLine($"Client connected: {client.RemoteEndPoint}");

				Thread thread = new(() => HandleClient(client));
                thread.Start();
            }
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.GetType()} {ex.Message}");
		}
    }

	static void HandleClient(Socket client)
	{
		try
		{
			NetworkStream stream = new(client);
			StreamWriter writer = new(stream);
			StreamReader reader = new(stream);

			writer.WriteLine("Welcome to the chat room!");
			writer.Flush();

			while (true)
			{
				string? message = reader.ReadLine();
				if (message == null)
				{
					break;
				}

				Console.WriteLine($"Received message from {client.RemoteEndPoint}: {message}");

				foreach (var otherClient in clients)
				{
					if (otherClient != client)
					{
                        NetworkStream otherStream = new(otherClient);
						StreamWriter otherWriter = new(otherStream);
						otherWriter.WriteLine($"{client.RemoteEndPoint}: {message}");
						otherWriter.Flush();
					}
				}
			}
            clients.Remove(client);

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