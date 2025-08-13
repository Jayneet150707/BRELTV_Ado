using BRELTV.Models.DTOs;
using FluentValidation;

namespace BRELTV.API.Validators
{
    public class BusinessRuleDtoValidator : AbstractValidator<BusinessRuleDto>
    {
        public BusinessRuleDtoValidator()
        {
            RuleFor(x => x.CustomerProfileBand)
                .NotEmpty().WithMessage("Customer profile band is required")
                .MaximumLength(50).WithMessage("Customer profile band cannot exceed 50 characters");

            RuleFor(x => x.NoIncomeProofLTV)
                .GreaterThan(0).WithMessage("No income proof LTV must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("No income proof LTV cannot exceed 100");

            RuleFor(x => x.MaxLTVWithProof)
                .GreaterThan(0).WithMessage("Max LTV with proof must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Max LTV with proof cannot exceed 100");

            RuleFor(x => x.FIRequirement)
                .NotEmpty().WithMessage("FI requirement is required")
                .Must(x => x == "FI Mandatory" || x == "FI Waiver")
                .WithMessage("FI requirement must be either 'FI Mandatory' or 'FI Waiver'");

            RuleFor(x => x.MinIncomeProofAmount)
                .GreaterThan(0).WithMessage("Minimum income proof amount must be greater than 0");

            RuleFor(x => x.MinFloatingMoneyPercentage)
                .GreaterThan(0).WithMessage("Minimum floating money percentage must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Minimum floating money percentage cannot exceed 100");
        }
    }
}

