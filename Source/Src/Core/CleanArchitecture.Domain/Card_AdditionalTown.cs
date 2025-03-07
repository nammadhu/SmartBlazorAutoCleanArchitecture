
namespace CleanArchitecture.Domain
{
    public class Card_AdditionalTown : AuditableBaseEntity
    {
        [NotMapped]
        private new int Id { get; set; }

        public Card_AdditionalTown()
        {
        }

        public Card_AdditionalTown(int idTown, int idCard)
        {
            IdTown = idTown;
            IdCARD = idCard;
        }

        public int IdTown { get; set; }
        public int IdCARD { get; set; }

        //[ForeignKey(nameof(IdCard))]
        public virtual Card? iCard { get; set; }

        //[ForeignKey(nameof(IdTown))]
        public virtual Town? Town { get; set; }
    }
}
