using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernWpf.Controls;
using ModernListViewItem = ModernWpf.Controls.ListViewItem;

namespace WpfEncryptor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        pagesList.SelectionChanged += PagesList_OnSelectionChanged;
        contentFrame.Navigate(new CesarPage());
    }

    private void PagesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (pagesList.SelectedItem != null)
        {
            ModernListViewItem selectedItem = (pagesList.SelectedItem as ModernListViewItem)!;
            string tag = selectedItem.Tag.ToString()!;

            switch (tag)
            {
                case "Cesar":
                    contentFrame.Navigate(new CesarPage());
                    break;
                case "Vigenere":
                    contentFrame.Navigate(new VigenerePage());
                    break;
            }
        }
    }
}