using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain;

public class CardTag : AuditableBaseEntity
    {
    //here its actually one tag for one town,so had to be unique with townid
    //later we can plan for multiple taf for single town or card
    //[Required]

    [Key]
    public override int Id { get; set; } //bhadravathi,kadur,bidar

    [ForeignKey(nameof(Id))]
    public virtual Card? iCard { get; set; }

    //here its actually one tag for one town,so had to be unique with townid
    public string Tag { get; set; } = default!; //lets make these as csv or multiple entry then had to make combined key
    }