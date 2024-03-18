using System.Net;
using System.Net.Sockets;

namespace TcpAttackApp
{
    public class Server
    {
        private static readonly int _maxSyn = 10;
        private static readonly int _maxConnection = 10;
        private static readonly List<int> _syn = new List<int>();
        private static readonly List<int> _connections = new List<int>();

        public static void Start()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(Configuration.IP), Configuration.Port);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                server.Bind(endPoint);
                while (true)
                {
                    var data = new byte[1024];
                    var len = server.Receive(data);
                    var recievedPacket = Packet.FromByteArray(Packet.BitSection(data, len));

                    if (recievedPacket.DestinationPort != Configuration.Port)
                    {
                        continue;
                    }
                    else if (_connections.Contains(recievedPacket.SenderPort))
                    {
                        continue;
                    }
                    else if (_syn.Contains(recievedPacket.SenderPort) && recievedPacket.ACK)
                    {
                        _connections.Add(recievedPacket.SenderPort);
                        _syn.Remove(recievedPacket.SenderPort);

                        if (_connections.Count > _maxConnection)
                            throw new Exception("Connection limit exceeded");

                        Console.WriteLine($"Received ACK from {recievedPacket.SenderPort}");
                    }
                    else if (!_syn.Contains(recievedPacket.SenderPort) && recievedPacket.SYN)
                    {
                        Console.WriteLine($"Received SYN from {recievedPacket.SenderPort}");

                        var sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        try
                        {
                            sender.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));

                            var responsePacket = new Packet(
                                Configuration.Port,
                                recievedPacket.SenderPort,
                                syn: true,
                                ack: true);
                            sender.SendTo(responsePacket.ToByteArray(), new IPEndPoint(IPAddress.Parse(Configuration.IP), recievedPacket.SenderPort));
                            _syn.Add(recievedPacket.SenderPort);

                            if (_syn.Count > _maxSyn)
                                throw new Exception("SYN limit exceeded");

                            Console.WriteLine($"Sent SYN-ACK to {recievedPacket.SenderPort}");
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            sender.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.Close();
            }
        }
    }
}
