using SecureApp.Models;

namespace SecureApp.Repositories;

public class UserRepository : IUserRepository
{
    public static Dictionary<string, User> _users = new Dictionary<string, User>();

    public UserRepository()
    {
        if (_users.Count == 0)
        {
            _users.Add("admin", new User()
            {
                Id = 1,
                Email = "admin@gmail.com",
                LastName = "LastName",
                Name = "Name",
                Password = "12345678",
                Username = "admin",
                Role = Role.Admin
            });
            _users.Add("user", new User()
            {
                Id = 2,
                Email = "user@gmail.com",
                LastName = "LastName",
                Name = "Name",
                Password = "12345678",
                Username = "user"
            });
            _users.Add("bob", new User()
            {
                Id = 3,
                Email = "bob@gmail.com",
                LastName = "Bob",
                Name = "Bob",
                Password = "12345678",
                Username = "bob"
            });
            _users.Add("tom", new User()
            {
                Id = 4,
                Email = "tom@gmail.com",
                LastName = "Tom",
                Name = "Tom",
                Password = "12345678",
                Username = "tom"
            });
            _users.Add("alex", new User()
            {
                Id = 5,
                Email = "alex@gmail.com",
                LastName = "Alex",
                Name = "Alex",
                Password = "12345678",
                Username = "alex"
            });
        }
    }
    
    public User? AuthenticateUser(string username, string password)
    {
        if (_users.ContainsKey(username) && _users[username].Password == password)
            return _users[username];
        return null;
    }

    public void Add(User user)
    {
        if (!_users.ContainsKey(user.Username))
        {
            _users.Add(user.Username, user);
        }
    }

    public void Edit(User user)
    {
        if (_users.ContainsKey(user.Username))
        {
            _users.Remove(user.Username);
            
            _users.Add(user.Username, user);
        }
    }

    public void Remove(int id)
    {
        foreach (var user in _users)
        {
            if (user.Value.Id == id)
            {
                _users.Remove(user.Key);
            }
        }
    }

    public User? GetById(int id)
    {
        foreach (var user in _users)
        {
            if (user.Value.Id == id)
            {
                return user.Value;
            }
        }

        return null;
    }

    public IEnumerable<User> GetAll()
    {
        return _users.Values.ToArray();
    }
}