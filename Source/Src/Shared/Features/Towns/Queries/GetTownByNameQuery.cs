using FluentValidation;
using Shared.DTOs;

namespace Shared.Features.Towns.Queries;

//GetTownCardsForTodayQuery
public class GetTownsByNameQuery : IRequest<BaseResult<List<TownDto>>>
    {
    public string TownName { get; set; }

    // public int? IdCard { get; set; }//optional
    public DateTime? LastCacheTime { get; set; }
    }

public class GetTownByIdNameQueryValidator : AbstractValidator<GetTownsByNameQuery>
    {
    public GetTownByIdNameQueryValidator()//(ITranslator translator)
        {
        RuleFor(p => !string.IsNullOrEmpty(p.TownName));
        //.WithName(p => translator[nameof(p.Name)]);
        //RuleFor(p => p.ShortName)
        //    .NotNull();
        //.WithName(p => translator[nameof(p.ShortName)]);
        }
    }
