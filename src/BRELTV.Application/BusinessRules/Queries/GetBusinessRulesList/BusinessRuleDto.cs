using System;

namespace BRELTV.Application.BusinessRules.Queries.GetBusinessRulesList
{
    public class BusinessRuleDto
    {
        public int Id { get; set; }
        public string CustomerProfileBand { get; set; }
        public decimal NoIncomeProofLTV { get; set; }
        public decimal MaxLTVWithProof { get; set; }
        public string FIRequirement { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

