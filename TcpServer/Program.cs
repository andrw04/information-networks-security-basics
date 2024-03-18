using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TCPServer
{
    static void Main()
    {
        int port = 8888;
        int backlog = 100; // Ограничение очереди на подключения

        TcpListener server = null;

        try
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            server.Start();

            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Подключен клиент!");

                NetworkStream stream = client.GetStream();

                byte[] data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);
                string message = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine($"Получено: {message}");

                byte[] response = Encoding.ASCII.GetBytes("Сообщение получено");
                stream.Write(response, 0, response.Length);

                client.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            server.Stop();
        }
    }
}