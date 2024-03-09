using System.Collections;

namespace DataEncryptionStandard;

public static class DES
{
    public static byte[] Encrypt(byte[] text, byte[] key)
    {
        var keys = GetKeys(new BitArray(key));

        if (text.Length % 8 != 0)
        {
            var temp = text.ToList();
            temp.AddRange(Enumerable.Range(0, 8 - text.Length % 8).Select(x => (byte)x));
            text = temp.ToArray();
        }

        var blocks = new BitArray(text).SplitBy(64).ToArray();
        var result = new BitArray(text.Length * 8);

        for (int i = 0; i < blocks.Length; i++)
        {
            var block = ProcessBlock(blocks[i], keys);

            for (int j = 0; j < 64; j++)
            {
                result.Set(i * 64 + j, block[j]);
            }
        }

        var resultArray = new byte[text.Length];
        result.CopyTo(resultArray, 0);

        return resultArray;
    }

    public static byte[] Decrypt(byte[] text, byte[] key)
    {
        var keys = GetKeys(new BitArray(key));
        Array.Reverse(keys);

        var blocks = new BitArray(text).SplitBy(64).ToArray();
        var result = new BitArray(text.Length * 8);

        for (int i = 0; i < blocks.Length; i++)
        {
            var block = ProcessBlock(blocks[i], keys);

            for (int j = 0; j < 64; j++)
            {
                result.Set(i * 64 + j, block[j]);
            }
        }

        var resultArray = new byte[text.Length];
        result.CopyTo(resultArray, 0);

        var tailLen = 1;
        var endIndex = 1;
        while (tailLen < 7 && resultArray[^endIndex] - 1 == resultArray[^(endIndex + 1)])
        {
            tailLen++;
            endIndex++;
        }

        if (tailLen == 1 && resultArray[^1] != 0)
            tailLen = 0;

        resultArray = resultArray.Take(resultArray.Length - tailLen).ToArray();

        return resultArray;
    }

    private static BitArray ProcessBlock(BitArray block, BitArray[] keys)
    {
        if (block.Length != 64)
            throw new ArgumentException($"Binary text must be 64 length, but: {block.Length}");

        var initialPermutation = Permutation(block, PermutationTables.InitialPermutationTable);

        var parts = initialPermutation.SplitBy(32);

        var left = parts[0];
        var right = parts[1];

        for (int round = 0; round < 16; round++)
        {
            var expandedRight = Permutation(right, PermutationTables.ExpansionTable);

            var subKey = keys[round];
            var f = expandedRight.Xor(subKey);

            f = SBoxesSubstitution(f, PermutationTables.SBoxesTable);

            f = Permutation(f, PermutationTables.PBlocksTable);

            var temp = right;
            right = left.Xor(f);
            left = temp;
        }

        var combine = Utilities.Join(right, left);
        var result = Permutation(combine, PermutationTables.FinalPermutationTable);

        return result;
    }

    public static BitArray ShiftKey(BitArray key, int shift)
    {
        BitArray bits = key.GetSubArray(key.Length - shift - 1, shift);

        key.LeftShift(shift);

        for (int i = 0; i < bits.Length; i++)
        {
            key.Set(i, bits[i]);
        }

        return key;
    }

    public static BitArray[] GetKeys(BitArray key)
    {
        if (key.Length != 64)
            throw new ArgumentException($"Binary key must be 64 length, but: {key.Length}");

        BitArray key56 = Permutation(key, PermutationTables.PermutedChoice1Table);

        var parts = key56.SplitBy(28);

        var left = parts[0];
        var right = parts[1];

        BitArray[] keys = new BitArray[16];

        for (int i = 0; i < 16; i++)
        {
            left = ShiftKey(left, PermutationTables.KeyShiftsTable[i]);
            
            right = ShiftKey(right, PermutationTables.KeyShiftsTable[i]);

            var combine = Utilities.Join(left, right);

            var subKey = Permutation(combine, PermutationTables.CompressionTable);

            keys[i] = subKey;
        }

        return keys;
    }

    private static BitArray Permutation(BitArray original, int[] table)
    {
        BitArray result = new BitArray(table.Length);

        for (int i = 0; i < table.Length; i++)
        {
            result[i] = original[table[i] - 1];
        }

        return result;
    }

    private static BitArray SBoxesSubstitution(BitArray original, int[,,] sboxes)
    {
        var blocks = original.SplitBy(6);

        var result = new BitArray(32);
        for (int i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            var row = new BitArray(new[] { block[0], block[^1] }).ToInt();
            var column = new BitArray(Enumerable.Range(1, 4).Select(x => block[x]).ToArray()).ToInt();

            var val = sboxes[i, row, column];
            var bin = new BitArray(new int[] { val });

            for (int j = 0; j < 4; j++)
            {
                result.Set(i * 4 + j, bin[j]);
            }
        }

        return result;
    }
}
