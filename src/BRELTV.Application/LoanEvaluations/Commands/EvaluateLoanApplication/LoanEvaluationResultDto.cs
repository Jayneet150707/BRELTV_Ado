namespace BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication
{
    public class LoanEvaluationResultDto
    {
        public decimal LTV { get; set; }
        public string FIRequirement { get; set; }
        public string Reason { get; set; }
    }
}

