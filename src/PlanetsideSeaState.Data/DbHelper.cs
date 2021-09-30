using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data
{
    public class DbHelper : IDbHelper
    {
        private readonly DatabaseOptions _dbOptions;
        
        public DbHelper(IOptions<DatabaseOptions> dbOptions)
        {
            _dbOptions = dbOptions.Value ?? throw new ArgumentNullException(nameof(dbOptions));
        }

        public NpgsqlConnection CreateConnection() => new(_dbOptions.DBConnectionString);
        
        public async Task<NpgsqlCommand> CreateTextCommand(string cmdText)
        {
            var connection = CreateConnection();

            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = cmdText;

            return command;
        }
        
        public async Task<NpgsqlCommand> CreateStoredProcedureCommand(string procedureName)
        {
            var connection = CreateConnection();

            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = procedureName;

            return command;
        }
    }
}
