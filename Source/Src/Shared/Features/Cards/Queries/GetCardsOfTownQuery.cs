using SHARED.DTOs;

namespace SHARED.Features.Cards.Queries;

public class GetCardsOfTownQuery : IRequest<BaseResult<TownCardsDto>>
    {
    public int IdTown { get; set; }

    public int? IdCard { get; set; }//need to make as when some direct reader but for next steps //laterTodo

    //public bool All { get; set; }
    public DateTime? LastServerFetchTime { get; set; }

    public bool DeltaChangesOnly { get; set; } = false;

    public bool RefreshByAdmin { get; set; }
    }

/*
public class GetCardsOfTownQueryValidator : AbstractValidator<GetCardsOfTownQuery>
{
    public GetCardsOfTownQueryValidator()//(ITranslator translator)
    {
        RuleFor(p => p.IdTown).NotNull().NotEqual(-1)
        .NotEmpty().GreaterThan(0).WithMessage("Town is Must");
    }
}*/
