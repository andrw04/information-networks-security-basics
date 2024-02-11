using System.Text;

namespace DataEncryptionStandard;

public class DataBlock
{
    private StringBuilder sb = new StringBuilder();

    public int Count
    {
        get => sb.Length;
    }
    public string Bits
    {
        get => sb.ToString();
    }
    public DataBlock(string str)
    {
        if (str.Length != 64)
            throw new ArgumentException("String must have 64 chars");

        sb.Clear();

        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != '0' && str[i] != '1')
                throw new ArgumentException("String must consist of 1 and 0");
        }

        sb.Append(str);
    }

    public void Swap(int left, int right)
    {
        char temp = sb[left];
        sb[left] = sb[right];
        sb[right] = temp;
    }
}


public class DES
{
    // начальная перестановка
    private static int[] InitialPermutation =
    {
        58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
        62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
        57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
        61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7
    };

    private static int[] InverseInitialPermutation =
    {
        40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
        38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
        36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
        34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25,
    };

    public static DataBlock[] DivideIntoBlocks(string data)
    {
        List<DataBlock> result = new();

        string bytes = String.Join("", Encoding.UTF8.GetBytes(data).Select(d => 
            Convert.ToString(d, 2).PadLeft(8, '0')));
        
        for (int i = 0; i < bytes.Length; i += 64)
        {
            string substring = bytes.Substring(i, Math.Min(64, bytes.Length - i));
            result.Add(new DataBlock(substring.PadLeft(64, '0')));
        }
        
        return result.ToArray();
    }

    public static void GetInitialPermutation(DataBlock[] blocks)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            for (int j = 0; j < blocks[i].Count; j++)
            {
                blocks[i].Swap(j, InitialPermutation[j] - 1);
            }
        }
    }
}