using MediatR;

namespace BRELTV.Application.BusinessRules.Commands.RejectBusinessRule
{
    public class RejectBusinessRuleCommand : IRequest
    {
        public int RuleApprovalId { get; set; }
        public string RejectedBy { get; set; }
        public string Comments { get; set; }
    }
}

