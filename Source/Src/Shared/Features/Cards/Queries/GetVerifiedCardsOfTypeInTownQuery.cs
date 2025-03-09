using FluentValidation;
using Shared.DTOs;

namespace Shared.Features.Cards.Queries;

public class GetVerifiedCardsOfTypeInTownQuery : IRequest<BaseResult<List<iCardDto>>>
    {
    public int TownId { get; set; }
    public int TypeId { get; set; }
    }

public class GetVerifiedCardsOfTypeInTownQueryValidator : AbstractValidator<GetVerifiedCardsOfTypeInTownQuery>
    {
    public GetVerifiedCardsOfTypeInTownQueryValidator(ITranslator translator)
        {
        //RuleFor(p => p.IsCreator)
        //    .NotNull();
        }
    }
