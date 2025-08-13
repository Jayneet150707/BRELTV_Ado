using BRELTV.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRELTV.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultDataAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Seed customer profiles if they don't exist
                if (!context.CustomerProfiles.Any())
                {
                    await context.CustomerProfiles.AddRangeAsync(GetDefaultCustomerProfiles());
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded default customer profiles");
                }

                // Seed business rules if they don't exist
                if (!context.BusinessRules.Any())
                {
                    var profiles = context.CustomerProfiles.ToList();
                    await context.BusinessRules.AddRangeAsync(GetDefaultBusinessRules(profiles));
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded default business rules");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private static List<CustomerProfile> GetDefaultCustomerProfiles()
        {
            return new List<CustomerProfile>
            {
                new CustomerProfile
                {
                    ProfileBand = "0",
                    Description = "No Credit History",
                    MinScore = 0,
                    MaxScore = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new CustomerProfile
                {
                    ProfileBand = "-1",
                    Description = "Negative Credit History",
                    MinScore = -1,
                    MaxScore = -1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new CustomerProfile
                {
                    ProfileBand = "650-700",
                    Description = "Low Credit Score",
                    MinScore = 650,
                    MaxScore = 700,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new CustomerProfile
                {
                    ProfileBand = "700-750",
                    Description = "Medium Credit Score",
                    MinScore = 700,
                    MaxScore = 750,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new CustomerProfile
                {
                    ProfileBand = "750+",
                    Description = "High Credit Score",
                    MinScore = 750,
                    MaxScore = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };
        }

        private static List<BusinessRule> GetDefaultBusinessRules(List<CustomerProfile> profiles)
        {
            var rules = new List<BusinessRule>();
            var now = DateTime.UtcNow;

            // Rule for profile "0"
            var profile0 = profiles.FirstOrDefault(p => p.ProfileBand == "0");
            if (profile0 != null)
            {
                rules.Add(new BusinessRule
                {
                    CustomerProfileId = profile0.Id,
                    NoIncomeProofLTV = 75,
                    MaxLTVWithProof = 85,
                    FIRequirement = "FI Mandatory",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = now,
                    CreatedBy = "System",
                    ApprovedAt = now,
                    ApprovedBy = "System"
                });
            }

            // Rule for profile "-1"
            var profileNeg1 = profiles.FirstOrDefault(p => p.ProfileBand == "-1");
            if (profileNeg1 != null)
            {
                rules.Add(new BusinessRule
                {
                    CustomerProfileId = profileNeg1.Id,
                    NoIncomeProofLTV = 75,
                    MaxLTVWithProof = 85,
                    FIRequirement = "FI Mandatory",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = now,
                    CreatedBy = "System",
                    ApprovedAt = now,
                    ApprovedBy = "System"
                });
            }

            // Rule for profile "650-700"
            var profile650 = profiles.FirstOrDefault(p => p.ProfileBand == "650-700");
            if (profile650 != null)
            {
                rules.Add(new BusinessRule
                {
                    CustomerProfileId = profile650.Id,
                    NoIncomeProofLTV = 75,
                    MaxLTVWithProof = 85,
                    FIRequirement = "FI Mandatory",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = now,
                    CreatedBy = "System",
                    ApprovedAt = now,
                    ApprovedBy = "System"
                });
            }

            // Rule for profile "700-750"
            var profile700 = profiles.FirstOrDefault(p => p.ProfileBand == "700-750");
            if (profile700 != null)
            {
                rules.Add(new BusinessRule
                {
                    CustomerProfileId = profile700.Id,
                    NoIncomeProofLTV = 80,
                    MaxLTVWithProof = 95,
                    FIRequirement = "FI Waiver",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = now,
                    CreatedBy = "System",
                    ApprovedAt = now,
                    ApprovedBy = "System"
                });
            }

            // Rule for profile "750+"
            var profile750 = profiles.FirstOrDefault(p => p.ProfileBand == "750+");
            if (profile750 != null)
            {
                rules.Add(new BusinessRule
                {
                    CustomerProfileId = profile750.Id,
                    NoIncomeProofLTV = 85,
                    MaxLTVWithProof = 100,
                    FIRequirement = "FI Waiver",
                    MinIncomeProofAmount = 25000,
                    MinFloatingMoneyPercentage = 50,
                    IsActive = true,
                    IsApproved = true,
                    CreatedAt = now,
                    CreatedBy = "System",
                    ApprovedAt = now,
                    ApprovedBy = "System"
                });
            }

            return rules;
        }
    }
}

