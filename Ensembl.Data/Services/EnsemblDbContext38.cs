using Ensembl.Data.Services.Configuration.Options;

namespace Ensembl.Data.Services;

public class EnsemblDbContext38 : EnsemblDbContext
{
    public override byte Genome => 38;

    public EnsemblDbContext38(string connectionString) : base(connectionString) {}

    public EnsemblDbContext38(ISqlOptions sqlOptions, IEnsemblOptions ensemblOptions) : base(sqlOptions, ensemblOptions) {}
}
