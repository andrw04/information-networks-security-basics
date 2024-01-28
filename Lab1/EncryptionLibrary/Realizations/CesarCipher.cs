﻿using System.Text;
using EncryptionLibrary.Abstractions;

namespace EncryptionLibrary.Realizations;

public class CesarCipher : IEncryptStrategy
{
    private Dictionary<char, int>? _indexes;
    private char[]? _symbols;
    private int _shift;

    public CesarCipher(string alphabet, int shift = 0)
    {
        SetAlphabet(alphabet);
        SetShift(shift);
    }

    public void SetShift(int shift) => _shift = shift;
    
    public void SetAlphabet(string alphabet)
    {
        alphabet = alphabet.ToLower();
        
        _indexes = new Dictionary<char, int>();
        for (var i = 0; i < alphabet.Length; i++)
        {
            _indexes.Add(alphabet[i], i);
        }

        _symbols = alphabet.ToArray();
    }
    
    public string Encrypt(string text)
    {
        if (_indexes is null || _symbols is null)
            throw new InvalidOperationException("Alphabet is not defined.");
        
        StringBuilder encryptedString = new StringBuilder();

        for (var i = 0; i < text.Length; i++)
        {
            int x;
            
            if (_indexes.TryGetValue(char.ToLower(text[i]), out x))
            {
                int y = ((x + _shift < 0 ? x + _indexes.Count  : x) + _shift) % _indexes.Count;

                char symbol = _symbols[y];

                if (text[i] == char.ToUpper(text[i]))
                    symbol = char.ToUpper(symbol);

                encryptedString.Append(symbol);
            }
            else
            {
                encryptedString.Append(text[i]);
            }
        }
        
        return encryptedString.ToString();
    }

    public string Decrypt(string text)
    {
        if (_indexes is null || _symbols is null)
            throw new InvalidOperationException("Alphabet is not defined.");
        
        StringBuilder decryptedString = new StringBuilder();

        for (var i = 0; i < text.Length; i++)
        {
            int y;

            if (_indexes.TryGetValue(char.ToLower(text[i]), out y))
            {
                int x = ((y - _shift  < 0 ? y + _indexes.Count : y) - _shift) % _indexes.Count;
                
                char symbol = _symbols[x];
                
                if (text[i] == char.ToUpper(text[i]))
                    symbol = char.ToUpper(symbol);
                
                decryptedString.Append(symbol);
            }
            else
            {
                decryptedString.Append(text[i]);
            }
        }

        return decryptedString.ToString();
    }
}