using System.ComponentModel.DataAnnotations;

namespace BASE.Common
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
        protected AuditableBaseEntity(string name)
            {
            Name = name;
            }

        protected AuditableBaseEntity()
            {
            }
        //[Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "{0} Must be {2} & {1} characters")]
        public virtual string Name { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        }

    public abstract class AuditableBaseEntity : AuditableBaseEntity<int>, IAuditableBaseEntity
        {
        }
    }
