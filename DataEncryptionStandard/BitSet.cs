using System.Text;

namespace DataEncryptionStandard;

public class BitSet
{
    private char[] _data;
    private int _bitCount;

    public int Count { get => _bitCount; }

    public BitSet(int count = 64)
    {
        if (count < 0)
            throw new ArgumentException("Index must be greater than 0.");
        _bitCount = count;
        _data = new char[_bitCount / 8 == 0 ? 1 : _bitCount / 8];
        Clear();
    }

    public BitSet(string str)
    {
        _bitCount = str.Length;
        _data = str.ToCharArray();
    }

    public void Set(int index)
    {
        if (index > _bitCount - 1 || index < 0)
            throw new ArgumentOutOfRangeException(nameof(index),$"Index must be between 0 and {_bitCount - 1}");

        byte currentByte = (byte)_data[index / 8];
        byte mask = (byte)(1 << (8 - (index % 8)) - 1);

        _data[index / 8] = (char)(currentByte | mask);
    }

    public bool Get(int index)
    {
        if (index > _bitCount - 1 || index < 0)
            throw new ArgumentOutOfRangeException(nameof(index),$"Index must be between 0 and {_bitCount - 1}");

        byte currentByte = (byte)_data[index / 8];
        byte mask = (byte)(1 << (8 - (index % 8)) - 1);

        return Convert.ToBoolean(currentByte & mask);
    }

    public void Swap(int firstIndex, int secondIndex)
    {
        bool first = Get(firstIndex);
        bool second = Get(secondIndex);
        
        if (first)
            Set(secondIndex);
        else
            Clear(secondIndex);
        
        if (second)
            Set(firstIndex);
        else
            Clear(firstIndex);
    }

    public void Clear()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = (char)0;
        }
    }

    public void Clear(int index)
    {
        if (index > _bitCount - 1 || index < 0)
            throw new ArgumentOutOfRangeException(nameof(index),$"Index must be between 0 and {_bitCount - 1}");
        
        byte currentByte = (byte)_data[index / 8];
        byte mask = (byte)~(1 << (8 - (index % 8)) - 1);
        
        _data[index / 8] = (char)(currentByte & mask);
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < _data.Length; i++)
        {
            result.Append(Convert.ToString(_data[i], 2).PadLeft(8, '0'));
        }

        return result.ToString().Substring(0, _bitCount);
    }
}