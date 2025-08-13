using FluentValidation;

namespace BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication
{
    public class EvaluateLoanApplicationCommandValidator : AbstractValidator<EvaluateLoanApplicationCommand>
    {
        public EvaluateLoanApplicationCommandValidator()
        {
            RuleFor(v => v.CustomerProfile)
                .NotEmpty().WithMessage("Customer profile is required.");

            RuleFor(v => v.IncomeProofAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Income proof amount must be non-negative.");

            RuleFor(v => v.FloatingMoneyAfterExpensesAndEMI)
                .GreaterThanOrEqualTo(0).WithMessage("Floating money percentage must be non-negative.")
                .LessThanOrEqualTo(100).WithMessage("Floating money percentage cannot exceed 100%.");

            RuleFor(v => v.EvaluatedBy)
                .NotEmpty().WithMessage("Evaluated by is required.");
        }
    }
}

