using System;

namespace BRELTV.Models.Entities
{
    public class RuleApproval
    {
        public int Id { get; set; }
        public int BusinessRuleId { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty; // "Pending", "Approved", "Rejected"
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Comments { get; set; }
    }
}

