using BRELTV.Models.DTOs;
using BRELTV.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.Services.Interfaces
{
    public interface IBusinessRuleService
    {
        Task<IEnumerable<BusinessRuleDto>> GetAllBusinessRulesAsync();
        Task<BusinessRuleDto?> GetBusinessRuleByIdAsync(int id);
        Task<int> CreateBusinessRuleAsync(BusinessRuleDto businessRule, string createdBy);
        Task<bool> UpdateBusinessRuleAsync(BusinessRuleDto businessRule, string updatedBy);
        Task<bool> ApproveBusinessRuleAsync(int id, string approvedBy, string? comments = null);
        Task<bool> RejectBusinessRuleAsync(int id, string rejectedBy, string? comments = null);
        Task<IEnumerable<BusinessRuleDto>> GetPendingApprovalsAsync();
    }
}

