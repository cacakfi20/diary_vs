using System.Collections.Generic;
using System.Globalization;
using dairy.Models;

// interface servicy pro pracování s diářem 
namespace dairy.Interfaces
{
    public interface IDiaryService
    {
        void AddEntry(DiaryEntry entry);
        List<DiaryEntry> GetAllEntries();
        List<DiaryEntry> GetEntriesByDate(string user);
        List<DiaryEntry> GetAllEntriesByUser(string user);
        List<DiaryEntry> SearchEntriesByKeyword(string user, string keyword);
        DiaryEntry GetEntryById(int id);
        void UpdateEntry(DiaryEntry entry);
        void DeleteEntry(int id);
        void ShowHeader(string title);
        void ShowFooter();
    }
}