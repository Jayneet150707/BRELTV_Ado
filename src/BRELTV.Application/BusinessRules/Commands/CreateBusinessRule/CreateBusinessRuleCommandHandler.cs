using BRELTV.Application.Common.Exceptions;
using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.BusinessRules.Commands.CreateBusinessRule
{
    public class CreateBusinessRuleCommandHandler : IRequestHandler<CreateBusinessRuleCommand, int>
    {
        private readonly IBusinessRuleRepository _repository;

        public CreateBusinessRuleCommandHandler(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateBusinessRuleCommand request, CancellationToken cancellationToken)
        {
            // Get the customer profile
            var profile = await _repository.GetCustomerProfileByBandAsync(request.CustomerProfileBand);
            if (profile == null)
            {
                throw new NotFoundException($"Customer profile with band '{request.CustomerProfileBand}' not found.");
            }

            // Create the business rule
            var rule = new BusinessRule
            {
                CustomerProfileId = profile.Id,
                NoIncomeProofLTV = request.NoIncomeProofLTV,
                MaxLTVWithProof = request.MaxLTVWithProof,
                FIRequirement = request.FIRequirement,
                MinIncomeProofAmount = request.MinIncomeProofAmount,
                MinFloatingMoneyPercentage = request.MinFloatingMoneyPercentage,
                CreatedBy = request.RequestedBy
            };

            // Save the rule
            var ruleId = await _repository.CreateRuleAsync(rule);

            // Create an approval request
            var approval = new RuleApproval
            {
                BusinessRuleId = ruleId,
                RequestedBy = request.RequestedBy
            };

            await _repository.CreateRuleApprovalRequestAsync(approval);

            return ruleId;
        }
    }
}

