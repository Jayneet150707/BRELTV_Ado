using BRELTV.Application.Common.Exceptions;
using BRELTV.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.BusinessRules.Queries.GetBusinessRuleDetail
{
    public class GetBusinessRuleDetailQueryHandler : IRequestHandler<GetBusinessRuleDetailQuery, BusinessRuleDetailDto>
    {
        private readonly IBusinessRuleRepository _repository;

        public GetBusinessRuleDetailQueryHandler(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<BusinessRuleDetailDto> Handle(GetBusinessRuleDetailQuery request, CancellationToken cancellationToken)
        {
            var rule = await _repository.GetRuleByIdAsync(request.Id);
            
            if (rule == null)
            {
                throw new NotFoundException($"Business rule with ID {request.Id} not found.");
            }

            var profile = await _repository.GetCustomerProfileByIdAsync(rule.CustomerProfileId);
            
            if (profile == null)
            {
                throw new NotFoundException($"Customer profile with ID {rule.CustomerProfileId} not found.");
            }

            return new BusinessRuleDetailDto
            {
                Id = rule.Id,
                CustomerProfileBand = profile.ProfileBand,
                NoIncomeProofLTV = rule.NoIncomeProofLTV,
                MaxLTVWithProof = rule.MaxLTVWithProof,
                FIRequirement = rule.FIRequirement,
                MinIncomeProofAmount = rule.MinIncomeProofAmount,
                MinFloatingMoneyPercentage = rule.MinFloatingMoneyPercentage,
                IsActive = rule.IsActive,
                IsApproved = rule.IsApproved,
                CreatedAt = rule.CreatedAt,
                CreatedBy = rule.CreatedBy,
                UpdatedAt = rule.UpdatedAt,
                UpdatedBy = rule.UpdatedBy,
                ApprovedAt = rule.ApprovedAt,
                ApprovedBy = rule.ApprovedBy
            };
        }
    }
}

