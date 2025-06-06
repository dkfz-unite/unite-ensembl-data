using Ensembl.Data.Attributes;
using Ensembl.Data.Entities;
using Ensembl.Data.Services.Configuration.Options;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services;

public abstract class EnsemblDbContext : DbContext
{
    private readonly string _connectionString;

    public const string _database = "homo_sapiens_core_{0}_{1}"; //113_37 or 113_38

    public abstract byte Genome { get; }


    public DbSet<AttribType> AttribTypes { get; set; }
    public DbSet<Biotype> Biotypes { get; set; }
    public DbSet<CoordSystem> CoordSystems { get; set; }
    public DbSet<Exon> Exons { get; set; }
    public DbSet<ExonTranscript> ExonTranscripts { get; set; }
    public DbSet<ExternalDb> ExternalDbs { get; set; }
    public DbSet<ExternalSynonym> ExternalSynonyms { get; set; }
    public DbSet<Gene> Genes { get; set; }
    public DbSet<GeneArchive> GeneArchives { get; set; }
    public DbSet<GeneAttrib> GeneAttribs { get; set; }
    public DbSet<Interpro> Interpros { get; set; }
    public DbSet<ObjectXref> ObjectXrefs { get; set; }
    public DbSet<OntologyXref> OntologyXrefs { get; set; }
    public DbSet<ProteinFeature> ProteinFeatures { get; set; }
    public DbSet<SeqRegion> SeqRegions { get; set; }
    public DbSet<SeqRegionAttrib> SeqRegionAttribs { get; set; }
    public DbSet<SeqRegionSynonym> SeqRegionSynonyms { get; set; }
    public DbSet<Transcript> Transcripts { get; set; }
    public DbSet<TranscriptAttrib> TranscriptAttribs { get; set; }
    public DbSet<Translation> Translations { get; set; }
    public DbSet<TranslationAttrib> TranslationAttribs { get; set; }
    public DbSet<Xref> Xrefs { get; set; }


    public EnsemblDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public EnsemblDbContext(ISqlOptions sqlOptions, IEnsemblOptions ensemblOptions)
    {
        _connectionString = CreateConnectionString(sqlOptions, ensemblOptions, Genome);
    }

    public static string CreateConnectionString(ISqlOptions sqlOptions, IEnsemblOptions ensemblOptions, byte genome)
    {
        var database = string.Format(_database, ensemblOptions.Release, genome);
        
        return $"server={sqlOptions.Host};port={sqlOptions.Port};database={database};user={sqlOptions.User};password={sqlOptions.Password}";
    }

    public int[] GetCoordSystemIds()
    {
        var name = "chromosome";
        var version = $"GRCh{Genome}";

        return CoordSystems
            .Where(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                     && e.Version.Equals(version, StringComparison.InvariantCultureIgnoreCase))
            .Select(e => e.CoordSystemId)
            .ToArray();
    }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL(_connectionString, b => b.MigrationsAssembly("Ensembl.Data.Migrations"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {  
            if (mutableEntityType.ClrType == null)
                continue;

            var members = mutableEntityType.ClrType.GetMembers()
                .Where(member => member.CustomAttributes
                    .Any(attr => attr.AttributeType == typeof(CompositeKeyAttribute))
                );

            var names = new List<string>();
            
            foreach (var member in members)
            {
                names.Add(member.Name);
            }
            
            if (names.Count > 0)
                modelBuilder.Entity(mutableEntityType.ClrType).HasKey(names.ToArray());
        }
    }
}
