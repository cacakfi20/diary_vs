using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dairy.Models;

namespace dairy.Utilities
{
    public static class CsvHelper
    {
        public static List<DiaryEntry> ReadCsv(string filePath)
        {
            var entries = new List<DiaryEntry>();
            if (!File.Exists(filePath)) return entries;

            var lines = File.ReadAllLines(filePath).Skip(1);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 4) continue;

                entries.Add(new DiaryEntry
                {
                    Id = int.Parse(parts[0]),
                    User = parts[1],
                    Date = DateTime.Parse(parts[2]),
                    Title = parts[3],
                    Description = parts[4]
                });
            }
            return entries;
        }

        public static void WriteCsv(string filePath, List<DiaryEntry> entries)
        {
            var lines = new List<string> { "Id,User,Date,Title,Content" };
            lines.AddRange(entries.Select(e => $"{e.Id},{e.User},{e.Date},{e.Title},{e.Description}"));
            File.WriteAllLines(filePath, lines);
        }
    }
}