using System;

namespace BRELTV.Domain.Entities
{
    public class LoanEvaluation
    {
        public int Id { get; set; }
        public string CustomerProfileBand { get; set; }
        public bool IncomeProofAvailable { get; set; }
        public decimal IncomeProofAmount { get; set; }
        public decimal FloatingMoneyAfterExpensesAndEMI { get; set; }
        public decimal AssignedLTV { get; set; }
        public string FIRequirement { get; set; }
        public string Reason { get; set; }
        public DateTime EvaluatedAt { get; set; }
        public string EvaluatedBy { get; set; }
    }
}

