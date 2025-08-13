using MediatR;

namespace BRELTV.Application.BusinessRules.Commands.CreateBusinessRule
{
    public class CreateBusinessRuleCommand : IRequest<int>
    {
        public string CustomerProfileBand { get; set; }
        public decimal NoIncomeProofLTV { get; set; }
        public decimal MaxLTVWithProof { get; set; }
        public string FIRequirement { get; set; }
        public decimal MinIncomeProofAmount { get; set; }
        public decimal MinFloatingMoneyPercentage { get; set; }
        public string RequestedBy { get; set; }
    }
}

