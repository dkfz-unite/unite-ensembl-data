using Ensembl.Data.Services;
using Ensembl.Data.Services.Configuration.Options;
using Ensembl.Data.Web.Configuration.Options;

namespace Ensembl.Data.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
	public static void AddServices(this IServiceCollection services)
	{
		services.AddTransient<ISqlOptions, SqlOptions>();
		services.AddTransient<IEnsemblOptions, EnsemblOptions>();

		services.AddTransient<EnsemblDbContext37>();
		services.AddTransient<EnsemblDbContext38>();

		// services.AddTransient<GeneSearchService>();
		// services.AddTransient<TranscriptSearchService>();
		// services.AddTransient<ProteinSearchService>();
	}
}
