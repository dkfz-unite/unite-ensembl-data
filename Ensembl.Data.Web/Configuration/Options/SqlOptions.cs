using Ensembl.Data.Services.Configuration.Options;

namespace Ensembl.Data.Web.Configuration.Options;

public class SqlOptions : ISqlOptions
{
    public string Host => Environment.GetEnvironmentVariable("ENSEMBL_SQL_HOST");
    public string Port => Environment.GetEnvironmentVariable("ENSEMBL_SQL_PORT");
    public string User => Environment.GetEnvironmentVariable("ENSEMBL_SQL_USER");
    public string Password => Environment.GetEnvironmentVariable("ENSEMBL_SQL_PASSWORD");
}

