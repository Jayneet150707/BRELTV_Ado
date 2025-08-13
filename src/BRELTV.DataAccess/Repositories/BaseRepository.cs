using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BRELTV.DataAccess.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly DatabaseConnection _dbConnection;
        protected readonly ILogger _logger;

        protected BaseRepository(DatabaseConnection dbConnection, ILogger logger)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            try
            {
                using var connection = _dbConnection.CreateSqlConnection();
                using var command = CreateCommand(connection, sql, parameters);
                
                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scalar query: {Sql}", sql);
                throw;
            }
        }

        protected async Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null)
        {
            try
            {
                using var connection = _dbConnection.CreateSqlConnection();
                using var command = CreateCommand(connection, sql, parameters);
                
                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing non-query: {Sql}", sql);
                throw;
            }
        }

        protected async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, Func<SqlDataReader, T> map, object? parameters = null)
        {
            try
            {
                var results = new List<T>();
                using var connection = _dbConnection.CreateSqlConnection();
                using var command = CreateCommand(connection, sql, parameters);
                
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    results.Add(map(reader));
                }
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing reader query: {Sql}", sql);
                throw;
            }
        }

        protected async Task<T?> ExecuteReaderSingleAsync<T>(string sql, Func<SqlDataReader, T> map, object? parameters = null) where T : class
        {
            try
            {
                using var connection = _dbConnection.CreateSqlConnection();
                using var command = CreateCommand(connection, sql, parameters);
                
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return map(reader);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing reader single query: {Sql}", sql);
                throw;
            }
        }

        protected async Task<T> ExecuteInTransactionAsync<T>(Func<SqlConnection, SqlTransaction, Task<T>> operation)
        {
            using var connection = _dbConnection.CreateSqlConnection();
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = await operation(connection, transaction);
                transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private SqlCommand CreateCommand(SqlConnection connection, string sql, object? parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            return command;
        }

        private void AddParameters(SqlCommand command, object parameters)
        {
            var properties = parameters.GetType().GetProperties();
            
            foreach (var property in properties)
            {
                var value = property.GetValue(parameters);
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{property.Name}";
                parameter.Value = value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }
    }
}

