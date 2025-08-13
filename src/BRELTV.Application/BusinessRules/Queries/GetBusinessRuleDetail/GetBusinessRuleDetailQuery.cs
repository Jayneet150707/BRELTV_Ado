using MediatR;

namespace BRELTV.Application.BusinessRules.Queries.GetBusinessRuleDetail
{
    public class GetBusinessRuleDetailQuery : IRequest<BusinessRuleDetailDto>
    {
        public int Id { get; set; }
    }
}

