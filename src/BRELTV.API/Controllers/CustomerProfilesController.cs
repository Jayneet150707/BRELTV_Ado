using System.Collections.Generic;
using System.Threading.Tasks;
using BRELTV.Application.CustomerProfiles.Queries.GetCustomerProfilesList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BRELTV.API.Controllers
{
    public class CustomerProfilesController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CustomerProfileDto>>> GetProfiles()
        {
            return await Mediator.Send(new GetCustomerProfilesListQuery());
        }
    }
}

