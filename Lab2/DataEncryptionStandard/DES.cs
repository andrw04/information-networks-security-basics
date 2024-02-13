using System.Text;

namespace DataEncryptionStandard;

public class DES
{
    private static int[] InitialPermutationTable =
    {   
        58,	50,	42,	34,	26,	18,	10,	2,	60,	52,	44,	36,	28,	20,	12,	4,
        62,	54,	46,	38,	30,	22,	14,	6,	64,	56,	48,	40,	32,	24,	16,	8,
        57,	49,	41,	33,	25,	17,	9,	1,	59,	51,	43,	35,	27,	19,	11,	3,
        61,	53,	45,	37,	29,	21,	13,	5,	63,	55,	47,	39,	31,	23,	15,	7,
    };
    
    private static int[] ExpansionTable =
    {
        32,	1,	2,	3,	4,	5,
        4,	5,	6,	7,	8,	9,
        8,	9,	10,	11,	12,	13,
        12,	13,	14,	15,	16,	17,
        16,	17,	18,	19,	20,	21,
        20,	21,	22,	23,	24,	25,
        24,	25,	26,	27,	28,	29,
        28,	29,	30,	31,	32,	1,
    };
    
        private static int[,,] SBoxesTable =
    {
        {
            { 14,	4,	13,	1,	2,	15,	11,	8,	3,	10,	6,	12,	5,	9,	0,	7, },
            { 0,	15,	7,	4,	14,	2,	13,	1,	10,	6,	12,	11,	9,	5,	3,	8, },
            { 4,	1,	14,	8,	13,	6,	2,	11,	15,	12,	9,	7,	3,	10,	5,	0, },
            { 15,	12,	8,	2,	4,	9,	1,	7,	5,	11,	3,	14,	10,	0,	6,	13, }
        },
        {
            { 15,	1,	8,	14,	6,	11,	3,	4,	9,	7,	2,	13,	12,	0,	5,	10, },
            { 3,	13,	4,	7,	15,	2,	8,	14,	12,	0,	1,	10,	6,	9,	11,	5, },
            { 0,	14,	7,	11,	10,	4, 13,	1,	5,	8,	12,	6,	9,	3,	2,	15, },
            { 13,	8,	10,	1,	3,	15,	4,	2,	11,	6,	7,	12,	0,	5,	14,	9, }
        },
        {
            { 10,	0,	9,	14,	6,	3,	15,	5,	1,	13,	12,	7,	11,	4,	2,	8, },
            { 13,	7,	0,	9,	3,	4,	6,	10,	2,	8,	5,	14,	12,	11,	15,	1, },
            { 13,	6,	4,	9,	8,	15,	3,	0,	11,	1,	2,	12,	5,	10,	14,	7, },
            { 1,	10,	13,	0,	6,	9,	8,	7,	4,	15,	14,	3,	11,	5,	2,	12, }
        },
        {
            { 7,	13,	14,	3,	0,	6,	9,	10,	1,	2,	8,	5,	11,	12,	4,	15, },
            { 13,	8,	11,	5,	6,	15,	0,	3,	4,	7,	2,	12,	1,	10,	14,	9, },
            { 10,	6,	9,	0,	12,	11,	7,	13,	15,	1,	3,	14,	5,	2,	8,	4, },
            { 3,	15,	0,	6,	10,	1,	13,	8,	9,	4,	5,	11,	12,	7,	2,	14, }
        },
        {
            { 2,	12,	4,	1,	7,	10,	11,	6,	8,	5,	3,	15,	13,	0,	14,	9, },
            { 14,	11,	2,	12,	4,	7,	13,	1,	5,	0,	15,	10,	3,	9,	8,	6, },
            { 4,	2,	1,	11,	10,	13,	7,	8,	15,	9,	12,	5,	6,	3,	0,	14, },
            { 11,	8,	12,	7,	1,	14,	2,	13,	6,	15,	0,	9,	10,	4,	5,	3, }
        },
        {
            { 12,	1,	10,	15,	9,	2,	6,	8,	0,	13,	3,	4,	14,	7,	5,	11, },
            { 10,	15,	4,	2,	7,	12,	9,	5,	6,	1,	13,	14,	0,	11,	3,	8, },
            { 9,	14,	15,	5,	2,	8,	12,	3,	7,	0,	4,	10,	1,	13,	11,	6, },
            { 4,	3,	2,	12,	9,	5,	15,	10,	11,	14,	1,	7,	6,	0,	8,	13, }
        },
        {
            { 4,	11,	2,	14,	15,	0,	8,	13,	3,	12,	9,	7,	5,	10,	6,	1, },
            { 13,	0,	11,	7,	4,	9,	1,	10,	14,	3,	5,	12,	2,	15,	8,	6, },
            { 1,	4,	11,	13,	12,	3,	7,	14,	10,	15,	6,	8,	0,	5,	9,	2, },
            { 6,	11,	13,	8,	1,	4,	10,	7,	9,	5,	0,	15,	14,	2,	3,	12, }
        },
        {
            { 13,	2,	8,	4,	6,	15,	11,	1,	10,	9,	3,	14,	5,	0,	12,	7, },
            { 1,	15,	13,	8,	10,	3,	7,	4,	12,	5,	6,	11,	0,	14,	9,	2, },
            { 7,	11,	4,	1,	9,	12,	14,	2,	0,	6,	10,	13,	15,	3,	5,	8, },
            { 2,	1,	14,	7,	4,	10,	8,	13,	15,	12,	9,	0,	3,	5,	6,	11, }
        }
    };
        
    // для конечной перестановки
    private static int[] PBlocksTable =
    {
        16,	7,	20,	21,	29,	12,	28,	17,
        1,	15,	23,	26,	5,	18,	31,	10,
        2,	8,	24,	14,	32,	27,	3,	9,
        19,	13,	30,	6,	22,	11,	4,	25,
    };

    private static int[] PermutedChoice1Table =
    {
        57,	49,	41,	33,	25,	17,	9,	1,	58,	50,	42,	34,	26,	18,	
        10,	2,	59,	51,	43,	35,	27,	19,	11,	3,	60,	52,	44,	36,	
        63,	55,	47,	39,	31,	23,	15,	7,	62,	54,	46,	38,	30,	22,	
        14,	6,	61,	53,	45,	37,	29,	21,	13,	5,	28,	20,	12,	4,
    };

    private static int[] KeyShiftsTable =
    {
        1,	1,	2,	2,	2,	2,	2,	2,	1,	2,	2,	2,	2,	2,	2,	1,
    };

    // Таблица для сжатия ключа из 56 бит до 48 бит
    private static int[] CompressionTable =
    {
        14,	17,	11,	24,	1,	5,	3,	28,	15,	6,	21,	10,	23,	19,	12,	4,
        26,	8,	16,	7,	27,	20,	13,	2,	41,	52,	31,	37,	47,	55,	30,	40,
        51,	45,	33,	48,	44,	49,	39,	56,	34,	53,	46,	42,	50,	36,	29,	32,
    };

    private static int[] FinalPermutationTable =
    {
        40,	8,	48,	16,	56,	24,	64,	32,	39,	7,	47,	15,	55,	23,	63,	31,
        38,	6,	46,	14,	54,	22,	62,	30,	37,	5,	45,	13,	53,	21,	61,	29,
        36,	4,	44,	12,	52,	20,	60,	28,	35,	3,	43,	11,	51,	19,	59,	27,
        34,	2,	42,	10,	50,	18,	58,	26,	33,	1,	41,	9,	49,	17,	57,	25,
    };

    private DES() {}
    
    public static string Encrypt(string binaryText, string binaryKey)
    {
        binaryKey = binaryKey.Substring(binaryKey.Length - 64);
        string[] keys = GetKeys(binaryKey);

        StringBuilder result = new StringBuilder();
        
        for (int i = 0; i < binaryText.Length; i += 64)
        {
            string binarySubstring = binaryText.Substring(i, 64);
            string encryptedBinarySubstring = MakeInitialPermitationWithRounds(binarySubstring, keys);
            result.Append(encryptedBinarySubstring);
        }
        
        return result.ToString();
    }
    
    public static string Decrypt(string binaryText, string binaryKey)
    {
        binaryKey = binaryKey.Substring(binaryKey.Length - 64);
        string[] keys = GetKeys(binaryKey);
        Array.Reverse(keys);

        StringBuilder result = new StringBuilder();
        
        for (int i = 0; i < binaryText.Length; i += 64)
        {
            string binarySubstring = binaryText.Substring(i, 64);
            string decryptedBinarySubstring = MakeInitialPermitationWithRounds(binarySubstring, keys);
            result.Append(decryptedBinarySubstring);
        }
        
        return result.ToString();
    }
    
    private static string MakeInitialPermitationWithRounds(string binaryText, string[] binaryKeys)
    {
        if (binaryText.Length != 64)
            throw new ArgumentException($"Binary Text must be 64 length, but: {binaryText.Length}");
        
        string initialPermutation = Permutation(binaryText, InitialPermutationTable);
    
        (string left, string right) = DivideEqually(initialPermutation);

        for (int round = 0; round < 16; round++)
        {
            string expandedR = Permutation(right, ExpansionTable);

            string subKey = binaryKeys[round];
            string f = Xor(expandedR, subKey);

            f = SBoxesSubstitution(f, SBoxesTable);

            f = Permutation(f, PBlocksTable);

            string temp = right;
            right = Xor(left, f);
            left = temp;
        }

        string combine = right + left;

        string result = Permutation(combine, FinalPermutationTable);

        return result;
    }
    
    public static string ConvertToBinaryString(string original)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(original);

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < bytes.Length; i += 8)
        {
            StringBuilder temp = new StringBuilder();
            temp.Append(new String('0', 64 - Math.Min(bytes.Length - i, 8) * 8));
            for (int j = i; j < Math.Min(bytes.Length, i + 8); j++)
            {
                temp.Append(Convert.ToString(bytes[j], 2).PadLeft(8,'0'));
            }

            sb.Append(temp);
        }

        return sb.ToString();
    }

    public static string ConvertFromBinaryString(string binary)
    {
        List<byte> byteList = new List<byte>();
        for (int i = 0; i < binary.Length; i += 8)
        {
            string binaryByte = binary.Substring(i, 8);
            byte byteValue = Convert.ToByte(binaryByte, 2);
            byteList.Add(byteValue);
        }

        return Encoding.UTF8.GetString(byteList.ToArray());
    }
    
    private static string[] GetKeys(string binaryKey)
    {
        if (binaryKey.Length != 64)
            throw new ArgumentException($"Binary key must be 64 length, but: {binaryKey.Length}");
        
        string key56 = Permutation(binaryKey, PermutedChoice1Table);
    
        (string keyLeft, string keyRight) = DivideEqually(key56);
    
        string[] keys = new string[16];

        for (int round = 0; round < 16; round++)
        {
            keyLeft = Shift(keyLeft, KeyShiftsTable[round]);
            keyRight = Shift(keyRight, KeyShiftsTable[round]);
    
            // Завершаем обработку ключа
            string subKey = Permutation(keyLeft + keyRight, CompressionTable);

            keys[round] = subKey;
        }

        return keys;
    }
    
    private static string Permutation(string original, int[] table)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < table.Length; i++)
        {
            result.Append(original[table[i] - 1]);
        }
    
        return result.ToString();
    }
    
    private static (string left, string right) DivideEqually(string original)
    {
        if (original.Length % 2 != 0)
            throw new ArgumentException("Length must be even.");

        int middle = original.Length / 2;
    
        return (original.Substring(0, middle), original.Substring(middle, middle));
    }
    
    private static string Shift(string original, int shift)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < original.Length; i++)
        {
            result.Append(original[(i + shift) % original.Length]);
        }

        return result.ToString();
    }
    
    private static string Xor(string first, string second)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < first.Length; i++)
        {
            if (first[i] == second[i])
                result.Append('0');
            else
                result.Append('1');
        }

        return result.ToString();
    }
    
    private static string SBoxesSubstitution(string original, int[,,] sboxes)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < 8; i++)
        {
            int row = Convert.ToInt32(original.Substring(i * 6, 6).Remove(1, 4), 2);
            int column = Convert.ToInt32(original.Substring(i * 6 + 1, 4), 2);

            result.Append(Convert.ToString(sboxes[i, row, column], 2).PadLeft(4, '0'));
        }

        return result.ToString();
    }
}