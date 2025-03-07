namespace CleanArchitecture.Domain.Common
{
    public abstract class BaseEntity<TKey>
    {
        public virtual TKey Id { get; set; } = default!;
    }
    public abstract class BaseEntity : BaseEntity<long>
    {
    }
}
