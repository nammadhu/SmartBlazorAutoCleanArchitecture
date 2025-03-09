using CleanArchitecture.Application;

namespace MyTown.Application.Features.Cards.Commands;

public partial class CU_CardCommandHandler(ICardRepository cardRepository,
    ICard_DraftChangesRepository draftRepository,
    //IAdditionalTownsOfVerifiedCardRepository additionalTownsOfVerifiedCardRepository,
    //ICardDataRepository cardDataRepository, ICardDetailRepository cardDetailsRepository,
    IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper,
    IAuthenticatedUserService authenticatedUser, IAzImageStorage azImageStorage,
    ICardApprovalRepository townCardApprovalRepository, ServerCachingTownCards cachingServiceTown
    , ILogger<CU_CardCommandHandler> logger,
    IUserDetailRepository userDetailRepository, IIdentityRepository identityRepository)
    : IRequestHandler<CU_CardCommand, BaseResult<iCardDto>>
    {
    public async Task<BaseResult<iCardDto>> Handle(CU_CardCommand requestCommand, CancellationToken cancellationToken)
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

            UserDetailBase userOnGraphDb = await identityRepository.GetUserAsync(authenticatedUser.UserId, cancellationToken);
            if (userOnGraphDb == null || userOnGraphDb == default)
                return new BaseResult<iCardDto>() { Success = false, Errors = [new Error(ErrorCode.TamperedData, description: "User Not Found on Authentication System", "User Full")] };

            if (userOnGraphDb.Email != authenticatedUser.Email)
                return new BaseResult<iCardDto>() { Success = false, Errors = [new Error(ErrorCode.TamperedData, description: "Email id doesnt match", "Email")] };

            BaseResult<iCardDto> result;
            if (requestCommand.Id == 0)//for create
                result = await CreateCardWithData(requestCommand, userOnGraphDb, cancellationToken);
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
