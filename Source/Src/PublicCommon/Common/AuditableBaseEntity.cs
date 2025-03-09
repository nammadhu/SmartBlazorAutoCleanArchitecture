namespace PublicCommon.Common
    {
    public interface IAuditableBaseEntity
        {
        public string Name { get; set; }
        DateTime Created { get; set; }
        Guid CreatedBy { get; set; }
        DateTime? LastModified { get; set; }
        Guid? LastModifiedBy { get; set; }
        }

    public interface IAuditableBaseEntity<TKey> : IAuditableBaseEntity
        { //still not used anywhere
        public TKey Id { get; set; }
        }

    public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>, IAuditableBaseEntity
        {
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        }

    public abstract class AuditableBaseEntity : AuditableBaseEntity<int>, IAuditableBaseEntity
        {
        }
    }
