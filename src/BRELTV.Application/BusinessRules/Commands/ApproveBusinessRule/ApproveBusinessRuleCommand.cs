using MediatR;

namespace BRELTV.Application.BusinessRules.Commands.ApproveBusinessRule
{
    public class ApproveBusinessRuleCommand : IRequest
    {
        public int RuleApprovalId { get; set; }
        public string ApprovedBy { get; set; }
    }
}

