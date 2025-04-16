namespace dairy.Models;

// třída pro Zápis do diáře
public class DiaryEntry
{
    public int Id { get; set; }
    public string? User { get; set; }
    public required DateTime Date { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    
    // metoda pro výpis události v diáři
    public override string ToString()
    {
        return $"| {Date.ToShortDateString()} | ID: {Id} - {Title} - {Description}\n--------------------------------";
    }
}
