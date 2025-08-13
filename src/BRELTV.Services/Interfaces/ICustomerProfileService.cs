using BRELTV.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.Services.Interfaces
{
    public interface ICustomerProfileService
    {
        Task<IEnumerable<CustomerProfile>> GetAllCustomerProfilesAsync();
        Task<CustomerProfile?> GetCustomerProfileByIdAsync(int id);
        Task<CustomerProfile?> GetCustomerProfileByBandAsync(string profileBand);
        Task<int> CreateCustomerProfileAsync(CustomerProfile profile);
        Task<bool> UpdateCustomerProfileAsync(CustomerProfile profile);
    }
}

