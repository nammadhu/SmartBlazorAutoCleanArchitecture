using BASE;

namespace SHARED.DTOs;

public class TownCardsDto
    {
    public int Id { get; set; }
    public List<CardDto>? VerifiedCards { get; set; } = new List<CardDto>();
    public List<CardDto>? DraftCards { get; set; } = new List<CardDto>();
    public DateTime LastSyncedTime { get; set; }
    public DateTime LastAccessedTime { get; set; } = DateTimeExtension.CurrentTime;
    public DateTime LastCardUpdateTime { get; set; }

    public int UserCount { get; set; }
    }
