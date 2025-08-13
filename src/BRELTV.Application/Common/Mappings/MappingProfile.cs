using AutoMapper;
using BRELTV.Application.BusinessRules.Queries.GetBusinessRuleDetail;
using BRELTV.Application.BusinessRules.Queries.GetBusinessRulesList;
using BRELTV.Application.BusinessRules.Queries.GetPendingApprovals;
using BRELTV.Domain.Entities;
using System.Linq;

namespace BRELTV.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Business Rule mappings
            CreateMap<BusinessRule, BusinessRuleDto>()
                .ForMember(d => d.CustomerProfileBand, opt => opt.MapFrom(s => s.CustomerProfile.ProfileBand));

            CreateMap<BusinessRule, BusinessRuleDetailDto>()
                .ForMember(d => d.CustomerProfileBand, opt => opt.MapFrom(s => s.CustomerProfile.ProfileBand))
                .ForMember(d => d.CustomerProfileDescription, opt => opt.MapFrom(s => s.CustomerProfile.Description));

            // Rule Approval mappings
            CreateMap<RuleApproval, RuleApprovalDto>()
                .ForMember(d => d.CustomerProfileBand, opt => opt.MapFrom(s => s.BusinessRule.CustomerProfile.ProfileBand))
                .ForMember(d => d.NoIncomeProofLTV, opt => opt.MapFrom(s => s.BusinessRule.NoIncomeProofLTV))
                .ForMember(d => d.MaxLTVWithProof, opt => opt.MapFrom(s => s.BusinessRule.MaxLTVWithProof))
                .ForMember(d => d.FIRequirement, opt => opt.MapFrom(s => s.BusinessRule.FIRequirement));
        }
    }
}

