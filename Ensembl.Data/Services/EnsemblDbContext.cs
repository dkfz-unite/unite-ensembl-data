using System;
using Ensembl.Data.Entities;
using Ensembl.Data.Services.Configuration.Options;
using Ensembl.Data.Services.Mappers.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Ensembl.Data.Services
{
    public class EnsemblDbContext : DbContext
    {
        private readonly string _database = "homo_sapiens_core_{0}"; //108_37
        private readonly string _connectionString;


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
            var database = string.Format(_database, ensemblOptions.Release);

            _connectionString = $"server={sqlOptions.Host};port={sqlOptions.Port};database={database};user={sqlOptions.User};password={sqlOptions.Password}";
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
            modelBuilder.Map();
        }
    }
}

