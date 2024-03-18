using System.Collections;
using System.Text;

namespace TcpAttackApp
{
    public class Packet
    {
        public int SenderPort;
        public int DestinationPort;
        public int SequenceNumber;
        public int AckNumber;
        public bool URG;
        public bool ACK;
        public bool PSH;
        public bool RST;
        public bool SYN;
        public bool FIN;
        public string OtherData;

        public Packet(
            int senderPort = 0,
            int destinationPort = 0,
            int sequenceNumber = 0,
            int ackNumber = 0,
            bool urg = false,
            bool ack = false,
            bool psh = false,
            bool rst = false,
            bool syn = false,
            bool fin = false,
            string data = "")
        {
            SenderPort = senderPort;
            DestinationPort = destinationPort;
            SequenceNumber = sequenceNumber;
            AckNumber = ackNumber;
            URG = urg;
            ACK = ack;
            PSH = psh;
            RST = rst;
            SYN = syn;
            FIN = fin;
            OtherData = data;
        }

        public static Packet FromByteArray(byte[] src)
        {
            var source = new BitArray(src);
            return new Packet(
                senderPort: BitToInt(Slice(source, 0, 16)),
                           destinationPort: BitToInt(Slice(source, 16, 16)),
                           sequenceNumber: BitToInt(Slice(source, 32, 32)),
                           ackNumber: BitToInt(Slice(source, 64, 32)),
                           urg: source[106],
                           ack: source[107],
                           psh: source[108],
                           rst: source[109],
                           syn: source[110],
                           fin: source[111],
                           data: BitArrayToString(Slice(source, 160, source.Length - 160)));
        }

        private BitArray ToBitArray()
        {
            BitArray res = new BitArray(160 + OtherData.Length * 8);
            CopyBits(ConvertIntToBits(SenderPort), res, 0);
            CopyBits(ConvertIntToBits(DestinationPort), res, 16);
            CopyBits(new BitArray(new int[] { SequenceNumber }), res, 32);
            CopyBits(new BitArray(new int[] { AckNumber }), res, 64);
            res[106] = URG;
            res[107] = ACK;
            res[108] = PSH;
            res[109] = RST;
            res[110] = SYN;
            res[111] = FIN;
            CopyBits(new BitArray(Encoding.UTF8.GetBytes(OtherData)), res, 160);
            return res;
        }

        public byte[] ToByteArray()
        {
            BitArray bitArray = ToBitArray();
            byte[] data = new byte[bitArray.Length / 8];
            bitArray.CopyTo(data, 0);
            return data;
        }

        private static BitArray ConvertIntToBits(int value)
        {
            BitArray res = new BitArray(16);
            for (int i = 0; value > 0 && i < res.Length; i++)
            {
                res[i] = value % 2 == 0 ? false : true;
                value /= 2;
            }
            return res;
        }

        private static int BitToInt(BitArray source)
        {
            int[] data = new int[1];
            source.CopyTo(data, 0);
            return data[0];
        }

        private static string BitArrayToString(BitArray source)
        {
            byte[] data = new byte[source.Length / 8];
            source.CopyTo(data, 0);
            return Encoding.UTF8.GetString(data);
        }

        private static BitArray Slice(BitArray source, int start, int count)
        {
            BitArray slice = new BitArray(count);
            for (int i = 0; i < count; i++)
            {
                slice[i] = source[i + start];
            }
            return slice;
        }

        private void CopyBits(BitArray source, BitArray dest, int start)
        {
            for (int i = 0; i < source.Length; i++)
            {
                dest[start + i] = source[i];
            }
        }

        public static byte[] BitSection(byte[] source, int len)
        {
            byte[] res = new byte[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i];
            }
            return res;
        }
    }
}
