namespace BRELTV.Models.DTOs
{
    public class LoanEvaluationRequest
    {
        /// <summary>
        /// Customer profile band (e.g., "0", "-1", "650-700", "700-750", "750+")
        /// </summary>
        public string CustomerProfile { get; set; } = string.Empty;

        /// <summary>
        /// Whether income proof is available
        /// </summary>
        public bool IncomeProofAvailable { get; set; }

        /// <summary>
        /// Income proof amount (monthly income)
        /// </summary>
        public decimal IncomeProofAmount { get; set; }

        /// <summary>
        /// Percentage of disposable money left after expenses and EMI
        /// </summary>
        public decimal FloatingMoneyAfterExpensesAndEMI { get; set; }

        /// <summary>
        /// User who initiated the evaluation
        /// </summary>
        public string? EvaluatedBy { get; set; }
    }
}

