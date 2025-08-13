using System;

namespace BRELTV.Application.BusinessRules.Queries.GetPendingApprovals
{
    public class RuleApprovalDto
    {
        public int Id { get; set; }
        public int BusinessRuleId { get; set; }
        public string CustomerProfileBand { get; set; }
        public decimal NoIncomeProofLTV { get; set; }
        public decimal MaxLTVWithProof { get; set; }
        public string FIRequirement { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}

