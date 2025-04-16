using dairy.Models;

//interface pro funkce authorizační servicy
namespace dairy.Interfaces
{
    public interface IAuthService
    {
        bool Register(string username, string password);
        string Login(string username, string password);
        List<User> GetAllUsers();
    }
}
