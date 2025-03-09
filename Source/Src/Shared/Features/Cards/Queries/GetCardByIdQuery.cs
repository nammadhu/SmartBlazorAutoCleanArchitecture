using Shared.DTOs;

namespace Shared.Features.Cards.Queries
    {
    public class GetCardByIdQuery : IRequest<BaseResult<iCardDto>>
        {
        public bool IsDraft { get; set; } = false;
        public int IdCard { get; set; }
        //public Guid? UserId { get; set; }
        //dont pass userid over request,instead use authorizedclient & service api side use IAuthenticatedUser.Userid  or IsAuthenticated()
        }
    }
