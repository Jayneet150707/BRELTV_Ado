using BRELTV.Models.DTOs;
using BRELTV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BRELTV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanEvaluationsController : ControllerBase
    {
        private readonly ILoanEvaluationService _loanEvaluationService;

        public LoanEvaluationsController(ILoanEvaluationService loanEvaluationService)
        {
            _loanEvaluationService = loanEvaluationService;
        }

        /// <summary>
        /// Evaluates a loan application and returns LTV and FI requirements
        /// </summary>
        /// <param name="request">Loan evaluation request</param>
        /// <returns>Loan evaluation result with LTV and FI requirements</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LoanEvaluationResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<LoanEvaluationResponse>> EvaluateLoan([FromBody] LoanEvaluationRequest request)
        {
            var result = await _loanEvaluationService.EvaluateLoanAsync(request);
            return Ok(result);
        }
    }
}

