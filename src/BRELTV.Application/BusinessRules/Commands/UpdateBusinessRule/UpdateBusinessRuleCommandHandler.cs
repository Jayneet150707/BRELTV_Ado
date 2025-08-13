using BRELTV.Application.Common.Exceptions;
using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.BusinessRules.Commands.UpdateBusinessRule
{
    public class UpdateBusinessRuleCommandHandler : IRequestHandler<UpdateBusinessRuleCommand>
    {
        private readonly IBusinessRuleRepository _repository;

        public UpdateBusinessRuleCommandHandler(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateBusinessRuleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetRuleByIdAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(BusinessRule), request.Id);
            }

            // Update the entity
            entity.CustomerProfileId = request.CustomerProfileId;
            entity.NoIncomeProofLTV = request.NoIncomeProofLTV;
            entity.MaxLTVWithProof = request.MaxLTVWithProof;
            entity.FIRequirement = request.FIRequirement;
            entity.MinIncomeProofAmount = request.MinIncomeProofAmount;
            entity.MinFloatingMoneyPercentage = request.MinFloatingMoneyPercentage;
            entity.UpdatedBy = request.UpdatedBy;

            await _repository.UpdateRuleAsync(entity);

            // Create an approval request for this updated rule
            var ruleApproval = new RuleApproval
            {
                BusinessRuleId = entity.Id,
                RequestedBy = request.UpdatedBy,
                ApprovalStatus = "Pending"
            };

            await _repository.CreateRuleApprovalRequestAsync(ruleApproval);
        }
    }
}

