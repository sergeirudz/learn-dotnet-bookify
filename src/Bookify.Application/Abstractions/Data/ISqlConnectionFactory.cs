using System.Data;

namespace Bookify.Application.Abstractions.Data;

/**
 * Creates a new connection to DB
 */
public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
