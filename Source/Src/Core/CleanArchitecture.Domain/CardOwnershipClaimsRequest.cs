using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain;

public class CardOwnershipClaimsRequest : AuditableBaseEntity
    {
    //idcard can be duplicate entries,as multiple users might be claiming
    public int IdCard { get; set; }

    [ForeignKey(nameof(IdCard))]
    public virtual Card iCard { get; set; } = default!;

    public int IdTown { get; set; }

    [ForeignKey(nameof(IdCard))]
    public virtual Town? Town { get; set; }

    public Guid RequestorUserId { get; set; }

    //[ForeignKey(nameof(RequestorUserId))]
    //public User User { get; set; }
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Message { get; set; }

    public bool DoNotShowRequest { get; set; } = false;

    public bool IsProcessed { get; set; }
    }