namespace dairy.Models
{
    //třída uživatele
    public class User
    {
        public int Id { get; set; }
        public required string? Username { get; set; }
        public required string? HashedPassword { private get; set; }

        //metoda pro vrácení zahashovaného hesla (má nastavený private getter)
        public string? getHashedPassword() {
            return HashedPassword;
        }
    }
}