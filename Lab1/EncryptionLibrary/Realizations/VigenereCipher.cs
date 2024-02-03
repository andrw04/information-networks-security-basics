﻿using System.Text;
using EncryptionLibrary.Abstractions;

namespace EncryptionLibrary.Realizations;

public class VigenereCipher : IEncryptStrategy
{
    private Dictionary<char, int>? _indexes;
    private char[]? _symbols;
    private CesarCipher? _cesarCipher;
    private string _key;
    
    public VigenereCipher(string alphabet, string key)
    {
        SetAlphabet(alphabet);
        _cesarCipher = new CesarCipher(alphabet);
        SetKey(key);
    }
    
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

    public void SetKey(string key)
    {
        if (_indexes is not null)
        {
            if (key.Length > _indexes.Count)
            {
                _key = key.Substring(0, _indexes.Count);
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                while (sb.Length < _indexes?.Count)
                {
                    sb.Append(key);
                }

                _key = sb.ToString().Substring(0, _indexes.Count);
            }
        }
    }
    
    public string Encrypt(string text)
    {
        if (_indexes is null || _symbols is null || _cesarCipher is null)
            throw new InvalidOperationException("Alphabet is not defined.");

        if (_key.Length != text.Length)
            throw new ArgumentException("Key length is not equal text length");

        StringBuilder enctyptedString = new StringBuilder();

        for (var i = 0; i < text.Length; i++)
        {
            int shift;

            if (_indexes.TryGetValue(text[i], out shift))
            {
                _cesarCipher.SetShift(shift);

                string symbol = _cesarCipher.Encrypt(text[i].ToString().ToLower());
                
                if (text[i] == char.ToUpper(text[i]))
                    symbol = symbol.ToUpper();

                enctyptedString.Append(symbol);
            }
            else
            {
                enctyptedString.Append(text[i]);
            }
        }

        return enctyptedString.ToString();
    }

    public string Decrypt(string text)
    {
        if (_indexes is null || _symbols is null || _cesarCipher is null)
            throw new InvalidOperationException("Alphabet is not defined.");

        if (_key.Length != text.Length)
            throw new ArgumentException("Key length is not equal text length");

        StringBuilder dectyptedString = new StringBuilder();

        for (var i = 0; i < _key.Length; i++)
        {
            int index;
            if (_indexes.TryGetValue(_key[i], out index))
            {
                _cesarCipher.SetShift(index);

                string symbol = _cesarCipher.Decrypt(text[i].ToString().ToLower());
                
                if (text[i] == char.ToUpper(text[i]))
                    symbol = symbol.ToUpper();
                
                dectyptedString.Append(symbol);
            }
            else
            {
                dectyptedString.Append(text[i]);
            }
        }

        return dectyptedString.ToString();
    }
}