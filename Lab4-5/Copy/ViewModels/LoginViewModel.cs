using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using SecureApp.Models;
using SecureApp.Repositories;
using SecureApp.Views;

namespace SecureApp.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _username;
    private string _password;
    private string _errorMessage;

    private IUserRepository _userRepository;

    public string Username
    {
        get
        {
            return _username;
        }
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public string Password
    {
        get
        {
            return _password;
        }
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public string ErrorMessage
    {
        get
        {
            return _errorMessage;
        }
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }
    
    public ICommand LoginCommand { get; }
    public ICommand ShowPasswordCommand { get; }

    public LoginViewModel()
    {
        _userRepository = new UserRepository();
        LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
    }

    private bool CanExecuteLoginCommand(object obj)
    {
        bool validData = true;
        if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 || Password == null || Password.Length < 8)
            validData = false;

        return validData;
    }

    private void ExecuteLoginCommand(object obj)
    {
        var user = _userRepository.AuthenticateUser(Username, Password);
        if (user is not null)
        {
            Thread.CurrentPrincipal = new GenericPrincipal(
                new GenericIdentity(Username), [user.Role.ToString()]);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            CloseWindow();
        }
        else
        {
            ErrorMessage = "* Invalid username or password";
        }
    }

    private void CloseWindow()
    {
        Window window = Application.Current.Windows.OfType<LoginView>().FirstOrDefault(w => w.DataContext == this);
        if (window is not null)
        {
            window.Close();
        }
    }
}