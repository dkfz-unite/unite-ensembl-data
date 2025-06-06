using Ensembl.Data.Services.Configuration.Options;

namespace Ensembl.Data.Services;

public class EnsemblDbContext37 : EnsemblDbContext
{
    public override byte Genome => 37;

    public EnsemblDbContext37(string connectionString) : base(connectionString) {}

    public EnsemblDbContext37(ISqlOptions sqlOptions, IEnsemblOptions ensemblOptions) : base(sqlOptions, ensemblOptions) {}
}
