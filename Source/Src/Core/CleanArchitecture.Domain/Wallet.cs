namespace CleanArchitecture.Domain
    {
    //later not yet included in db schema
    public class Wallet : AuditableBaseEntity
        {
        [Key]
        public new Guid Id { get; set; } // Foreign key to ApplicationUser

        //[ForeignKey(nameof(UserId))]//its on differnt table so no link here
        //public ApplicationUser User { get; set; }

        public float Balance { get; set; }//decimal is constly

        // Other properties as needed
        }
    }