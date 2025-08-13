using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRELTV.UnitTests
{
    public class MockBusinessRuleRepository : IBusinessRuleRepository
    {
        private readonly List<CustomerProfile> _customerProfiles;
        private readonly List<BusinessRule> _businessRules;
        private readonly List<RuleApproval> _ruleApprovals;

        public MockBusinessRuleRepository()
        {
            // Initialize with test data
            _customerProfiles = new List<CustomerProfile>
            {
                new CustomerProfile { Id = 1, ProfileBand = "0", Description = "No Credit History", MinScore = 0, MaxScore = 0, IsActive = true },
                new CustomerProfile { Id = 2, ProfileBand = "-1", Description = "Negative Credit History", MinScore = -1, MaxScore = -1, IsActive = true },
                new CustomerProfile { Id = 3, ProfileBand = "650-700", Description = "Low Credit Score", MinScore = 650, MaxScore = 700, IsActive = true },
                new CustomerProfile { Id = 4, ProfileBand = "700-750", Description = "Medium Credit Score", MinScore = 700, MaxScore = 750, IsActive = true },
                new CustomerProfile { Id = 5, ProfileBand = "750+", Description = "High Credit Score", MinScore = 750, MaxScore = null, IsActive = true }
            };

            _businessRules = new List<BusinessRule>
            {
                new BusinessRule
                {
                    Id = 1,
                    CustomerProfileId = 1, // "0"
                    NoIncomeProofLTV = 75,
                    MaxLTVWithProof = 85,
                    FIRequirement = "FI Mandatory",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    ApprovedBy = "System",
                    ApprovedAt = DateTime.UtcNow
                },
                new BusinessRule
                {
                    Id = 2,
                    CustomerProfileId = 2, // "-1"
                    NoIncomeProofLTV = 75,
                    MaxLTVWithProof = 85,
                    FIRequirement = "FI Mandatory",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    ApprovedBy = "System",
                    ApprovedAt = DateTime.UtcNow
                },
                new BusinessRule
                {
                    Id = 3,
                    CustomerProfileId = 3, // "650-700"
                    NoIncomeProofLTV = 75,
                    MaxLTVWithProof = 85,
                    FIRequirement = "FI Mandatory",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    ApprovedBy = "System",
                    ApprovedAt = DateTime.UtcNow
                },
                new BusinessRule
                {
                    Id = 4,
                    CustomerProfileId = 4, // "700-750"
                    NoIncomeProofLTV = 80,
                    MaxLTVWithProof = 95,
                    FIRequirement = "FI Waiver",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    ApprovedBy = "System",
                    ApprovedAt = DateTime.UtcNow
                },
                new BusinessRule
                {
                    Id = 5,
                    CustomerProfileId = 5, // "750+"
                    NoIncomeProofLTV = 85,
                    MaxLTVWithProof = 100,
                    FIRequirement = "FI Waiver",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    ApprovedBy = "System",
                    ApprovedAt = DateTime.UtcNow
                }
            };

            _ruleApprovals = new List<RuleApproval>();
        }

        public Task<BusinessRule> GetActiveRuleForProfileAsync(string profileBand)
        {
            var profile = _customerProfiles.FirstOrDefault(p => p.ProfileBand.Trim().Equals(profileBand.Trim(), StringComparison.OrdinalIgnoreCase));
            if (profile == null)
            {
                return Task.FromResult<BusinessRule>(null);
            }

            var rule = _businessRules.FirstOrDefault(r => r.CustomerProfileId == profile.Id && r.IsActive && r.IsApproved);
            return Task.FromResult(rule);
        }

        public Task<IEnumerable<BusinessRule>> GetAllActiveRulesAsync()
        {
            return Task.FromResult(_businessRules.Where(r => r.IsActive && r.IsApproved).AsEnumerable());
        }

        public Task<BusinessRule> GetRuleByIdAsync(int id)
        {
            return Task.FromResult(_businessRules.FirstOrDefault(r => r.Id == id));
        }

        public Task<int> CreateRuleAsync(BusinessRule rule)
        {
            rule.Id = _businessRules.Max(r => r.Id) + 1;
            rule.CreatedAt = DateTime.UtcNow;
            _businessRules.Add(rule);
            return Task.FromResult(rule.Id);
        }

        public Task UpdateRuleAsync(BusinessRule rule)
        {
            var existingRule = _businessRules.FirstOrDefault(r => r.Id == rule.Id);
            if (existingRule != null)
            {
                existingRule.CustomerProfileId = rule.CustomerProfileId;
                existingRule.NoIncomeProofLTV = rule.NoIncomeProofLTV;
                existingRule.MaxLTVWithProof = rule.MaxLTVWithProof;
                existingRule.FIRequirement = rule.FIRequirement;
                existingRule.MinIncomeProofAmount = rule.MinIncomeProofAmount;
                existingRule.MinFloatingMoneyPercentage = rule.MinFloatingMoneyPercentage;
                existingRule.UpdatedAt = DateTime.UtcNow;
                existingRule.UpdatedBy = rule.UpdatedBy;
            }
            return Task.CompletedTask;
        }

        public Task<bool> DeleteRuleAsync(int id)
        {
            var rule = _businessRules.FirstOrDefault(r => r.Id == id);
            if (rule != null)
            {
                _businessRules.Remove(rule);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<CustomerProfile>> GetAllCustomerProfilesAsync()
        {
            return Task.FromResult(_customerProfiles.AsEnumerable());
        }

        public Task<CustomerProfile> GetCustomerProfileByBandAsync(string profileBand)
        {
            return Task.FromResult(_customerProfiles.FirstOrDefault(p => 
                p.ProfileBand.Trim().Equals(profileBand.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        public Task<int> CreateRuleApprovalRequestAsync(RuleApproval ruleApproval)
        {
            ruleApproval.Id = _ruleApprovals.Any() ? _ruleApprovals.Max(r => r.Id) + 1 : 1;
            ruleApproval.RequestedAt = DateTime.UtcNow;
            _ruleApprovals.Add(ruleApproval);
            return Task.FromResult(ruleApproval.Id);
        }

        public Task<IEnumerable<RuleApproval>> GetPendingApprovalsAsync()
        {
            return Task.FromResult(_ruleApprovals.Where(r => r.ApprovalStatus == "Pending").AsEnumerable());
        }

        public Task ApproveRuleAsync(int ruleApprovalId, string approvedBy)
        {
            var approval = _ruleApprovals.FirstOrDefault(r => r.Id == ruleApprovalId);
            if (approval != null)
            {
                approval.ApprovalStatus = "Approved";
                approval.ApprovedBy = approvedBy;
                approval.ApprovedAt = DateTime.UtcNow;

                var rule = _businessRules.FirstOrDefault(r => r.Id == approval.BusinessRuleId);
                if (rule != null)
                {
                    rule.IsApproved = true;
                    rule.ApprovedBy = approvedBy;
                    rule.ApprovedAt = DateTime.UtcNow;
                }
            }
            return Task.CompletedTask;
        }

        public Task RejectRuleAsync(int ruleApprovalId, string rejectedBy, string comments)
        {
            var approval = _ruleApprovals.FirstOrDefault(r => r.Id == ruleApprovalId);
            if (approval != null)
            {
                approval.ApprovalStatus = "Rejected";
                approval.ApprovedBy = rejectedBy;
                approval.ApprovedAt = DateTime.UtcNow;
                approval.Comments = comments;
            }
            return Task.CompletedTask;
        }
    }
}

