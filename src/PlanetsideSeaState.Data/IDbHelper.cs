using Npgsql;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data
{
    public interface IDbHelper
    {
        /// <summary>
        /// Get a new connection to the PostgreSQL database.
        /// </summary>
        /// <returns>A new <see cref="NpgsqlConnection"/></returns>
        NpgsqlConnection CreateConnection();

        /// <summary>
        /// Get a new <see cref="NpgsqlCommand"/> with type object from SQL command text.
        /// </summary>
        /// <param name="cmdText">SQL text of the command</param>
        /// <returns>A <see cref="NpgsqlCommand"/> with a CommandType of CommandType.Text</returns>
        Task<NpgsqlCommand> CreateTextCommand(string cmdText);

        /// <summary>
        /// Get a new <see cref="NpgsqlCommand"/> object for executing a SQL stored procedure.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute</param>
        /// <returns>A <see cref="NpgsqlCommand"/> with a CommandType of CommandType.StoredProcedure</returns>
        Task<NpgsqlCommand> CreateStoredProcedureCommand(string procedureName);
    }
}
