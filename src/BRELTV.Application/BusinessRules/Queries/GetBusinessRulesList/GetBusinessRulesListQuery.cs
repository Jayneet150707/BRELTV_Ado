using MediatR;
using System.Collections.Generic;

namespace BRELTV.Application.BusinessRules.Queries.GetBusinessRulesList
{
    public class GetBusinessRulesListQuery : IRequest<List<BusinessRuleDto>>
    {
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
    }
}

