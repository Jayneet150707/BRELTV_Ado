using BRELTV.Models.DTOs;
using BRELTV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessRulesController : ControllerBase
    {
        private readonly IBusinessRuleService _businessRuleService;

        public BusinessRulesController(IBusinessRuleService businessRuleService)
        {
            _businessRuleService = businessRuleService;
        }

        /// <summary>
        /// Gets all business rules
        /// </summary>
        /// <returns>List of business rules</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BusinessRuleDto>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<BusinessRuleDto>>> GetAllBusinessRules()
        {
            var rules = await _businessRuleService.GetAllBusinessRulesAsync();
            return Ok(rules);
        }

        /// <summary>
        /// Gets a business rule by ID
        /// </summary>
        /// <param name="id">Business rule ID</param>
        /// <returns>Business rule details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BusinessRuleDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<BusinessRuleDto>> GetBusinessRuleById(int id)
        {
            var rule = await _businessRuleService.GetBusinessRuleByIdAsync(id);
            if (rule == null)
            {
                return NotFound();
            }

            return Ok(rule);
        }

        /// <summary>
        /// Creates a new business rule
        /// </summary>
        /// <param name="businessRule">Business rule to create</param>
        /// <returns>Created business rule ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<int>> CreateBusinessRule([FromBody] BusinessRuleDto businessRule)
        {
            var createdBy = User.Identity?.Name ?? "System";
            var id = await _businessRuleService.CreateBusinessRuleAsync(businessRule, createdBy);
            return CreatedAtAction(nameof(GetBusinessRuleById), new { id }, id);
        }

        /// <summary>
        /// Gets pending business rule approvals
        /// </summary>
        /// <returns>List of pending business rule approvals</returns>
        [HttpGet("pending-approvals")]
        [ProducesResponseType(typeof(IEnumerable<BusinessRuleDto>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<BusinessRuleDto>>> GetPendingApprovals()
        {
            var pendingRules = await _businessRuleService.GetPendingApprovalsAsync();
            return Ok(pendingRules);
        }

        /// <summary>
        /// Approves a business rule
        /// </summary>
        /// <param name="id">Business rule ID</param>
        /// <param name="comments">Optional approval comments</param>
        /// <returns>Success status</returns>
        [HttpPost("approve/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult> ApproveBusinessRule(int id, [FromBody] string? comments = null)
        {
            var approvedBy = User.Identity?.Name ?? "System";
            var success = await _businessRuleService.ApproveBusinessRuleAsync(id, approvedBy, comments);
            if (!success)
            {
                return Problem("Failed to approve business rule", statusCode: 500);
            }

            return Ok();
        }

        /// <summary>
        /// Rejects a business rule
        /// </summary>
        /// <param name="id">Business rule ID</param>
        /// <param name="comments">Optional rejection comments</param>
        /// <returns>Success status</returns>
        [HttpPost("reject/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult> RejectBusinessRule(int id, [FromBody] string? comments = null)
        {
            var rejectedBy = User.Identity?.Name ?? "System";
            var success = await _businessRuleService.RejectBusinessRuleAsync(id, rejectedBy, comments);
            if (!success)
            {
                return Problem("Failed to reject business rule", statusCode: 500);
            }

            return Ok();
        }
    }
}

