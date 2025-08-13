namespace BRELTV.Models.DTOs
{
    public class LoanEvaluationResponse
    {
        /// <summary>
        /// Loan-to-Value percentage
        /// </summary>
        public decimal LTV { get; set; }

        /// <summary>
        /// Field Investigation requirement ("FI Mandatory" or "FI Waiver")
        /// </summary>
        public string FIRequirement { get; set; } = string.Empty;

        /// <summary>
        /// Reason for the decision
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Evaluation record ID
        /// </summary>
        public int EvaluationId { get; set; }
    }
}

