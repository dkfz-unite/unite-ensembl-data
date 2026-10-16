using Ensembl.Data.Services.Configuration.Options;

namespace Ensembl.Data.Web.Configuration.Options;

public class EnsemblOptions : IEnsemblOptions
{
    // Ensembl 116 is the last release published on ftp.ensembl.org with MySQL database dumps.
    public string Release => "116";

    public string Genome => Environment.GetEnvironmentVariable("ENSEMBL_GENOME");
}
