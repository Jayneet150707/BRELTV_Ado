using BRELTV.Domain.ValueObjects;

namespace BRELTV.Domain.Interfaces
{
    public interface IBusinessRuleEngine
    {
        LoanEvaluationResult EvaluateLoanApplication(
            string customerProfile, 
            bool incomeProofAvailable, 
            decimal incomeProofAmount, 
            decimal floatingMoneyAfterExpensesAndEMI);
    }
}

