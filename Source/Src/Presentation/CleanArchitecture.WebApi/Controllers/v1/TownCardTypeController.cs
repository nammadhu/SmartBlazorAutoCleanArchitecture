using BASE;
using SHARED.Features.CardTypes.Commands;
using SHARED.Features.CardTypes.Queries;

namespace CleanArchitecture.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class TownCardTypeController(IMediator mediator) : BaseApiController(mediator), ITownCardTypeController
    {//ILogger<TownCardTypeController> logger,
        //to avoid second operation started error fr firsttime adding this flag.No other reason.
        private static bool firstTime = true;

        [HttpGet]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<BaseResult<List<CardTypeDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                if (firstTime) { firstTime = false; return null; }
                else
                    return await Mediator.Send(new GetCardTypesAllQuery(), cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //throw;
                return new BaseResult<List<CardTypeDto>>() { Success = false };
            }
        }

        [HttpGet]
        public async Task<PagedResponse<CardTypeDto>> GetPagedList([FromQuery] GetCardTypesPagedListQuery model, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await Mediator.Send(model, cancellationToken);
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        [HttpGet]
        public async Task<BaseResult<CardTypeDto>> GetById([FromQuery] int id, CancellationToken cancellationToken = default)
            => await Mediator.Send(new GetCardTypeByIdQuery() { IdCardType = id }, cancellationToken);

        [HttpPost, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
        public async Task<BaseResult<CardTypeDto>> Create(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
            //model.CreatedBy = UserIdExtract();
            //above is separately not required bcz ApplicationDBcontext amking default changes onCreate Authuser id & onUpdate LastModifedBy userid
            return await Mediator.Send(model, cancellationToken);
        }

        [HttpPost, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
        public async Task<BaseResult<CardTypeDto>> CreateUpdate(CU_CardTypeCommand model, CancellationToken cancellationToken)
        {
            if (model.Id == 0)
                return await Create(model, cancellationToken);
            else
                return await Update(model, cancellationToken);
        }

        [HttpPut, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
        public async Task<BaseResult<CardTypeDto>> Update(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
            //model.LastModifiedBy = UserIdExtract();
            return await Mediator.Send(model, cancellationToken);
        }

        [HttpDelete, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
        public async Task<BaseResult> Delete([FromQuery] int id, CancellationToken cancellationToken = default)
            => await Mediator.Send(new DeleteCardTypeCommand() { IdCardType = id }, cancellationToken);

        //private Guid UserIdExtract()
        //    {
        //    //this is separately not required bcz ApplicationDBcontext amking default changes onCreate Authuser id & onUpdate LastModifedBy userid
        //    var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (Guid.TryParse(id, out Guid guid))
        //        {
        //        return guid;
        //        }
        //    throw new Exception("UserId Not Found");
        //    }
    }
}
