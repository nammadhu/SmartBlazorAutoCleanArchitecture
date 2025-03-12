
using BASE;
using SHARED.Features.Towns.Commands;
using SHARED.Features.Towns.Queries;

namespace CleanArchitecture.WebApi.Controllers.v1;

[ApiVersion("1")]
public class TownController(ILogger<TownController> logger, IMediator mediator)
    : BaseApiController(mediator), ITownController
//ServerCachingService serverCachingService
{
    [HttpGet]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<BaseResult<List<TownDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Mediator.Send(new GetTownsAllQuery(), cancellationToken);
            if (result?.Data?.Count > 0)
                result.Data.ForEach(town => town.NullifyPrivateData());
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            //throw;
            return new BaseResult<List<TownDto>>() { Success = false };
        }
    }

    [HttpGet]
    public async Task<BaseResult<List<TownDto>>> DeltaLoadOfTownsChanges(CancellationToken cancellationToken)
    {//this is only for clientSide,not for server
        return await GetAll(cancellationToken);
    }

    [HttpGet]
    public async Task<BaseResult<List<TownDto>>> GetDeltaOfTownsList(DateTime lastServerFetchTime, CancellationToken cancellationToken = default)
    {
        //with this result,client should update latest fetched time also on clientSide ,so further next round will be reduced
        try
        {
            var result = await Mediator.Send(new GetTownsAllQuery(), cancellationToken);
            if (result?.Data?.Count > 0)
            {
                var filteredResult = result.Data.Where(x => x.LastCardUpdateTime > lastServerFetchTime);
                filteredResult.ToList().ForEach(town => town.NullifyPrivateData());
                return filteredResult.ToList();
            }
            return new BaseResult<List<TownDto>>() { Success = true };
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            //throw;
            return new BaseResult<List<TownDto>>() { Success = false };
        }
    }

    [HttpGet]
    public async Task<BaseResult<TownDto>> GetTownHeaderById(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = (await GetAll(cancellationToken))?.Data.FirstOrDefault(x => x.Id == id);
            if (result != null)
                result.NullifyPrivateData();
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            //throw;
            return new BaseResult<TownDto>() { Success = false };
        }
    }

    [HttpGet]
    public async Task<BaseResult<List<TownDto>>> Search(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Mediator.Send(new GetTownsByNameQuery() { TownName = name }, cancellationToken);
            if (result?.Data?.Count > 0)
                result.Data.ForEach(town => town.NullifyPrivateData());
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            //throw;
            return new BaseResult<List<TownDto>>() { Success = false };
        }
    }

    [HttpGet]
    public async Task<PagedResponse<TownDto>> GetPagedList([FromQuery] GetTownsPagedListQuery model, CancellationToken cancellationToken)
    {//todo had to move
        try
        {
            var res = await Mediator.Send(model, cancellationToken);
            return res;
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            throw;
        }
    }

    [HttpPost, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
    public async Task<BaseResult<TownDto>> Create(CU_TownCommand model, CancellationToken cancellationToken)
    {
        //model.CreatedBy = UserIdExtract();
        //above is separately not required bcz ApplicationDbContext making default changes onCreate Authuser id & onUpdate LastModifiedBy userid
        return await Mediator.Send(model, cancellationToken);
    }

    [HttpPost, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
    public async Task<BaseResult<TownDto>> CreateUpdate(CU_TownCommand model, CancellationToken cancellationToken)
    {
        if (model.Id == 0)
            return await Create(model, cancellationToken);
        else
            return await Update(model, cancellationToken);
    }

    [HttpPut, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
    public async Task<BaseResult<TownDto>> Update(CU_TownCommand model, CancellationToken cancellationToken)
    {
        //model.LastModifiedBy = UserIdExtract();
        return await Mediator.Send(model, cancellationToken);
    }

    [HttpDelete, Authorize(Roles = CONSTANTS.ROLES.Role_Admin)]
    public async Task<BaseResult> Delete(int id, CancellationToken cancellationToken)
        => await Mediator.Send(new DeleteTownCommand() { IdTown = id }, cancellationToken);
}
