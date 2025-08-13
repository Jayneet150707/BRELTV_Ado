using BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BRELTV.API.Controllers
{
    public class LoanEvaluationsController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<LoanEvaluationResultDto>> EvaluateLoanApplication(EvaluateLoanApplicationCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}

