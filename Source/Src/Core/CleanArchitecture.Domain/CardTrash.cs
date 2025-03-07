namespace CleanArchitecture.Domain;

public class CardTrash : AuditableBaseEntity
{
    //non auto generated key

    [Key]
    public override int Id { get; set; }//same as idCard,not auto generated

    public string CardDataAsJsonString { get; set; } = default!;
}