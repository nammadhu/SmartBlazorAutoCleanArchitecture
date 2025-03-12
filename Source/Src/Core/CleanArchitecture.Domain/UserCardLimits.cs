using BASE.Common;

namespace CleanArchitecture.Domain
    {
    public class UserCardLimits : AuditableBaseEntity
        {
        public const int MaxAllowedCardsNormalUser = 3;

        //this is the key but not auto generated,instead id of ApplicationUser table. even on that deletion ,this will be maintained forever.
        [Key]
        public new Guid Id { get; set; }

        public int TotalCardCount { get; set; } = 1;
        public int TotalCreatedCardCount { get; set; } = 1;//includes deleted
        public int TotalVerifiedCardCount { get; set; } = 1;
        public int TotalDraftCardCount { get; set; } = 1;
        public int TotalDeletedCardCount { get; set; } = 0;
        public int AllowedCardLimits { get; set; } = MaxAllowedCardsNormalUser;
        }
    }