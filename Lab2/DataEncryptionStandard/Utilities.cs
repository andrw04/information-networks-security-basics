using System.Collections;
using System.Text;

namespace DataEncryptionStandard;

public static class Utilities
{
    public static byte[] ToByteArray(this BitArray bitArray)
    {
        int arrayLength = (bitArray.Length + (8 - bitArray.Length % 8)) / 8;

        byte[] bytes = new byte[arrayLength];

        bitArray.CopyTo(bytes, 0);

        return bytes;
    }

    public static string ToBinaryString(this BitArray bitArray)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < bitArray.Length; i++)
        {
            sb.Append(bitArray[i] ? 1 : 0);
        }

        return sb.ToString();
    }

    public static BitArray[] SplitBy(this BitArray bitArray, int count)
    {
        if (bitArray.Length % count != 0)
            throw new ArgumentException($"Original array cannot be divided by {count}");

        BitArray[] blocks = new BitArray[bitArray.Length / count];

        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i] = bitArray.GetSubArray(i * count, count);
        }

        return blocks;
    }

    public static BitArray GetSubArray(this BitArray bitArray, int start, int length)
    {
        BitArray result = new BitArray(length, false);

        for (int i = 0; i < length; i++)
        {
            result[i] = bitArray[start + i];
        }

        return result;
    }

    public static BitArray Remove(this BitArray bitArray, int start, int count)
    {
        BitArray result = new BitArray(bitArray.Length - count, false);

        int originalIndex = 0;
        int resultIndex = 0;
        while(originalIndex < bitArray.Length)
        {
            while (originalIndex >= start)
            result[resultIndex] = bitArray[originalIndex];
        }

        return result;
    }

    public static BitArray Join(params BitArray[] bitArrays)
    {
        int length = 0;

        foreach (BitArray bitArray in bitArrays)
        {
            length += bitArray.Length;
        }

        BitArray result = new BitArray(length, false);

        int resultArrayIndex = 0;


        for (int i = 0; i < bitArrays.Length; i++)
        {
            for (int j = 0; j < bitArrays[i].Length; j++)
            {
                result[resultArrayIndex++] = bitArrays[i][j];
            }
        }

        return result;
    }
    
    public static int ToInt(this BitArray bitArray)
    {
        if (bitArray.Length > 32)
            throw new ArgumentException("Length should not exceed 32");

        var array = new int[1];
        bitArray.CopyTo(array, 0);
        return array[0];
    }
}
