using FluentValidation;

namespace BRELTV.Application.BusinessRules.Commands.CreateBusinessRule
{
    public class CreateBusinessRuleCommandValidator : AbstractValidator<CreateBusinessRuleCommand>
    {
        public CreateBusinessRuleCommandValidator()
        {
            RuleFor(v => v.CustomerProfileBand)
                .NotEmpty().WithMessage("Customer profile band is required.");

            RuleFor(v => v.NoIncomeProofLTV)
                .GreaterThanOrEqualTo(0).WithMessage("No income proof LTV must be non-negative.")
                .LessThanOrEqualTo(100).WithMessage("No income proof LTV cannot exceed 100%.");

            RuleFor(v => v.MaxLTVWithProof)
                .GreaterThanOrEqualTo(0).WithMessage("Max LTV with proof must be non-negative.")
                .LessThanOrEqualTo(100).WithMessage("Max LTV with proof cannot exceed 100%.");

            RuleFor(v => v.FIRequirement)
                .NotEmpty().WithMessage("FI requirement is required.")
                .Must(fi => fi == "FI Mandatory" || fi == "FI Waiver")
                .WithMessage("FI requirement must be either 'FI Mandatory' or 'FI Waiver'.");

            RuleFor(v => v.MinIncomeProofAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum income proof amount must be non-negative.");

            RuleFor(v => v.MinFloatingMoneyPercentage)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum floating money percentage must be non-negative.")
                .LessThanOrEqualTo(100).WithMessage("Minimum floating money percentage cannot exceed 100%.");

            RuleFor(v => v.CreatedBy)
                .NotEmpty().WithMessage("Created by is required.");
        }
    }
}

