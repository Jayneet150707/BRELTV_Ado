using FluentValidation;

namespace BRELTV.Application.BusinessRules.Commands.UpdateBusinessRule
{
    public class UpdateBusinessRuleCommandValidator : AbstractValidator<UpdateBusinessRuleCommand>
    {
        public UpdateBusinessRuleCommandValidator()
        {
            RuleFor(v => v.Id)
                .GreaterThan(0).WithMessage("Business rule ID must be greater than 0.");

            RuleFor(v => v.CustomerProfileId)
                .GreaterThan(0).WithMessage("Customer profile ID must be greater than 0.");

            RuleFor(v => v.NoIncomeProofLTV)
                .InclusiveBetween(0, 100).WithMessage("No income proof LTV must be between 0 and 100.");

            RuleFor(v => v.MaxLTVWithProof)
                .InclusiveBetween(0, 100).WithMessage("Max LTV with proof must be between 0 and 100.");

            RuleFor(v => v.FIRequirement)
                .NotEmpty().WithMessage("FI requirement is required.")
                .Must(fi => fi == "FI Mandatory" || fi == "FI Waiver")
                .WithMessage("FI requirement must be either 'FI Mandatory' or 'FI Waiver'.");

            RuleFor(v => v.MinIncomeProofAmount)
                .GreaterThan(0).WithMessage("Minimum income proof amount must be greater than 0.");

            RuleFor(v => v.MinFloatingMoneyPercentage)
                .InclusiveBetween(0, 100).WithMessage("Minimum floating money percentage must be between 0 and 100.");

            RuleFor(v => v.UpdatedBy)
                .NotEmpty().WithMessage("Updated by is required.");
        }
    }
}

