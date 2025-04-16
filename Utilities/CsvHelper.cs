using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dairy.Models;

namespace dairy.Utilities
{
    public static class CsvHelper
    {
        /// Načte CSV soubor a převede jeho obsah na seznam objektů DiaryEntry.
        public static List<DiaryEntry> ReadCsv(string filePath)
        {
            var entries = new List<DiaryEntry>();

            // Pokud soubor neexistuje, vrátí prázdný seznam
            if (!File.Exists(filePath)) return entries;

            // Načte všechny řádky, kromě hlavičky
            var lines = File.ReadAllLines(filePath).Skip(1);

            foreach (var line in lines)
            {
                // Rozdělení řádku podle čárek
                var parts = line.Split(',');

                // Kontrola minimálního počtu sloupců
                if (parts.Length < 4) continue;

                // Vytvoření nové položky deníku a přidání do seznamu
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

        // Zapíše seznam položek typu DiaryEntry do CSV souboru.
        public static void WriteCsv(string filePath, List<DiaryEntry> entries)
        {
            // Vytvoření hlavičky CSV
            var lines = new List<string> { "Id,User,Date,Title,Content" };

            // Převod jednotlivých záznamů na řádky CSV
            lines.AddRange(entries.Select(e => $"{e.Id},{e.User},{e.Date},{e.Title},{e.Description}"));

            // Přepsání celého souboru novými daty
            File.WriteAllLines(filePath, lines);
        }
    }
}
