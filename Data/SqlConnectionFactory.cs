using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ECFautoecole.Data
{
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            // le nom DOIT matcher la clé dans appsettings: ConnectionStrings.ECF_AEL
            string? cs = configuration.GetConnectionString("ECF_AEL");
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("ConnectionStrings:ECF_AEL introuvable ou vide.");
            _connectionString = cs;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
