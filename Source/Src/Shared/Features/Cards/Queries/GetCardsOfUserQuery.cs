using FluentValidation;
using Shared.DTOs;

namespace Shared.Features.Cards.Queries;

public class GetCardsOfUserQuery : IRequest<BaseResult<UserDetailDto>>
    {
    public Guid UserId { get; set; }
    //Fetch from IAuthenticationService for logged in user & assign in controller levels. if null then donot call itself

    //public bool IsCardCreator { get; set; }
    //public bool IsCardOwner { get; set; }
    //public bool IsCardVerifiedOwner { get; set; }

    //public bool IsCardVerifiedReviewer { get; set; }//mostly not required

    // Guid userId, bool isCreator, bool isOwner, bool isVerifiedCardOwner, bool isVerifiedReviewer, int townId = 0
    }

public class GetUserCardMoreDetailsValidator : AbstractValidator<GetCardsOfUserQuery>
    {
    public GetUserCardMoreDetailsValidator(ITranslator translator)
        {
        //RuleFor(p => p.IsCreator)
        //    .NotNull();
        }
    }
