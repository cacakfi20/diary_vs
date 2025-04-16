using System;
using System.Security.Cryptography;
using System.Text;

namespace dairy.Utilities
{
    // Pomocná třída pro hashování hesel.
    public static class HashHelper
    {
        //Vygeneruje SHA-256 hash ze vstupního hesla.
        public static string HashPassword(string password)
        {
            // Vytvoří instanci SHA256 (použije implementaci systému)
            using (SHA256 sha256 = SHA256.Create())
            {
                // Převede vstupní řetězec na pole bajtů v UTF-8 kódování
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Převede hash na čitelný Base64 string
                return Convert.ToBase64String(bytes);
            }
        }
    }
}