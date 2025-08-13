using System;

namespace BRELTV.Models.Entities
{
    public class LoanEvaluation
    {
        public int Id { get; set; }
        public string CustomerProfileBand { get; set; } = string.Empty;
        public bool IncomeProofAvailable { get; set; }
        public decimal IncomeProofAmount { get; set; }
        public decimal FloatingMoneyAfterExpensesAndEMI { get; set; }
        public decimal AssignedLTV { get; set; }
        public string FIRequirement { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime EvaluatedAt { get; set; }
        public string EvaluatedBy { get; set; } = string.Empty;
    }
}

