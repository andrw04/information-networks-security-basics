using System.Windows;
using System.Windows.Controls;
using SecureApp.ViewModels;

namespace SecureApp.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (this.DataContext != null)
        {
            ((LoginViewModel)this.DataContext).Password = ((PasswordBox)sender).Password;
        }
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}