namespace SHARED.DTOs;

public class TownCardsGrouping
    {
    public int TypeId { get; set; }
    public string? TypeName { get; set; }

    public List<CardDto> Cards { get; set; } = new();
    }
