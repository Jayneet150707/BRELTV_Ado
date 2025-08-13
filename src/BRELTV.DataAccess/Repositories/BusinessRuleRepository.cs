using BRELTV.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.DataAccess.Repositories
{
    public class BusinessRuleRepository : BaseRepository
    {
        public BusinessRuleRepository(DatabaseConnection dbConnection, ILogger<BusinessRuleRepository> logger) 
            : base(dbConnection, logger)
        {
        }

        public async Task<IEnumerable<BusinessRule>> GetAllAsync()
        {
            const string sql = @"
                SELECT br.Id, br.CustomerProfileId, cp.ProfileBand, br.NoIncomeProofLTV, 
                       br.MaxLTVWithProof, br.FIRequirement, br.MinIncomeProofAmount, 
                       br.MinFloatingMoneyPercentage, br.IsActive, br.IsApproved,
                       br.CreatedAt, br.CreatedBy, br.UpdatedAt, br.UpdatedBy, 
                       br.ApprovedAt, br.ApprovedBy
                FROM BusinessRules br
                JOIN CustomerProfiles cp ON br.CustomerProfileId = cp.Id
                ORDER BY cp.ProfileBand";

            return await ExecuteReaderAsync(sql, MapBusinessRule);
        }

        public async Task<BusinessRule?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT br.Id, br.CustomerProfileId, cp.ProfileBand, br.NoIncomeProofLTV, 
                       br.MaxLTVWithProof, br.FIRequirement, br.MinIncomeProofAmount, 
                       br.MinFloatingMoneyPercentage, br.IsActive, br.IsApproved,
                       br.CreatedAt, br.CreatedBy, br.UpdatedAt, br.UpdatedBy, 
                       br.ApprovedAt, br.ApprovedBy
                FROM BusinessRules br
                JOIN CustomerProfiles cp ON br.CustomerProfileId = cp.Id
                WHERE br.Id = @Id";

            return await ExecuteReaderSingleAsync(sql, MapBusinessRule, new { Id = id });
        }

        public async Task<BusinessRule?> GetByProfileBandAsync(string profileBand)
        {
            const string sql = @"
                SELECT br.Id, br.CustomerProfileId, cp.ProfileBand, br.NoIncomeProofLTV, 
                       br.MaxLTVWithProof, br.FIRequirement, br.MinIncomeProofAmount, 
                       br.MinFloatingMoneyPercentage, br.IsActive, br.IsApproved,
                       br.CreatedAt, br.CreatedBy, br.UpdatedAt, br.UpdatedBy, 
                       br.ApprovedAt, br.ApprovedBy
                FROM BusinessRules br
                JOIN CustomerProfiles cp ON br.CustomerProfileId = cp.Id
                WHERE cp.ProfileBand = @ProfileBand AND br.IsActive = 1 AND br.IsApproved = 1";

            return await ExecuteReaderSingleAsync(sql, MapBusinessRule, new { ProfileBand = profileBand });
        }

        public async Task<int> CreateAsync(BusinessRule businessRule)
        {
            const string sql = @"
                INSERT INTO BusinessRules (
                    CustomerProfileId, NoIncomeProofLTV, MaxLTVWithProof, FIRequirement,
                    MinIncomeProofAmount, MinFloatingMoneyPercentage, IsActive, IsApproved,
                    CreatedAt, CreatedBy)
                VALUES (
                    @CustomerProfileId, @NoIncomeProofLTV, @MaxLTVWithProof, @FIRequirement,
                    @MinIncomeProofAmount, @MinFloatingMoneyPercentage, @IsActive, @IsApproved,
                    @CreatedAt, @CreatedBy);
                SELECT SCOPE_IDENTITY();";

            return await ExecuteScalarAsync<int>(sql, new
            {
                businessRule.CustomerProfileId,
                businessRule.NoIncomeProofLTV,
                businessRule.MaxLTVWithProof,
                businessRule.FIRequirement,
                businessRule.MinIncomeProofAmount,
                businessRule.MinFloatingMoneyPercentage,
                businessRule.IsActive,
                businessRule.IsApproved,
                CreatedAt = DateTime.UtcNow,
                businessRule.CreatedBy
            });
        }

        public async Task<bool> UpdateAsync(BusinessRule businessRule)
        {
            const string sql = @"
                UPDATE BusinessRules
                SET NoIncomeProofLTV = @NoIncomeProofLTV,
                    MaxLTVWithProof = @MaxLTVWithProof,
                    FIRequirement = @FIRequirement,
                    MinIncomeProofAmount = @MinIncomeProofAmount,
                    MinFloatingMoneyPercentage = @MinFloatingMoneyPercentage,
                    IsActive = @IsActive,
                    IsApproved = @IsApproved,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    ApprovedAt = @ApprovedAt,
                    ApprovedBy = @ApprovedBy
                WHERE Id = @Id";

            var rowsAffected = await ExecuteNonQueryAsync(sql, new
            {
                businessRule.Id,
                businessRule.NoIncomeProofLTV,
                businessRule.MaxLTVWithProof,
                businessRule.FIRequirement,
                businessRule.MinIncomeProofAmount,
                businessRule.MinFloatingMoneyPercentage,
                businessRule.IsActive,
                businessRule.IsApproved,
                UpdatedAt = DateTime.UtcNow,
                businessRule.UpdatedBy,
                businessRule.ApprovedAt,
                businessRule.ApprovedBy
            });

            return rowsAffected > 0;
        }

        public async Task<bool> ApproveAsync(int id, string approvedBy)
        {
            const string sql = @"
                UPDATE BusinessRules
                SET IsApproved = 1,
                    ApprovedAt = @ApprovedAt,
                    ApprovedBy = @ApprovedBy
                WHERE Id = @Id";

            var rowsAffected = await ExecuteNonQueryAsync(sql, new
            {
                Id = id,
                ApprovedAt = DateTime.UtcNow,
                ApprovedBy = approvedBy
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<BusinessRule>> GetPendingApprovalsAsync()
        {
            const string sql = @"
                SELECT br.Id, br.CustomerProfileId, cp.ProfileBand, br.NoIncomeProofLTV, 
                       br.MaxLTVWithProof, br.FIRequirement, br.MinIncomeProofAmount, 
                       br.MinFloatingMoneyPercentage, br.IsActive, br.IsApproved,
                       br.CreatedAt, br.CreatedBy, br.UpdatedAt, br.UpdatedBy, 
                       br.ApprovedAt, br.ApprovedBy
                FROM BusinessRules br
                JOIN CustomerProfiles cp ON br.CustomerProfileId = cp.Id
                WHERE br.IsApproved = 0
                ORDER BY br.CreatedAt DESC";

            return await ExecuteReaderAsync(sql, MapBusinessRule);
        }

        private BusinessRule MapBusinessRule(SqlDataReader reader)
        {
            return new BusinessRule
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                CustomerProfileId = reader.GetInt32(reader.GetOrdinal("CustomerProfileId")),
                NoIncomeProofLTV = reader.GetDecimal(reader.GetOrdinal("NoIncomeProofLTV")),
                MaxLTVWithProof = reader.GetDecimal(reader.GetOrdinal("MaxLTVWithProof")),
                FIRequirement = reader.GetString(reader.GetOrdinal("FIRequirement")),
                MinIncomeProofAmount = reader.GetDecimal(reader.GetOrdinal("MinIncomeProofAmount")),
                MinFloatingMoneyPercentage = reader.GetDecimal(reader.GetOrdinal("MinFloatingMoneyPercentage")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                ApprovedAt = reader.IsDBNull(reader.GetOrdinal("ApprovedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovedAt")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy"))
            };
        }
    }
}

