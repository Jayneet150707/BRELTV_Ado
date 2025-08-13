using BRELTV.Models.DTOs;
using FluentValidation;

namespace BRELTV.API.Validators
{
    public class LoanEvaluationRequestValidator : AbstractValidator<LoanEvaluationRequest>
    {
        public LoanEvaluationRequestValidator()
        {
            RuleFor(x => x.CustomerProfile)
                .NotEmpty().WithMessage("Customer profile is required")
                .MaximumLength(50).WithMessage("Customer profile cannot exceed 50 characters");

            RuleFor(x => x.IncomeProofAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Income proof amount must be greater than or equal to 0")
                .When(x => x.IncomeProofAvailable);

            RuleFor(x => x.FloatingMoneyAfterExpensesAndEMI)
                .GreaterThanOrEqualTo(0).WithMessage("Floating money percentage must be greater than or equal to 0")
                .LessThanOrEqualTo(100).WithMessage("Floating money percentage cannot exceed 100");
        }
    }
}

