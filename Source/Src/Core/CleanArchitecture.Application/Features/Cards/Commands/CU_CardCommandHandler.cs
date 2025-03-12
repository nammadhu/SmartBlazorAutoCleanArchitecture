using CleanArchitecture.Application;
using CleanArchitecture.Application.Interfaces.UserInterfaces;

namespace MyTown.Application.Features.Cards.Commands;

public partial class CU_CardCommandHandler(ICardRepository cardRepository,
    ICard_DraftChangesRepository draftRepository,
    //IAdditionalTownsOfVerifiedCardRepository additionalTownsOfVerifiedCardRepository,
    //ICardDataRepository cardDataRepository, ICardDetailRepository cardDetailsRepository,
    IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper,
    IAuthenticatedUserService authenticatedUserService, IAccountServices accountServices,
    IAzImageStorage azImageStorage,
    ICardApprovalRepository townCardApprovalRepository, ServerCachingTownCards cachingServiceTown
    , ILogger<CU_CardCommandHandler> logger)
    //IUserDetailRepository userDetailRepository,    IIdentityRepository identityRepository)
    : IRequestHandler<CU_CardCommand, BaseResult<CardDto>>
    {
    public async Task<BaseResult<CardDto>> Handle(CU_CardCommand requestCommand, CancellationToken cancellationToken)
        {
        try
            {
            //s1.fetch from graph,as valid or not ,also make sure is same email for phishing
            //s2.pass the same to Create/Update layer
            //s3.UserDetailRepo, check if exists,
            //s3.1yes & proper(as limit of 1 card for normal user) then give as true ,else throw ex as "Not Allowed"
            //s3.2 no, then return no. if
            //s4. on card insert table,if prevResult is no then insert now try coupled way otherwise just Creator role
            //s5.on update no changes
            //s6.on MoveToVerified step, Update or add also ok


            //todo adding user is an optional process
            //below is in case of cosmos db ensuring

            //todo add cancreate card utilization

            /* for ad b2c
            UserDetailBase userOnGraphDb = await identityRepository.GetUserAsync(authenticatedUserService.UserId, cancellationToken);
            if (userOnGraphDb == null || userOnGraphDb == default)
                return new BaseResult<iCardDto>() { Success = false, Errors = [new Error(ErrorCode.TamperedData, description: "User Not Found on Authentication System", "User Full")] };

            if (userOnGraphDb.Email != authenticatedUserService.Email)
                return new BaseResult<iCardDto>() { Success = false, Errors = [new Error(ErrorCode.TamperedData, description: "Email id doesnt match", "Email")] };
            */
            BaseResult<CardDto> result;
            if (requestCommand.Id == 0)//for create
                result = await CreateCardWithData(requestCommand, cancellationToken);
            else // UpdateCard not sure of checking userOnGraphDb
                result = await UpdateCardOnly(requestCommand, cancellationToken);//no data update here

            return result;
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCard Creation/UpdateCard"));
            }
        }
    }
