using BRELTV.Application.BusinessRules.Commands.ApproveBusinessRule;
using BRELTV.Application.BusinessRules.Commands.CreateBusinessRule;
using BRELTV.Application.BusinessRules.Commands.RejectBusinessRule;
using BRELTV.Application.BusinessRules.Queries.GetBusinessRuleDetail;
using BRELTV.Application.BusinessRules.Queries.GetBusinessRulesList;
using BRELTV.Application.BusinessRules.Queries.GetPendingApprovals;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.API.Controllers
{
    public class BusinessRulesController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<BusinessRuleDto>>> GetAllRules([FromQuery] GetBusinessRulesListQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessRuleDetailDto>> GetRuleById(int id)
        {
            return await Mediator.Send(new GetBusinessRuleDetailQuery { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateRule(CreateBusinessRuleCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("pending-approvals")]
        public async Task<ActionResult<List<RuleApprovalDto>>> GetPendingApprovals()
        {
            return await Mediator.Send(new GetPendingApprovalsQuery());
        }

        [HttpPost("approve/{id}")]
        public async Task<ActionResult> ApproveRule(int id, ApproveBusinessRuleCommand command)
        {
            if (id != command.RuleApprovalId)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPost("reject/{id}")]
        public async Task<ActionResult> RejectRule(int id, RejectBusinessRuleCommand command)
        {
            if (id != command.RuleApprovalId)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }
    }
}

