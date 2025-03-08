using PublicCommon;
using PublicCommon.Common;

namespace CleanArchitecture.Domain;

public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>
{
    public Guid CreatedBy { get; set; }
    public DateTime Created { get; set; } = DateTimeExtension.CurrentTime;
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }
}

public abstract class AuditableBaseEntity : AuditableBaseEntity<int>
{
}