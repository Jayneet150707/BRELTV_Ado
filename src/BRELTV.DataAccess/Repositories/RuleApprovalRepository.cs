using BRELTV.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.DataAccess.Repositories
{
    public class RuleApprovalRepository : BaseRepository
    {
        public RuleApprovalRepository(DatabaseConnection dbConnection, ILogger<RuleApprovalRepository> logger) 
            : base(dbConnection, logger)
        {
        }

        public async Task<IEnumerable<RuleApproval>> GetAllAsync()
        {
            const string sql = @"
                SELECT Id, BusinessRuleId, ApprovalStatus, RequestedBy, 
                       RequestedAt, ApprovedBy, ApprovedAt, Comments
                FROM RuleApprovals
                ORDER BY RequestedAt DESC";

            return await ExecuteReaderAsync(sql, MapRuleApproval);
        }

        public async Task<RuleApproval?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, BusinessRuleId, ApprovalStatus, RequestedBy, 
                       RequestedAt, ApprovedBy, ApprovedAt, Comments
                FROM RuleApprovals
                WHERE Id = @Id";

            return await ExecuteReaderSingleAsync(sql, MapRuleApproval, new { Id = id });
        }

        public async Task<IEnumerable<RuleApproval>> GetByBusinessRuleIdAsync(int businessRuleId)
        {
            const string sql = @"
                SELECT Id, BusinessRuleId, ApprovalStatus, RequestedBy, 
                       RequestedAt, ApprovedBy, ApprovedAt, Comments
                FROM RuleApprovals
                WHERE BusinessRuleId = @BusinessRuleId
                ORDER BY RequestedAt DESC";

            return await ExecuteReaderAsync(sql, MapRuleApproval, new { BusinessRuleId = businessRuleId });
        }

        public async Task<IEnumerable<RuleApproval>> GetPendingApprovalsAsync()
        {
            const string sql = @"
                SELECT ra.Id, ra.BusinessRuleId, ra.ApprovalStatus, ra.RequestedBy, 
                       ra.RequestedAt, ra.ApprovedBy, ra.ApprovedAt, ra.Comments
                FROM RuleApprovals ra
                JOIN BusinessRules br ON ra.BusinessRuleId = br.Id
                WHERE ra.ApprovalStatus = 'Pending' AND br.IsApproved = 0
                ORDER BY ra.RequestedAt ASC";

            return await ExecuteReaderAsync(sql, MapRuleApproval);
        }

        public async Task<int> CreateAsync(RuleApproval approval)
        {
            const string sql = @"
                INSERT INTO RuleApprovals (
                    BusinessRuleId, ApprovalStatus, RequestedBy, 
                    RequestedAt, Comments)
                VALUES (
                    @BusinessRuleId, @ApprovalStatus, @RequestedBy, 
                    @RequestedAt, @Comments);
                SELECT SCOPE_IDENTITY();";

            return await ExecuteScalarAsync<int>(sql, new
            {
                approval.BusinessRuleId,
                approval.ApprovalStatus,
                approval.RequestedBy,
                RequestedAt = DateTime.UtcNow,
                approval.Comments
            });
        }

        public async Task<bool> UpdateStatusAsync(int id, string status, string approvedBy, string? comments)
        {
            const string sql = @"
                UPDATE RuleApprovals
                SET ApprovalStatus = @Status,
                    ApprovedBy = @ApprovedBy,
                    ApprovedAt = @ApprovedAt,
                    Comments = CASE WHEN @Comments IS NULL THEN Comments ELSE @Comments END
                WHERE Id = @Id";

            var rowsAffected = await ExecuteNonQueryAsync(sql, new
            {
                Id = id,
                Status = status,
                ApprovedBy = approvedBy,
                ApprovedAt = DateTime.UtcNow,
                Comments = comments
            });

            return rowsAffected > 0;
        }

        private RuleApproval MapRuleApproval(SqlDataReader reader)
        {
            return new RuleApproval
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                BusinessRuleId = reader.GetInt32(reader.GetOrdinal("BusinessRuleId")),
                ApprovalStatus = reader.GetString(reader.GetOrdinal("ApprovalStatus")),
                RequestedBy = reader.GetString(reader.GetOrdinal("RequestedBy")),
                RequestedAt = reader.GetDateTime(reader.GetOrdinal("RequestedAt")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                ApprovedAt = reader.IsDBNull(reader.GetOrdinal("ApprovedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovedAt")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments"))
            };
        }
    }
}

