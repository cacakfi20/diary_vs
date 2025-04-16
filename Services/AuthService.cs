using dairy.Interfaces;
using dairy.Models;
using dairy.Utilities;

namespace dairy.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _userFilePath = "./Data/users.csv";

        public AuthService()
        {
            if (!File.Exists(_userFilePath))
            {
                Directory.CreateDirectory("./Data");
                File.WriteAllText(_userFilePath, "Username,HashedPassword\n");
            }
        }

        public bool Register(string username, string password)
        {
            var users = GetAllUsers();
            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine("⚠️ Username already exists!");
                return false;
            }

            string hashedPassword = HashHelper.HashPassword(password);

            File.AppendAllText(_userFilePath, $"{username},{hashedPassword}\n");
            Console.WriteLine("✅ Registration successful!");
            return true;
        }

        public string Login(string username, string password)
        {
            var users = GetAllUsers();
            string hashedPassword = HashHelper.HashPassword(password);
            var user = users.FirstOrDefault(u => u.Username == username && u.getHashedPassword() == hashedPassword);

            if (user == null || user.Username == null)
            {
                return "0";
            }
            return user.Username;
        }

        public List<User> GetAllUsers()
        {
            if (!File.Exists(_userFilePath)) return new List<User>();

            return File.ReadAllLines(_userFilePath)
                .Skip(1)
                .Select(line =>
                {
                    var parts = line.Split(',');
                    return new User { Username = parts[0], HashedPassword = parts[1] };
                }).ToList();
        }

        List<User> IAuthService.GetAllUsers()
        {
            return GetAllUsers();
        }
    }
}