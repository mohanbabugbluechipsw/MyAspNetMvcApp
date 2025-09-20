using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Create a static class for extension methods
    public static class DbConnectionExtensions
    {
        public static async Task OpenAsync(this IDbConnection connection)
        {
            if (connection is DbConnection dbConn)
            {
                await dbConn.OpenAsync(); // If the connection is a DbConnection, use its async Open
            }
            else
            {
                connection.Open(); // Fallback to synchronous if it's not a DbConnection
            }
        }
    }

}
