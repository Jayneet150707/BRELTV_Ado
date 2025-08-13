using BRELTV.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.BusinessRules.Queries.GetBusinessRulesList
{
    public class GetBusinessRulesListQueryHandler : IRequestHandler<GetBusinessRulesListQuery, List<BusinessRuleDto>>
    {
        private readonly IBusinessRuleRepository _repository;

        public GetBusinessRulesListQueryHandler(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BusinessRuleDto>> Handle(GetBusinessRulesListQuery request, CancellationToken cancellationToken)
        {
            var rules = await _repository.GetAllRulesAsync(request.IsActive, request.IsApproved);
            var profiles = await _repository.GetAllCustomerProfilesAsync();

            return rules.Select(rule => new BusinessRuleDto
            {
                Id = rule.Id,
                CustomerProfileBand = profiles.FirstOrDefault(p => p.Id == rule.CustomerProfileId)?.ProfileBand,
                NoIncomeProofLTV = rule.NoIncomeProofLTV,
                MaxLTVWithProof = rule.MaxLTVWithProof,
                FIRequirement = rule.FIRequirement,
                IsActive = rule.IsActive,
                IsApproved = rule.IsApproved,
                CreatedAt = rule.CreatedAt
            }).ToList();
        }
    }
}

