namespace SecureApp.Models;

public interface IUserRepository
{
    User? AuthenticateUser(string username, string password);
    void Add(User user);
    void Edit(User user);
    void Remove(int id);
    User? GetById(int id);
    IEnumerable<User> GetAll();
}