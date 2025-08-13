namespace BRELTV.Models.ValueObjects
{
    public class LoanEvaluationResult
    {
        public decimal LTV { get; }
        public string FIRequirement { get; }
        public string Reason { get; }

        public LoanEvaluationResult(decimal ltv, string fiRequirement, string reason)
        {
            LTV = ltv;
            FIRequirement = fiRequirement;
            Reason = reason;
        }
    }
}

