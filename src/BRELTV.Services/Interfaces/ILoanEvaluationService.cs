using BRELTV.Models.DTOs;
using BRELTV.Models.ValueObjects;
using System.Threading.Tasks;

namespace BRELTV.Services.Interfaces
{
    public interface ILoanEvaluationService
    {
        Task<LoanEvaluationResponse> EvaluateLoanAsync(LoanEvaluationRequest request);
        Task<LoanEvaluationResult> GetLtvAndFiAsync(
            string customerProfile, 
            bool incomeProofAvailable, 
            decimal incomeProofAmount, 
            decimal floatingMoneyAfterExpensesAndEmi);
    }
}

