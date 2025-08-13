using BRELTV.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.DataAccess.Repositories
{
    public class LoanEvaluationRepository : BaseRepository
    {
        public LoanEvaluationRepository(DatabaseConnection dbConnection, ILogger<LoanEvaluationRepository> logger) 
            : base(dbConnection, logger)
        {
        }

        public async Task<IEnumerable<LoanEvaluation>> GetAllAsync(int limit = 100)
        {
            const string sql = @"
                SELECT TOP (@Limit) Id, CustomerProfileBand, IncomeProofAvailable, 
                       IncomeProofAmount, FloatingMoneyAfterExpensesAndEMI, 
                       AssignedLTV, FIRequirement, Reason, 
                       EvaluatedAt, EvaluatedBy
                FROM LoanEvaluations
                ORDER BY EvaluatedAt DESC";

            return await ExecuteReaderAsync(sql, MapLoanEvaluation, new { Limit = limit });
        }

        public async Task<LoanEvaluation?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, CustomerProfileBand, IncomeProofAvailable, 
                       IncomeProofAmount, FloatingMoneyAfterExpensesAndEMI, 
                       AssignedLTV, FIRequirement, Reason, 
                       EvaluatedAt, EvaluatedBy
                FROM LoanEvaluations
                WHERE Id = @Id";

            return await ExecuteReaderSingleAsync(sql, MapLoanEvaluation, new { Id = id });
        }

        public async Task<int> CreateAsync(LoanEvaluation evaluation)
        {
            const string sql = @"
                INSERT INTO LoanEvaluations (
                    CustomerProfileBand, IncomeProofAvailable, IncomeProofAmount, 
                    FloatingMoneyAfterExpensesAndEMI, AssignedLTV, FIRequirement, 
                    Reason, EvaluatedAt, EvaluatedBy)
                VALUES (
                    @CustomerProfileBand, @IncomeProofAvailable, @IncomeProofAmount, 
                    @FloatingMoneyAfterExpensesAndEMI, @AssignedLTV, @FIRequirement, 
                    @Reason, @EvaluatedAt, @EvaluatedBy);
                SELECT SCOPE_IDENTITY();";

            return await ExecuteScalarAsync<int>(sql, new
            {
                evaluation.CustomerProfileBand,
                evaluation.IncomeProofAvailable,
                evaluation.IncomeProofAmount,
                evaluation.FloatingMoneyAfterExpensesAndEMI,
                evaluation.AssignedLTV,
                evaluation.FIRequirement,
                evaluation.Reason,
                EvaluatedAt = DateTime.UtcNow,
                evaluation.EvaluatedBy
            });
        }

        private LoanEvaluation MapLoanEvaluation(SqlDataReader reader)
        {
            return new LoanEvaluation
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                CustomerProfileBand = reader.GetString(reader.GetOrdinal("CustomerProfileBand")),
                IncomeProofAvailable = reader.GetBoolean(reader.GetOrdinal("IncomeProofAvailable")),
                IncomeProofAmount = reader.GetDecimal(reader.GetOrdinal("IncomeProofAmount")),
                FloatingMoneyAfterExpensesAndEMI = reader.GetDecimal(reader.GetOrdinal("FloatingMoneyAfterExpensesAndEMI")),
                AssignedLTV = reader.GetDecimal(reader.GetOrdinal("AssignedLTV")),
                FIRequirement = reader.GetString(reader.GetOrdinal("FIRequirement")),
                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                EvaluatedAt = reader.GetDateTime(reader.GetOrdinal("EvaluatedAt")),
                EvaluatedBy = reader.GetString(reader.GetOrdinal("EvaluatedBy"))
            };
        }
    }
}

