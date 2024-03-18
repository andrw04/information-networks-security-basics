using System.Net.Sockets;
using System.Net;

namespace TcpAttackApp
{
    public class TcpAttacker
    {
        public static void Start()
        {
            var targetEndpoint = new IPEndPoint(IPAddress.Parse(Configuration.IP), Configuration.Port);
            var attackingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                attackingSocket.Bind(new IPEndPoint(IPAddress.Parse(Configuration.IP), 10000));

                Parallel.For(1, 100, i =>
                {
                    var packet = new Packet(10000 + i, Configuration.Port, syn: true);

                    attackingSocket.SendTo(packet.ToByteArray(), targetEndpoint);

                    var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    try
                    {
                        clientSocket.Bind(new IPEndPoint(IPAddress.Parse(Configuration.IP), 10000 + i));

                        var data = new byte[100];
                        var countBytes = clientSocket.Receive(data);
                        var receivedPacket = Packet.FromByteArray(Packet.BitSection(data, countBytes));

                        var responsePacket = new Packet(receivedPacket.DestinationPort, receivedPacket.SenderPort, ack: true);
                        clientSocket.SendTo(responsePacket.ToByteArray(), targetEndpoint);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        clientSocket.Close();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                attackingSocket.Close();
            }
        }
    }

}
