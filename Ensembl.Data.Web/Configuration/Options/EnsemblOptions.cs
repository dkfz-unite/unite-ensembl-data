using Ensembl.Data.Services.Configuration.Options;

namespace Ensembl.Data.Web.Configuration.Options;

public class EnsemblOptions : IEnsemblOptions
{
    public string Release => Environment.GetEnvironmentVariable("ENSEMBL_RELEASE");
}

