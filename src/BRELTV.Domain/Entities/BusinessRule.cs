using System;

namespace BRELTV.Domain.Entities
{
    public class BusinessRule
    {
        public int Id { get; set; }
        public int CustomerProfileId { get; set; }
        public decimal NoIncomeProofLTV { get; set; }
        public decimal MaxLTVWithProof { get; set; }
        public string FIRequirement { get; set; }
        public decimal MinIncomeProofAmount { get; set; }
        public decimal MinFloatingMoneyPercentage { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }

        // Navigation properties
        public CustomerProfile CustomerProfile { get; set; }
    }
}

