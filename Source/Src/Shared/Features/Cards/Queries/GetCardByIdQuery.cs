using SHARED.DTOs;

namespace SHARED.Features.Cards.Queries
    {
    public class GetCardByIdQuery : IRequest<BaseResult<CardDto>>
        {
        public bool IsDraft { get; set; } = false;
        public int IdCard { get; set; }
        //public Guid? UserId { get; set; }
        //dont pass userid over request,instead use authorizedclient & service api side use IAuthenticatedUser.Userid  or IsAuthenticated()
        }
    }
