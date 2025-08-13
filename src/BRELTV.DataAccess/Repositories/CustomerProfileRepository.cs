using BRELTV.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRELTV.DataAccess.Repositories
{
    public class CustomerProfileRepository : BaseRepository
    {
        public CustomerProfileRepository(DatabaseConnection dbConnection, ILogger<CustomerProfileRepository> logger) 
            : base(dbConnection, logger)
        {
        }

        public async Task<IEnumerable<CustomerProfile>> GetAllAsync()
        {
            const string sql = @"
                SELECT Id, ProfileBand, Description, MinScore, MaxScore, 
                       IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM CustomerProfiles
                ORDER BY CASE 
                    WHEN ProfileBand = '0' THEN 1
                    WHEN ProfileBand = '-1' THEN 2
                    WHEN ProfileBand LIKE '%-%' THEN 3
                    ELSE 4
                END, ProfileBand";

            return await ExecuteReaderAsync(sql, MapCustomerProfile);
        }

        public async Task<CustomerProfile?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, ProfileBand, Description, MinScore, MaxScore, 
                       IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM CustomerProfiles
                WHERE Id = @Id";

            return await ExecuteReaderSingleAsync(sql, MapCustomerProfile, new { Id = id });
        }

        public async Task<CustomerProfile?> GetByProfileBandAsync(string profileBand)
        {
            const string sql = @"
                SELECT Id, ProfileBand, Description, MinScore, MaxScore, 
                       IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM CustomerProfiles
                WHERE ProfileBand = @ProfileBand AND IsActive = 1";

            return await ExecuteReaderSingleAsync(sql, MapCustomerProfile, new { ProfileBand = profileBand });
        }

        public async Task<int> CreateAsync(CustomerProfile profile)
        {
            const string sql = @"
                INSERT INTO CustomerProfiles (
                    ProfileBand, Description, MinScore, MaxScore, 
                    IsActive, CreatedAt, CreatedBy)
                VALUES (
                    @ProfileBand, @Description, @MinScore, @MaxScore, 
                    @IsActive, @CreatedAt, @CreatedBy);
                SELECT SCOPE_IDENTITY();";

            return await ExecuteScalarAsync<int>(sql, new
            {
                profile.ProfileBand,
                profile.Description,
                profile.MinScore,
                profile.MaxScore,
                profile.IsActive,
                CreatedAt = DateTime.UtcNow,
                profile.CreatedBy
            });
        }

        public async Task<bool> UpdateAsync(CustomerProfile profile)
        {
            const string sql = @"
                UPDATE CustomerProfiles
                SET ProfileBand = @ProfileBand,
                    Description = @Description,
                    MinScore = @MinScore,
                    MaxScore = @MaxScore,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            var rowsAffected = await ExecuteNonQueryAsync(sql, new
            {
                profile.Id,
                profile.ProfileBand,
                profile.Description,
                profile.MinScore,
                profile.MaxScore,
                profile.IsActive,
                UpdatedAt = DateTime.UtcNow,
                profile.UpdatedBy
            });

            return rowsAffected > 0;
        }

        private CustomerProfile MapCustomerProfile(SqlDataReader reader)
        {
            return new CustomerProfile
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProfileBand = reader.GetString(reader.GetOrdinal("ProfileBand")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                MinScore = reader.IsDBNull(reader.GetOrdinal("MinScore")) ? null : reader.GetInt32(reader.GetOrdinal("MinScore")),
                MaxScore = reader.IsDBNull(reader.GetOrdinal("MaxScore")) ? null : reader.GetInt32(reader.GetOrdinal("MaxScore")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}

