namespace PublicCommon.Common;

public abstract class BaseEntity<TKey>
{
    public virtual TKey Id { get; set; } = default!;
}

public abstract class BaseEntity : BaseEntity<int>
{
}

//[System.ComponentModel.DataAnnotations.Key]
//public virtual int Id { get; set; }