using System;
using dairy.Models;
using dairy.Services;
using dairy.Interfaces;
using System.Security.Cryptography.X509Certificates;

class Program
{
    static void Main()
    {
        // Inicializace služeb pro autentizaci a práci s deníkem
        IAuthService authService = new AuthService();
        IDiaryService diaryService = new DiaryService();

        string? loggedInUser = null;

        // Nastavení výstupu na UTF-8 pro podporu emoji
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // První loop – přihlašovací obrazovka, dokud se někdo nepřihlásí, nezaregistruje nebo neodejde
        while (loggedInUser == null)
        {
            Console.WriteLine("\n🔒 Diary App - Authentication");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": // Registrace
                    Console.Write("Enter username: ");
                    string? newUsername = Console.ReadLine();
                    Console.Write("Enter password: ");
                    string? newPassword = Console.ReadLine();

                    // Kontrola prázdných vstupů a pokus o registraci
                    if (newUsername != null && newPassword != null && newUsername.Trim() != "" && newPassword.Trim() != "" && authService.Register(newUsername, newPassword))
                    {
                        loggedInUser = newUsername;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Username or password cannot be empty.");
                        Console.ResetColor();
                        diaryService.ShowFooter();
                    }
                    break;

                case "2": // Přihlášení
                    Console.Write("Enter username: ");
                    string? username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    string? password = Console.ReadLine();

                    // Kontrola prázdných vstupů a pokus o přihlášení
                    if (username == null || password == null || username.Trim() == "" || password.Trim() == "")
                    {
                        Console.WriteLine("Username or password cannot be empty.");
                        break;
                    }

                    // Přihlášení uživatele
                    string loginRes = authService.Login(username, password);
                    if (loginRes != "0")
                    {
                        Console.WriteLine("✅ Login successful!");
                        loggedInUser = username;
                    }
                    else
                    {
                        Console.WriteLine("❌ Invalid username or password.");
                        diaryService.ShowFooter();
                        Console.Clear();
                    }
                    break;

                case "3": // Konec aplikace
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        // Hlavní menu po přihlášení
        while (true)
        {
            Console.WriteLine("\n📖 Diary App - Welcome, " + loggedInUser);
            Console.WriteLine("1. Add New Event");
            Console.WriteLine("2. View All Events");
            Console.WriteLine("3. View All Events by Date");
            Console.WriteLine("4. Delete Event by Id");
            Console.WriteLine("5. Search Entries by Keyword");
            Console.WriteLine("6. Edit Event by Id");
            Console.WriteLine("7. Logout");
            Console.Write("Choose an option: ");
            
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": // Přidání nové události
                    Console.Write("Date: ");
                    // Pokud není datum validní zvolí se dnešní datum
                    string date = Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd");

                    // Validace data
                    if (!DateTime.TryParse(date, out DateTime entryDate))
                    {
                        Console.WriteLine("Invalid date format. Using current date.");
                        entryDate = DateTime.Now;
                    }
                    Console.Write("Title: ");
                    string? title = Console.ReadLine();
                    Console.Write("Content: ");
                    string? content = Console.ReadLine();

                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine("Title and content cannot be empty.");
                        break;
                    }
                    // Uložení záznamu
                    diaryService.AddEntry(new DiaryEntry {User = loggedInUser, Date = entryDate, Title = title, Description = content });
                    Console.WriteLine("✅ Entry added.");
                    diaryService.ShowFooter();
                    Console.Clear();
                    break;
                
                case "2": // Zobrazení všech záznamů
                    var entries = diaryService.GetAllEntriesByUser(loggedInUser);
                    if (entries.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No entries found.");
                        Console.ResetColor();
                    } else {
                        diaryService.ShowHeader("Your Diary Entries");
                    }
                    foreach (var entry in entries)
                    {
                        Console.WriteLine(entry);
                    }
                    diaryService.ShowFooter();
                    Console.Clear();
                    break;
                
                case "3": // Zobrazení záznamů seřazených podle data
                    var sortedEntries = diaryService.GetEntriesByDate(loggedInUser);
                    if (sortedEntries.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No entries found.");
                        Console.ResetColor();
                    } else {
                        diaryService.ShowHeader("Your Diary Entries sorted by date");
                    }
                    foreach (var entry in sortedEntries)
                    {
                        Console.WriteLine(entry);
                    }
                    diaryService.ShowFooter();
                    Console.Clear();
                    break;

                case "4": // Smazání záznamu podle ID
                    var entriesToDelete = diaryService.GetAllEntriesByUser(loggedInUser);
                    if (entriesToDelete.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No entries to delete found.");
                        Console.ResetColor();
                    } else {
                        diaryService.ShowHeader("Your Diary Entries");
                    }
                    foreach (var entry in entriesToDelete)
                    {
                        Console.WriteLine(entry);
                    }
                    Console.Write("Enter Id to delete: ");
                    if (Int32.TryParse(Console.ReadLine(), out int id))
                    {
                        diaryService.DeleteEntry(id);
                        Console.WriteLine("✅ Entry deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format.");
                    }
                    break;

                case "5": // Vyhledání záznamů podle klíčového slova
                    Console.Write("Enter keyword to search: ");
                    string? keyword = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        Console.WriteLine("Keyword cannot be empty.");
                        break;
                    }

                    var matchedEntries = diaryService.SearchEntriesByKeyword(loggedInUser, keyword);

                    if (matchedEntries.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No matching entries found.");
                        Console.ResetColor();
                    }
                    else
                    {
                        diaryService.ShowHeader($"Search results for \"{keyword}\"");
                        foreach (var entry in matchedEntries)
                        {
                            Console.WriteLine(entry);
                        }
                    }
                    diaryService.ShowFooter();
                    Console.Clear();
                    break;

                case "6": // Editace existujícího záznamu podle ID
                    var entriesToEdit = diaryService.GetAllEntriesByUser(loggedInUser);
                    if (entriesToEdit.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No entries to edit found.");
                        Console.ResetColor();
                    }
                    else
                    {
                        diaryService.ShowHeader("Your Diary Entries");
                        foreach (var entry in entriesToEdit)
                        {
                            Console.WriteLine(entry);
                        }

                        Console.Write("Enter Id of the event you want to edit: ");
                        if (Int32.TryParse(Console.ReadLine(), out int editId))
                        {
                            var entryToEdit = diaryService.GetEntryById(editId);
                            if (entryToEdit == null)
                            {
                                Console.WriteLine("❌ Entry not found.");
                                diaryService.ShowFooter();
                                Console.Clear();
                                break;
                            }

                            Console.WriteLine($"Editing Entry: {entryToEdit}");

                            Console.Write("New Date (leave empty to keep current): ");
                            string? newDateInput = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newDateInput) && DateTime.TryParse(newDateInput, out DateTime newDate))
                            {
                                entryToEdit.Date = newDate;
                            }

                            Console.Write("New Title (leave empty to keep current): ");
                            string? newTitle = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newTitle))
                            {
                                entryToEdit.Title = newTitle;
                            }

                            Console.Write("New Content (leave empty to keep current): ");
                            string? newContent = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newContent))
                            {
                                entryToEdit.Description = newContent;
                            }

                            diaryService.UpdateEntry(entryToEdit);
                            Console.WriteLine("✅ Entry updated.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID format.");
                        }
                    }
                    diaryService.ShowFooter();
                    Console.Clear();
                    break;

                case "7": // Odhlášení a restart aplikace
                    loggedInUser = null;
                    Console.WriteLine("🔓 Logged out.");
                    Main(); // Rekurzivní restart hlavní metody
                    return;
                
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}