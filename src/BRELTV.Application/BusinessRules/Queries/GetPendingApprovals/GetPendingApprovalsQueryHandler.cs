using BRELTV.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.BusinessRules.Queries.GetPendingApprovals
{
    public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, List<RuleApprovalDto>>
    {
        private readonly IBusinessRuleRepository _repository;

        public GetPendingApprovalsQueryHandler(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RuleApprovalDto>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
        {
            var approvals = await _repository.GetPendingApprovalsAsync();
            var rules = await _repository.GetRulesByIdsAsync(approvals.Select(a => a.BusinessRuleId).ToList());
            var profiles = await _repository.GetAllCustomerProfilesAsync();

            return approvals.Select(approval => 
            {
                var rule = rules.FirstOrDefault(r => r.Id == approval.BusinessRuleId);
                var profile = rule != null 
                    ? profiles.FirstOrDefault(p => p.Id == rule.CustomerProfileId) 
                    : null;

                return new RuleApprovalDto
                {
                    Id = approval.Id,
                    BusinessRuleId = approval.BusinessRuleId,
                    CustomerProfileBand = profile?.ProfileBand,
                    NoIncomeProofLTV = rule?.NoIncomeProofLTV ?? 0,
                    MaxLTVWithProof = rule?.MaxLTVWithProof ?? 0,
                    FIRequirement = rule?.FIRequirement,
                    RequestedBy = approval.RequestedBy,
                    RequestedAt = approval.RequestedAt
                };
            }).ToList();
        }
    }
}

