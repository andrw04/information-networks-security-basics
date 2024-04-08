using System.Configuration;
using System.Data;
using System.Windows;
using SecureApp.Views;

namespace SecureApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void ApplicationStart(object sender, StartupEventArgs e)
    {
        var loginView = new LoginView();
        loginView.Show();
    }
}