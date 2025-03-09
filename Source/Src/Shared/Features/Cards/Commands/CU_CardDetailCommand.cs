using FluentValidation;
using Shared.DTOs;

namespace Shared.Features.Cards.Commands;

public class CU_CardDetailCommand : CardDetailDto, IRequest<BaseResult<CardDetailDto>>, IEquatable<CU_CardDetailCommand>
    {
    public Guid Operator { get; set; }

    public bool Equals(CU_CardDetailCommand? other)//compares including id
        {//usage bool isEqual1 = person1.EqualsKeyInfo(person2);
        if (other == null) return false; // Not the same type
        return base.Equals(other);
        }

    public void ClientToServerDataExclusion()
        {
        if (Image1 != null)
            Image1 = Image1.BeforePostingCorrection();
        if (Image2 != null)
            Image2 = Image2.BeforePostingCorrection();
        if (Image3 != null)
            Image3 = Image3.BeforePostingCorrection();
        if (Image4 != null)
            Image4 = Image4.BeforePostingCorrection();
        if (Image5 != null)
            Image5 = Image5.BeforePostingCorrection();
        if (Image6 != null)
            Image6 = Image6.BeforePostingCorrection();

        iCard = null;
        //this.Created = null;
        LastModifiedBy = null;
        }
    }

public class CreateUpdateCardDetailCommandValidator : AbstractValidator<CU_CardDetailCommand>
    {
    public CreateUpdateCardDetailCommandValidator()//(ITranslator translator)
        {
        RuleFor(p => p.IdTown).NotNull().NotEqual(-1)
         .NotEmpty().GreaterThan(0).WithMessage("Town Id is Must");
        RuleFor(p => p.Operator).NotNull().NotEqual(Guid.Empty)
           .WithMessage("Operator Id Missing");
        }
    }
