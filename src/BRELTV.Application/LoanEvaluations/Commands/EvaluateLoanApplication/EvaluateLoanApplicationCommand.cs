using MediatR;

namespace BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication
{
    public class EvaluateLoanApplicationCommand : IRequest<LoanEvaluationResultDto>
    {
        public string CustomerProfile { get; set; }
        public bool IncomeProofAvailable { get; set; }
        public decimal IncomeProofAmount { get; set; }
        public decimal FloatingMoneyAfterExpensesAndEMI { get; set; }
        public string EvaluatedBy { get; set; }
    }
}

