using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using EncryptionLibrary.Realizations;
using Microsoft.Win32;

namespace WpfEncryptor;

public partial class CesarPage : Page, INotifyPropertyChanged
{
    private int _shift;
    
    private string _inputFilePath = String.Empty;
    private string _outputFilePath = String.Empty;
    private string _inputText = String.Empty;
    private string _outputText = String.Empty;

    private const string _alphabet = "abcdefghijklmnopqrstuvwxyz";

    public string InputFilePath
    {
        get => _inputFilePath;
        set
        {
            _inputFilePath = value;
            OnPropertyChanged(nameof(InputFilePath));
        }
    }

    public string OutputFilePath
    {
        get => _outputFilePath;
        set
        {
            _outputFilePath = value;
            OnPropertyChanged(nameof(OutputFilePath));
        }
    }

    public string InputText
    {
        get => _inputText;
        set
        {
            _inputText = value;
            OnPropertyChanged(nameof(InputText));
        }
    }

    public string OutputText
    {
        get => _outputText;
        set
        {
            _outputText = value;
            OnPropertyChanged(nameof(OutputText));
        }
    }

    public int ShiftValue
    {
        get { return _shift; }
        set
        {
            _shift = value;
            OnPropertyChanged(nameof(ShiftValue));
        }
    }

    public CesarPage()
    {
        InitializeComponent();
        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void ChooseInputFile_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

        bool isOpened = openFileDialog.ShowDialog() ?? false;

        if (isOpened)
        {
            InputFilePath = openFileDialog.FileName;
        }
    }

    private void ChooseOutputFile_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

        bool isOpened = openFileDialog.ShowDialog() ?? false;

        if (isOpened)
        {
            OutputFilePath = openFileDialog.FileName;
        }
    }

    private void EncryptString_OnClick(object sender, RoutedEventArgs e)
    {
        if (!String.IsNullOrEmpty(InputText))
        {
            var encryptor = new Encryptor();
            encryptor.SetStrategy(new CesarCipher(_alphabet, ShiftValue));

            OutputText = encryptor.Encrypt(InputText);
        }
    }
    
    private void DecryptString_OnClick(object sender, RoutedEventArgs e)
    {
        if (!String.IsNullOrEmpty(InputText))
        {
            var encryptor = new Encryptor();
            encryptor.SetStrategy(new CesarCipher(_alphabet, ShiftValue));

            OutputText = encryptor.Decrypt(InputText);
        }
    }

    private void EncryptFile_OnClick(object sender, RoutedEventArgs e)
    {
        if (!File.Exists(InputFilePath) || !File.Exists(OutputFilePath))
        {
            MessageBox.Show("Incorrect file path!");
        }
        else
        {
            var encryptor = new Encryptor();
            encryptor.SetStrategy(new CesarCipher(_alphabet, ShiftValue));
            encryptor.EncryptFile(InputFilePath, OutputFilePath);
            MessageBox.Show("Success!");
        }
    }
    
    private void DecryptFile_OnClick(object sender, RoutedEventArgs e)
    {
        if (!File.Exists(InputFilePath))
        {
            MessageBox.Show("Incorrect file path!");
        }
        else
        {
            var encryptor = new Encryptor();
            encryptor.SetStrategy(new CesarCipher(_alphabet, ShiftValue));
            encryptor.DecryptFile(InputFilePath, OutputFilePath);
            MessageBox.Show("Success!");
        }
    }
}