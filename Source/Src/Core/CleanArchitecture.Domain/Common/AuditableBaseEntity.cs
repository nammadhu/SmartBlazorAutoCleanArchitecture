using System;

namespace CleanArchitecture.Domain.Common
{
    public interface IAuditableBaseEntity
    {
        DateTime Created { get; set; }
        Guid CreatedBy { get; set; }
        DateTime? LastModified { get; set; }
        Guid? LastModifiedBy { get; set; }
    }

    public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>, IAuditableBaseEntity
    {
        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }

    public abstract class AuditableBaseEntity : AuditableBaseEntity<int>, IAuditableBaseEntity
    {
    }
}
