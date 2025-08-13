using MediatR;

namespace BRELTV.Application.BusinessRules.Commands.UpdateBusinessRule
{
    public class UpdateBusinessRuleCommand : IRequest
    {
        public int Id { get; set; }
        public int CustomerProfileId { get; set; }
        public decimal NoIncomeProofLTV { get; set; }
        public decimal MaxLTVWithProof { get; set; }
        public string FIRequirement { get; set; }
        public decimal MinIncomeProofAmount { get; set; }
        public decimal MinFloatingMoneyPercentage { get; set; }
        public string UpdatedBy { get; set; }
    }
}

