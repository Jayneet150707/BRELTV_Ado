using BRELTV.Models.Entities;
using BRELTV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerProfilesController : ControllerBase
    {
        private readonly ICustomerProfileService _customerProfileService;

        public CustomerProfilesController(ICustomerProfileService customerProfileService)
        {
            _customerProfileService = customerProfileService;
        }

        /// <summary>
        /// Gets all customer profiles
        /// </summary>
        /// <returns>List of customer profiles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerProfile>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<CustomerProfile>>> GetAllCustomerProfiles()
        {
            var profiles = await _customerProfileService.GetAllCustomerProfilesAsync();
            return Ok(profiles);
        }

        /// <summary>
        /// Gets a customer profile by ID
        /// </summary>
        /// <param name="id">Customer profile ID</param>
        /// <returns>Customer profile details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerProfile), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<CustomerProfile>> GetCustomerProfileById(int id)
        {
            var profile = await _customerProfileService.GetCustomerProfileByIdAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        /// <summary>
        /// Gets a customer profile by profile band
        /// </summary>
        /// <param name="profileBand">Customer profile band</param>
        /// <returns>Customer profile details</returns>
        [HttpGet("band/{profileBand}")]
        [ProducesResponseType(typeof(CustomerProfile), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<CustomerProfile>> GetCustomerProfileByBand(string profileBand)
        {
            var profile = await _customerProfileService.GetCustomerProfileByBandAsync(profileBand);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        /// <summary>
        /// Creates a new customer profile
        /// </summary>
        /// <param name="profile">Customer profile to create</param>
        /// <returns>Created customer profile ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<int>> CreateCustomerProfile([FromBody] CustomerProfile profile)
        {
            profile.CreatedBy = User.Identity?.Name ?? "System";
            var id = await _customerProfileService.CreateCustomerProfileAsync(profile);
            return CreatedAtAction(nameof(GetCustomerProfileById), new { id }, id);
        }

        /// <summary>
        /// Updates an existing customer profile
        /// </summary>
        /// <param name="id">Customer profile ID</param>
        /// <param name="profile">Updated customer profile</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult> UpdateCustomerProfile(int id, [FromBody] CustomerProfile profile)
        {
            if (id != profile.Id)
            {
                return BadRequest("ID in URL does not match ID in request body");
            }

            profile.UpdatedBy = User.Identity?.Name ?? "System";
            var success = await _customerProfileService.UpdateCustomerProfileAsync(profile);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

