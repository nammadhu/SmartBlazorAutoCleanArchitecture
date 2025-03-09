using FluentValidation;

namespace Shared.Features.Cards.Commands;

public class CU_CardDataCommand : CardData, IRequest<BaseResult<CardData>>, IEquatable<CU_CardDataCommand>
    {
    public Guid Operator { get; set; }

    public bool Equals(CU_CardDataCommand? other)//compares including id
        {//usage bool isEqual1 = person1.EqualsKeyInfo(person2);
        if (other == null) return false; // Not the same type
        return base.Equals(other);
        }

    public void ClientToServerDataExclusion()
        {
        }
    }

public class CreateUpdateCardDataCommandValidator : AbstractValidator<CU_CardDataCommand>
    {
    public CreateUpdateCardDataCommandValidator()//(ITranslator translator)
        {
        RuleFor(p => p.IdTown).NotNull().NotEqual(-1)
        .NotEmpty().GreaterThan(0).WithMessage("Town Id is Must");
        RuleFor(p => p.Operator).NotNull().NotEqual(Guid.Empty)
            .WithMessage("Operator Id Missing");
        }
    }
