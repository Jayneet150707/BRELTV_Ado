using MediatR;
using System.Collections.Generic;

namespace BRELTV.Application.BusinessRules.Queries.GetPendingApprovals
{
    public class GetPendingApprovalsQuery : IRequest<List<RuleApprovalDto>>
    {
    }
}

