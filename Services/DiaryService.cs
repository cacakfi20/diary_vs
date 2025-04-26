using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dairy.Interfaces;
using dairy.Models;
using dairy.Utilities;

namespace dairy.Services
{
    public class DiaryService : IDiaryService
    {
        // Cesta k adresáři a souboru s daty
        private readonly string _directoryPath = "./Data";
        private readonly string _filePath;

        public DiaryService()
        {
            // Sestavení plné cesty k CSV souboru
            _filePath = Path.Combine(_directoryPath, "diary_entries.csv");

            // Vytvoření adresáře, pokud neexistuje
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            // Vytvoření CSV souboru s hlavičkou, pokud neexistuje
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "Id;User;Date;Title;Content\n");
            }
        }

        // Přidání nové položky do deníku
        public void AddEntry(DiaryEntry entry)
        {
            entry.Id = GetNextId(); // Automatické přidělení ID
            string csvLine = $"{entry.Id};{entry.User};{entry.Date};{entry.Title};{entry.Description}";
            File.AppendAllText(_filePath, csvLine + Environment.NewLine);
        }

        // Načte všechny záznamy bez ohledu na uživatele
        public List<DiaryEntry> GetAllEntries()
        {
            return CsvHelper.ReadCsv(_filePath); // Pomocná utilita pro čtení CSV
        }

        // Načte všechny záznamy patřící konkrétnímu uživateli
        public List<DiaryEntry> GetAllEntriesByUser(string user)
        {
            List<DiaryEntry> entries = CsvHelper.ReadCsv(_filePath);
            List<DiaryEntry> filteredEntries = new List<DiaryEntry>();

            // Ruční filtrování podle jména uživatele
            foreach (var entry in entries)
            {
                if (entry.User == user)
                {
                    filteredEntries.Add(entry);
                }
            }

            return filteredEntries;
        }

        // Vrací seznam záznamů daného uživatele seřazený podle data
        public List<DiaryEntry> GetEntriesByDate(string user)
        {
            var entries = GetAllEntriesByUser(user);
            var sorted = entries.OrderBy(e => e.Date).ToList();
            return sorted;
        }

        // Smaže záznam podle ID (přepíše celý CSV soubor bez daného záznamu)
        public void DeleteEntry(int id)
        {
            var entries = GetAllEntries().Where(e => e.Id != id).ToList();
            CsvHelper.WriteCsv(_filePath, entries); // Pomocná utilita pro zápis do CSV z helper třífy
        }

        // Zobrazí hlavičku tabulky v konzoli (barevně)
        public void ShowHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{title}");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("|    Date    | Title - Content");
            Console.WriteLine("--------------------------------");
            Console.ResetColor();
        }

        // Pauzne aplikaci a vyzve uživatele ke stisknutí klávesy
        public void ShowFooter()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.ResetColor();
        }

        // Získá další volné ID záznamu (auto-increment logika)
        private int GetNextId()
        {
            var entries = GetAllEntries();
            return entries.Count == 0 ? 1 : entries.Max(e => e.Id) + 1;
        }

        // Vyhledání záznamů podle klíčového slova v názvu nebo obsahu
        public List<DiaryEntry> SearchEntriesByKeyword(string user, string keyword)
        {
            var entries = GetAllEntriesByUser(user);

            return entries
                .Where(entry =>
                    (!string.IsNullOrEmpty(entry.Title) && entry.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(entry.Description) && entry.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public DiaryEntry GetEntryById(int id)
        {
            var entries = GetAllEntries();
            var entry = entries.FirstOrDefault(e => e.Id == id);
            return entry;
        }

        public void UpdateEntry(DiaryEntry entry)
        {
            var entries = GetAllEntries();
            var existingEntry = entries.FirstOrDefault(e => e.Id == entry.Id);

            if (existingEntry != null)
            {
                // Aktualizace existujícího záznamu
                existingEntry.User = entry.User;
                existingEntry.Date = entry.Date;
                existingEntry.Title = entry.Title;
                existingEntry.Description = entry.Description;

                CsvHelper.WriteCsv(_filePath, entries);
            }
        }
    }
}