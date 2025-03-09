using PublicCommon;

namespace Shared.DTOs;

public class TownCardsDto
    {
    public int Id { get; set; }
    public List<iCardDto>? VerifiedCards { get; set; } = new List<iCardDto>();
    public List<iCardDto>? DraftCards { get; set; } = new List<iCardDto>();
    public DateTime LastSyncedTime { get; set; }
    public DateTime LastAccessedTime { get; set; } = DateTimeExtension.CurrentTime;
    public DateTime LastCardUpdateTime { get; set; }

    public int UserCount { get; set; }
    }
