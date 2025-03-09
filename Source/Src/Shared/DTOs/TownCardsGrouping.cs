namespace Shared.DTOs;

public class TownCardsGrouping
    {
    public int TypeId { get; set; }
    public string? TypeName { get; set; }

    public List<iCardDto> Cards { get; set; } = new();
    }
