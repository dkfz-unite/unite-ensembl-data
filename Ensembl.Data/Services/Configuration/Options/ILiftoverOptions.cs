namespace Ensembl.Data.Services.Configuration.Options;

public interface ILiftoverOptions
{
    string ExecutablePath { get; }
    string ChainsDirectoryPath { get; }
}
