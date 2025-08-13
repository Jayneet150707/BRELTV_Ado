using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using BRELTV.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRELTV.Infrastructure.Repositories
{
    public class BusinessRuleRepository : IBusinessRuleRepository
    {
        private readonly ApplicationDbContext _context;

        public BusinessRuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessRule> GetActiveRuleForProfileAsync(string profileBand)
        {
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(p => p.ProfileBand == profileBand && p.IsActive);

            if (profile == null)
            {
                return null;
            }

            return await _context.BusinessRules
                .Include(r => r.CustomerProfile)
                .FirstOrDefaultAsync(r => r.CustomerProfileId == profile.Id && r.IsActive && r.IsApproved);
        }

        public async Task<IEnumerable<BusinessRule>> GetAllActiveRulesAsync()
        {
            return await _context.BusinessRules
                .Include(r => r.CustomerProfile)
                .Where(r => r.IsActive && r.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<BusinessRule>> GetAllRulesAsync()
        {
            return await _context.BusinessRules
                .Include(r => r.CustomerProfile)
                .ToListAsync();
        }

        public async Task<BusinessRule> GetRuleByIdAsync(int id)
        {
            return await _context.BusinessRules
                .Include(r => r.CustomerProfile)
                .FirstOrDefaultAsync(r => r.Id == id);
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

        public async Task<bool> DeleteRuleAsync(int id)
        {
            var rule = await _context.BusinessRules.FindAsync(id);
            if (rule == null)
            {
                return false;
            }

            _context.BusinessRules.Remove(rule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CustomerProfile>> GetAllCustomerProfilesAsync()
        {
            return await _context.CustomerProfiles
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<CustomerProfile> GetCustomerProfileByBandAsync(string profileBand)
        {
            return await _context.CustomerProfiles
                .FirstOrDefaultAsync(p => p.ProfileBand == profileBand && p.IsActive);
        }

        public async Task<int> CreateRuleApprovalRequestAsync(RuleApproval ruleApproval)
        {
            ruleApproval.RequestedAt = DateTime.UtcNow;
            ruleApproval.ApprovalStatus = "Pending";

            _context.RuleApprovals.Add(ruleApproval);
            await _context.SaveChangesAsync();

            return ruleApproval.Id;
        }

        public async Task<IEnumerable<RuleApproval>> GetPendingApprovalsAsync()
        {
            return await _context.RuleApprovals
                .Include(r => r.BusinessRule)
                .ThenInclude(b => b.CustomerProfile)
                .Where(r => r.ApprovalStatus == "Pending")
                .ToListAsync();
        }

        public async Task ApproveRuleAsync(int ruleApprovalId, string approvedBy)
        {
            var approval = await _context.RuleApprovals
                .Include(r => r.BusinessRule)
                .FirstOrDefaultAsync(r => r.Id == ruleApprovalId);

            if (approval == null)
            {
                throw new KeyNotFoundException($"Rule approval with ID {ruleApprovalId} not found.");
            }

            if (approval.ApprovalStatus != "Pending")
            {
                throw new InvalidOperationException($"Rule approval with ID {ruleApprovalId} is not in Pending status.");
            }

            // Update the approval
            approval.ApprovalStatus = "Approved";
            approval.ApprovedBy = approvedBy;
            approval.ApprovedAt = DateTime.UtcNow;

            // Update the business rule
            var rule = approval.BusinessRule;
            rule.IsApproved = true;
            rule.IsActive = true;
            rule.ApprovedBy = approvedBy;
            rule.ApprovedAt = DateTime.UtcNow;

            // Deactivate any other active rules for the same profile
            var otherActiveRules = await _context.BusinessRules
                .Where(r => r.CustomerProfileId == rule.CustomerProfileId && r.IsActive && r.Id != rule.Id)
                .ToListAsync();

            foreach (var otherRule in otherActiveRules)
            {
                otherRule.IsActive = false;
                otherRule.UpdatedAt = DateTime.UtcNow;
                otherRule.UpdatedBy = approvedBy;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RejectRuleAsync(int ruleApprovalId, string rejectedBy, string comments)
        {
            var approval = await _context.RuleApprovals
                .Include(r => r.BusinessRule)
                .FirstOrDefaultAsync(r => r.Id == ruleApprovalId);

            if (approval == null)
            {
                throw new KeyNotFoundException($"Rule approval with ID {ruleApprovalId} not found.");
            }

            if (approval.ApprovalStatus != "Pending")
            {
                throw new InvalidOperationException($"Rule approval with ID {ruleApprovalId} is not in Pending status.");
            }

            // Update the approval
            approval.ApprovalStatus = "Rejected";
            approval.ApprovedBy = rejectedBy;
            approval.ApprovedAt = DateTime.UtcNow;
            approval.Comments = comments;

            await _context.SaveChangesAsync();
        }

        public async Task SaveLoanEvaluationAsync(LoanEvaluation evaluation)
        {
            _context.LoanEvaluations.Add(evaluation);
            await _context.SaveChangesAsync();
        }
    }
}

