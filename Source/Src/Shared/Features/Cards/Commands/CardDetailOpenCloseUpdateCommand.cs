using FluentValidation;

namespace SHARED.Features.Cards.Commands;

public class CardDetailOpenCloseUpdateCommand : IRequest<BaseResult<bool?>>
    {
    public Guid Operator { get; set; }
    public int IdTown { get; set; }//primary town is must,useful for cache purpose
    public int Id { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOpenClose { get; set; }
    }

public class CardDetailOpenCloseUpdateCommandValidator : AbstractValidator<CardDetailOpenCloseUpdateCommand>
    {
    public CardDetailOpenCloseUpdateCommandValidator()//(ITranslator translator)
        {
        RuleFor(p => p.IsOpenClose)
            .NotNull().WithMessage("Is Open Or Close Must Be Passed");
        RuleFor(p => p.IsVerified)
            .NotNull().WithMessage("IsVerified Must Be Passed");
        RuleFor(p => p.Id).NotNull().NotEqual(-1)
            .NotEmpty().GreaterThan(0).WithMessage("Id Card is Must");
        RuleFor(p => p.IdTown).NotNull().NotEqual(-1)
            .NotEmpty().GreaterThan(0).WithMessage("Town is Must");
        RuleFor(p => p.Operator).NotNull().NotEqual(Guid.Empty)
                    .WithMessage("Operator Id Missing");
        }
    }
