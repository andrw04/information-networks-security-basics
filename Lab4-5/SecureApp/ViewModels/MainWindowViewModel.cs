using System.Collections.ObjectModel;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using SecureApp.Models;
using SecureApp.Protection;
using SecureApp.Repositories;

namespace SecureApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IUserRepository _userRepository;
    private ObservableCollection<User> _users;
    private string _text;
    private string _path;

    public ObservableCollection<User> Users
    {
        get { return _users; }
        set
        {
            _users = value;
            OnPropertyChanged(nameof(Users));
        }
    }
    
    public bool IsAdmin
    {
        get
        {
            var principal = Thread.CurrentPrincipal;
            if (principal is not null && principal.Identity.IsAuthenticated)
            {
                var genericPrincipal = principal as GenericPrincipal;
                if (genericPrincipal is not null)
                    return genericPrincipal.IsInRole(Role.Admin.ToString());
            }

            return false;
        }
    }

    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            var isValid = BufferProtection.ValidateInput(value);

            if (isValid)
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
            else
            {
                MessageBox.Show($"Text is too long... Max text length is {BufferProtection.MaxLength}");
            }
        }
    }

    public string Path
    {
        get
        {
            return _path;
        }
        set
        {
            _path = value;
            OnPropertyChanged(nameof(Path));
        }
    }

    public ICommand RemoveCommand { get; }
    public ICommand ReadFile { get; }
    public ICommand HandleText { get; }
    
    public MainWindowViewModel()
    {
        _userRepository = new UserRepository();
        LoadUsers();
        RemoveCommand = new ViewModelCommand(ExecuteRemoveCommand, CanExecuteRemoveCommand);
        ReadFile = new ViewModelCommand(ExecuteReadFileCommand);
        HandleText = new ViewModelCommand(ExecuteHandleTextCommand);
    }

    private void ExecuteHandleTextCommand(object obj)
    {
        var text = XXSProtection.EscapeHtml(Text);
        MessageBox.Show($"Text: {text}");
    }

    private void ExecuteReadFileCommand(object obj)
    {
        try
        {
            var isValid = CanonizationProtection.IsValidFilePath(Path);

            if (!isValid)
            {
                MessageBox.Show("File Path is not valid!");
                return;
            }

            if (!File.Exists(Path))
            {
                MessageBox.Show("File is not exists.");
                return;
            }
            
            using (StreamReader reader = new StreamReader(Path))
            {
                Text = reader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception: {ex.Message}");
        }
    }

    private void LoadUsers()
    {
        var usersFromRepo = _userRepository.GetAll();
        Users = new ObservableCollection<User>(usersFromRepo);
    }

    private bool CanExecuteRemoveCommand(object obj)
    {
        if (obj is User user)
        {
            if (IsAdmin && _userRepository.GetById(user.Id) is not null && user.Role != Role.Admin)
            {
                return true;
            }
        }

        return false;
    }

    private void ExecuteRemoveCommand(object obj)
    {
        if (obj is User user)
        {
            Users.Remove(user);
            _userRepository.Remove(user.Id);
        }
    } 
}