using BRELTV.DataAccess.Repositories;
using BRELTV.Models.Entities;
using BRELTV.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.Services
{
    public class CustomerProfileService : ICustomerProfileService
    {
        private readonly CustomerProfileRepository _customerProfileRepository;
        private readonly ILogger<CustomerProfileService> _logger;

        public CustomerProfileService(
            CustomerProfileRepository customerProfileRepository,
            ILogger<CustomerProfileService> logger)
        {
            _customerProfileRepository = customerProfileRepository ?? throw new ArgumentNullException(nameof(customerProfileRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<CustomerProfile>> GetAllCustomerProfilesAsync()
        {
            return await _customerProfileRepository.GetAllAsync();
        }

        public async Task<CustomerProfile?> GetCustomerProfileByIdAsync(int id)
        {
            return await _customerProfileRepository.GetByIdAsync(id);
        }

        public async Task<CustomerProfile?> GetCustomerProfileByBandAsync(string profileBand)
        {
            return await _customerProfileRepository.GetByProfileBandAsync(profileBand);
        }

        public async Task<int> CreateCustomerProfileAsync(CustomerProfile profile)
        {
            // Normalize profile band
            profile.ProfileBand = NormalizeProfileBand(profile.ProfileBand);
            
            // Set created date
            profile.CreatedAt = DateTime.UtcNow;
            
            return await _customerProfileRepository.CreateAsync(profile);
        }

        public async Task<bool> UpdateCustomerProfileAsync(CustomerProfile profile)
        {
            // Normalize profile band
            profile.ProfileBand = NormalizeProfileBand(profile.ProfileBand);
            
            // Set updated date
            profile.UpdatedAt = DateTime.UtcNow;
            
            return await _customerProfileRepository.UpdateAsync(profile);
        }

        private string NormalizeProfileBand(string profileBand)
        {
            // Trim and normalize profile band
            return profileBand.Trim().ToUpperInvariant() switch
            {
                "0" => "0",
                "-1" => "-1",
                "650-700" or "650 - 700" or "650- 700" or "650 -700" => "650-700",
                "700-750" or "700 - 750" or "700- 750" or "700 -750" => "700-750",
                "750+" or "750 +" => "750+",
                _ => profileBand.Trim()
            };
        }
    }
}

