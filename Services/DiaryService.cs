using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using dairy.Interfaces;
using dairy.Models;
using dairy.Utilities;

namespace dairy.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly string _directoryPath = "./Data";
        private readonly string _filePath;

        public DiaryService()
        {
            _filePath = Path.Combine(_directoryPath, "diary_entries.csv");

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "Id,Date,Title,Content\n");
            }
        }

        public void AddEntry(DiaryEntry entry)
        {
            entry.Id = GetNextId();
            string csvLine = $"{entry.Id},{entry.User},{entry.Date},{entry.Title},{entry.Description}";
            File.AppendAllText(_filePath, csvLine + Environment.NewLine);
        }

        public List<DiaryEntry> GetAllEntries()
        {   
            return CsvHelper.ReadCsv(_filePath);
        }

        public List<DiaryEntry> GetAllEntriesByUser(string user)
        {
            List<DiaryEntry> entries = CsvHelper.ReadCsv(_filePath);
            List<DiaryEntry> filteredEntries = new List<DiaryEntry>();
            foreach (var entry in entries)
            {
                if (entry.User == user)
                {
                    filteredEntries.Add(entry);
                }
            }
            return filteredEntries;
        }

        public List<DiaryEntry> GetEntriesByDate(string user)
        {
            var entries = GetAllEntriesByUser(user);
            var sorted = entries.OrderBy(e => e.Date).ToList();
            return sorted;
        }

        public void DeleteEntry(int id)
        {
            var entries = GetAllEntries().Where(e => e.Id != id).ToList();
            CsvHelper.WriteCsv(_filePath, entries);
        }

        public void ShowHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{title}");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("|    Date    | Title - Content");
            Console.WriteLine("--------------------------------");
            Console.ResetColor();
        }

        public void ShowFooter()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.ResetColor();
        }

        private int GetNextId()
        {
            var entries = GetAllEntries();
            return entries.Count == 0 ? 1 : entries.Max(e => e.Id) + 1;
        }

        public List<DiaryEntry> SearchEntriesByKeyword(string user, string keyword)
        {
            var entries = GetAllEntriesByUser(user);
            return entries
                .Where(entry =>
                    (!string.IsNullOrEmpty(entry.Title) && entry.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(entry.Description) && entry.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }
}