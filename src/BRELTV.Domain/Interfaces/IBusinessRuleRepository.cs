using BRELTV.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.Domain.Interfaces
{
    public interface IBusinessRuleRepository
    {
        // Customer Profile operations
        Task<CustomerProfile> GetCustomerProfileByBandAsync(string profileBand);
        Task<CustomerProfile> GetCustomerProfileByIdAsync(int id);
        Task<List<CustomerProfile>> GetAllCustomerProfilesAsync();

        // Business Rule operations
        Task<BusinessRule> GetRuleByIdAsync(int id);
        Task<List<BusinessRule>> GetRulesByIdsAsync(List<int> ids);
        Task<List<BusinessRule>> GetAllRulesAsync(bool? isActive = null, bool? isApproved = null);
        Task<List<BusinessRule>> GetActiveRulesAsync();
        Task<int> CreateRuleAsync(BusinessRule rule);
        Task UpdateRuleAsync(BusinessRule rule);

        // Rule Approval operations
        Task<RuleApproval> GetRuleApprovalByIdAsync(int id);
        Task<List<RuleApproval>> GetPendingApprovalsAsync();
        Task<int> CreateRuleApprovalRequestAsync(RuleApproval approval);
        Task ApproveRuleAsync(int approvalId, string approvedBy);
        Task RejectRuleAsync(int approvalId, string rejectedBy, string comments);

        // Loan Evaluation operations
        Task<int> LogLoanEvaluationAsync(LoanEvaluation evaluation);
    }
}

