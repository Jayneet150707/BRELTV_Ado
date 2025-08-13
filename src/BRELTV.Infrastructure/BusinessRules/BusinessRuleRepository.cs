using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using BRELTV.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRELTV.Infrastructure.BusinessRules
{
    public class BusinessRuleRepository : IBusinessRuleRepository
    {
        private readonly ApplicationDbContext _context;

        public BusinessRuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Customer Profile operations
        public async Task<CustomerProfile> GetCustomerProfileByBandAsync(string profileBand)
        {
            return await _context.CustomerProfiles
                .FirstOrDefaultAsync(p => p.ProfileBand == profileBand);
        }

        public async Task<CustomerProfile> GetCustomerProfileByIdAsync(int id)
        {
            return await _context.CustomerProfiles.FindAsync(id);
        }

        public async Task<List<CustomerProfile>> GetAllCustomerProfilesAsync()
        {
            return await _context.CustomerProfiles
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        // Business Rule operations
        public async Task<BusinessRule> GetRuleByIdAsync(int id)
        {
            return await _context.BusinessRules.FindAsync(id);
        }

        public async Task<List<BusinessRule>> GetRulesByIdsAsync(List<int> ids)
        {
            return await _context.BusinessRules
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();
        }

        public async Task<List<BusinessRule>> GetAllRulesAsync(bool? isActive = null, bool? isApproved = null)
        {
            var query = _context.BusinessRules.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(r => r.IsActive == isActive.Value);
            }

            if (isApproved.HasValue)
            {
                query = query.Where(r => r.IsApproved == isApproved.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<BusinessRule>> GetActiveRulesAsync()
        {
            return await _context.BusinessRules
                .Where(r => r.IsActive && r.IsApproved)
                .ToListAsync();
        }

        public async Task<int> CreateRuleAsync(BusinessRule rule)
        {
            rule.CreatedAt = DateTime.UtcNow;
            rule.IsActive = false;
            rule.IsApproved = false;

            _context.BusinessRules.Add(rule);
            await _context.SaveChangesAsync();

            return rule.Id;
        }

        public async Task UpdateRuleAsync(BusinessRule rule)
        {
            rule.UpdatedAt = DateTime.UtcNow;
            _context.BusinessRules.Update(rule);
            await _context.SaveChangesAsync();
        }

        // Rule Approval operations
        public async Task<RuleApproval> GetRuleApprovalByIdAsync(int id)
        {
            return await _context.RuleApprovals.FindAsync(id);
        }

        public async Task<List<RuleApproval>> GetPendingApprovalsAsync()
        {
            return await _context.RuleApprovals
                .Where(a => a.ApprovalStatus == "Pending")
                .ToListAsync();
        }

        public async Task<int> CreateRuleApprovalRequestAsync(RuleApproval approval)
        {
            approval.ApprovalStatus = "Pending";
            approval.RequestedAt = DateTime.UtcNow;

            _context.RuleApprovals.Add(approval);
            await _context.SaveChangesAsync();

            return approval.Id;
        }

        public async Task ApproveRuleAsync(int approvalId, string approvedBy)
        {
            var approval = await _context.RuleApprovals.FindAsync(approvalId);
            if (approval == null)
            {
                throw new KeyNotFoundException($"Rule approval with ID {approvalId} not found.");
            }

            if (approval.ApprovalStatus != "Pending")
            {
                throw new InvalidOperationException($"Rule approval with ID {approvalId} is not in 'Pending' status.");
            }

            var rule = await _context.BusinessRules.FindAsync(approval.BusinessRuleId);
            if (rule == null)
            {
                throw new KeyNotFoundException($"Business rule with ID {approval.BusinessRuleId} not found.");
            }

            // Update the approval
            approval.ApprovalStatus = "Approved";
            approval.ApprovedBy = approvedBy;
            approval.ApprovedAt = DateTime.UtcNow;

            // Update the rule
            rule.IsActive = true;
            rule.IsApproved = true;
            rule.ApprovedBy = approvedBy;
            rule.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task RejectRuleAsync(int approvalId, string rejectedBy, string comments)
        {
            var approval = await _context.RuleApprovals.FindAsync(approvalId);
            if (approval == null)
            {
                throw new KeyNotFoundException($"Rule approval with ID {approvalId} not found.");
            }

            if (approval.ApprovalStatus != "Pending")
            {
                throw new InvalidOperationException($"Rule approval with ID {approvalId} is not in 'Pending' status.");
            }

            // Update the approval
            approval.ApprovalStatus = "Rejected";
            approval.ApprovedBy = rejectedBy;
            approval.ApprovedAt = DateTime.UtcNow;
            approval.Comments = comments;

            await _context.SaveChangesAsync();
        }

        // Loan Evaluation operations
        public async Task<int> LogLoanEvaluationAsync(LoanEvaluation evaluation)
        {
            _context.LoanEvaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            return evaluation.Id;
        }
    }
}

