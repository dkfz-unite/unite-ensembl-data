using Ensembl.Data.Services;
using Ensembl.Data.Services.Configuration.Options;
using Ensembl.Data.Web.Configuration.Options;
using Ensembl.Data.Web.Models.Liftover.Variants;
using Ensembl.Data.Web.Validators.Liftover.Variants;
using FluentValidation;

namespace Ensembl.Data.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
	public static void AddServices(this IServiceCollection services)
	{
		services.AddTransient<ISqlOptions, SqlOptions>();
		services.AddTransient<IEnsemblOptions, EnsemblOptions>();
		services.AddTransient<ILiftoverOptions, LiftoverOptions>();

		services.AddTransient<EnsemblDbContext37>();
		services.AddTransient<EnsemblDbContext38>();
		services.AddTransient<CoordinateLiftoverService>();
		services.AddTransient<IValidator<Sm[]>, SmsValidator>();
		services.AddTransient<IValidator<Cnv[]>, CnvsValidator>();

		// services.AddTransient<GeneSearchService>();
		// services.AddTransient<TranscriptSearchService>();
		// services.AddTransient<ProteinSearchService>();
	}
}
