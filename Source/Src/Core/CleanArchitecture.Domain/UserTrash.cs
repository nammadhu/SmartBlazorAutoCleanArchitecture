namespace CleanArchitecture.Domain;

public class UserTrash : AuditableBaseEntity
{
    //non auto generated key
    [Key]
    public new Guid Id { get; set; }

    public string UserDataAsJsonString { get; set; } = default!;
}