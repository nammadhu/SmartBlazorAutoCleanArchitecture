namespace CleanArchitecture.Domain;

public class UserTrash : AuditableBaseEntity
{
    //non auto generated key
    public Guid UserId { get; set; }

    public string UserDataAsJsonString { get; set; } = default!;
}