using dairy.Interfaces;
using dairy.Models;
using dairy.Utilities;

namespace dairy.Services
{
    public class AuthService : IAuthService
    {
        // Cesta k souboru, kde jsou uloženi uživatelé (CSV formát)
        private readonly string _userFilePath = "./Data/users.csv";

        // Konstruktor – zajistí vytvoření souboru s hlavičkou, pokud ještě neexistuje
        public AuthService()
        {
            if (!File.Exists(_userFilePath))
            {
                Directory.CreateDirectory("./Data"); // vytvoří složku, pokud není
                File.WriteAllText(_userFilePath, "Username,HashedPassword\n"); // inicializuje soubor hlavičkou
            }
        }

        // Registrace nového uživatele
        public bool Register(string username, string password)
        {
            var users = GetAllUsers(); // Načtení všech uživatelů

            // Kontrola, zda už uživatel existuje
            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine("⚠️ Username already exists!");
                return false;
            }

            // Heslo se zahashuje pomocí utility
            string hashedPassword = HashHelper.HashPassword(password);

            // Přidání nového uživatele do CSV souboru
            File.AppendAllText(_userFilePath, $"{username};{hashedPassword}\n");

            Console.WriteLine("✅ Registration successful!");
            return true;
        }

        // Přihlášení uživatele
        public string Login(string username, string password)
        {
            var users = GetAllUsers();
            string hashedPassword = HashHelper.HashPassword(password);

            // Vyhledání uživatele podle jména a hashovaného hesla
            var user = users.FirstOrDefault(u => u.Username == username && u.getHashedPassword() == hashedPassword);

            // Pokud neexistuje, vrátí "0" jako chybový kód
            if (user == null || user.Username == null)
            {
                return "0";
            }

            // Jinak vrací jméno uživatele jako potvrzení přihlášení
            return user.Username;
        }

        // Načtení všech uživatelů ze souboru
        public List<User> GetAllUsers()
        {
            if (!File.Exists(_userFilePath)) 
                return new List<User>();

            return File.ReadAllLines(_userFilePath)
                .Skip(1) // přeskočí hlavičku
                .Select(line =>
                {
                    var parts = line.Split(';');
                    return new User { Username = parts[0], HashedPassword = parts[1] };
                }).ToList();
        }

        // Implementace rozhraní (přesměrování)
        List<User> IAuthService.GetAllUsers()
        {
            return GetAllUsers();
        }
    }
}