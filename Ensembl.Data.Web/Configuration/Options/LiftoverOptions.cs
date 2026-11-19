using Ensembl.Data.Services.Configuration.Options;

namespace Ensembl.Data.Web.Configuration.Options;

public class LiftoverOptions : ILiftoverOptions
{
    public string ExecutablePath => Environment.GetEnvironmentVariable("ENSEMBL_LIFTOVER_EXECUTABLE") ?? "liftOver";

    public string ChainsDirectoryPath => Environment.GetEnvironmentVariable("ENSEMBL_LIFTOVER_CHAINS_PATH") ?? "/chains";
}
