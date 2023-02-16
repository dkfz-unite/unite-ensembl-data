using Ensembl.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services.Mappers.Extensions;

internal static class MappingExtensions
{
    internal static void Map(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttribType>(entity =>
        {
            entity.ToTable("attrib_type");

            entity.HasIndex(e => e.Code, "code_idx")
                .IsUnique();

            entity.Property(e => e.AttribTypeId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("attrib_type_id");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("code")
                .HasDefaultValueSql("''");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Biotype>(entity =>
        {
            entity.ToTable("biotype");

            entity.HasIndex(e => new { e.Name, e.ObjectType }, "name_type_idx")
                .IsUnique();

            entity.Property(e => e.BiotypeId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("biotype_id");

            entity.Property(e => e.AttribTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("attrib_type_id");

            entity.Property(e => e.BiotypeGroup)
                .HasColumnType("enum('coding','pseudogene','snoncoding','lnoncoding','mnoncoding','LRG','undefined','no_group')")
                .HasColumnName("biotype_group");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnName("name");

            entity.Property(e => e.ObjectType)
                .IsRequired()
                .HasColumnType("enum('gene','transcript')")
                .HasColumnName("object_type")
                .HasDefaultValueSql("'gene'");

            entity.Property(e => e.SoAcc)
                .HasMaxLength(64)
                .HasColumnName("so_acc");

            entity.Property(e => e.SoTerm)
                .HasMaxLength(1023)
                .HasColumnName("so_term");
        });

        modelBuilder.Entity<CoordSystem>(entity =>
        {
            entity.ToTable("coord_system");

            entity.HasIndex(e => new { e.Name, e.Version, e.SpeciesId }, "name_idx")
                .IsUnique();

            entity.HasIndex(e => new { e.Rank, e.SpeciesId }, "rank_idx")
                .IsUnique();

            entity.HasIndex(e => e.SpeciesId, "species_idx");

            entity.Property(e => e.CoordSystemId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("coord_system_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("name");

            entity.Property(e => e.Rank)
                .HasColumnType("int(11)")
                .HasColumnName("rank");

            entity.Property(e => e.SpeciesId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("species_id")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.Version)
                .HasMaxLength(255)
                .HasColumnName("version");
        });

        modelBuilder.Entity<Exon>(entity =>
        {
            entity.ToTable("exon");

            entity.HasIndex(e => new { e.SeqRegionId, e.SeqRegionStart }, "seq_region_idx");

            entity.HasIndex(e => new { e.StableId, e.Version }, "stable_id_idx");

            entity.Property(e => e.ExonId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("exon_id");

            entity.Property(e => e.CreatedDate).HasColumnName("created_date");

            entity.Property(e => e.EndPhase)
                //.HasColumnType("tinyint(2)")
                .HasColumnName("end_phase");

            entity.Property(e => e.IsConstitutive).HasColumnName("is_constitutive");

            entity.Property(e => e.IsCurrent)
                .IsRequired()
                .HasColumnName("is_current")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");

            entity.Property(e => e.Phase)
                //.HasColumnType("tinyint(2)")
                .HasColumnName("phase");

            entity.Property(e => e.SeqRegionEnd)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_end");

            entity.Property(e => e.SeqRegionId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_id");

            entity.Property(e => e.SeqRegionStart)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_start");

            entity.Property(e => e.SeqRegionStrand)
                //.HasColumnType("tinyint(2)")
                .HasColumnName("seq_region_strand");

            entity.Property(e => e.StableId)
                .HasMaxLength(128)
                .HasColumnName("stable_id");

            entity.Property(e => e.Version)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("version");

            entity.HasOne(e => e.SeqRegion)
                  .WithMany()
                  .HasForeignKey(e => e.SeqRegionId);
        });

        modelBuilder.Entity<ExonTranscript>(entity =>
        {
            entity.HasKey(e => new { e.ExonId, e.TranscriptId, e.Rank })
                .HasName("PRIMARY");

            entity.ToTable("exon_transcript");

            entity.HasIndex(e => e.ExonId, "exon");

            entity.HasIndex(e => e.TranscriptId, "transcript");

            entity.Property(e => e.ExonId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("exon_id");

            entity.Property(e => e.TranscriptId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("transcript_id");

            entity.Property(e => e.Rank)
                .HasColumnType("int(10)")
                .HasColumnName("rank");

            entity.HasOne(e => e.Exon)
                  .WithMany()
                  .HasForeignKey(e => e.ExonId);

            entity.HasOne(e => e.Transcript)
                  .WithMany(e => e.TranscriptExons)
                  .HasForeignKey(e => e.TranscriptId);
        });

        modelBuilder.Entity<ExternalDb>(entity =>
        {
            entity.ToTable("external_db");

            entity.HasIndex(e => new { e.DbName, e.DbRelease }, "db_name_db_release_idx")
                .IsUnique();

            entity.Property(e => e.ExternalDbId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("external_db_id");

            entity.Property(e => e.DbDisplayName)
                .HasMaxLength(255)
                .HasColumnName("db_display_name");

            entity.Property(e => e.DbName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("db_name");

            entity.Property(e => e.DbRelease)
                .HasMaxLength(255)
                .HasColumnName("db_release");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.Priority)
                .HasColumnType("int(11)")
                .HasColumnName("priority");

            entity.Property(e => e.SecondaryDbName)
                .HasMaxLength(255)
                .HasColumnName("secondary_db_name");

            entity.Property(e => e.SecondaryDbTable)
                .HasMaxLength(255)
                .HasColumnName("secondary_db_table");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnType("enum('KNOWNXREF','KNOWN','XREF','PRED','ORTH','PSEUDO')")
                .HasColumnName("status");

            entity.Property(e => e.Type)
                .IsRequired()
                .HasColumnType("enum('ARRAY','ALT_TRANS','ALT_GENE','MISC','LIT','PRIMARY_DB_SYNONYM','ENSEMBL')")
                .HasColumnName("type");
        });

        modelBuilder.Entity<ExternalSynonym>(entity =>
        {
            entity.HasKey(e => new { e.XrefId, e.Synonym })
                .HasName("PRIMARY");

            entity.ToTable("external_synonym");

            entity.HasIndex(e => e.Synonym, "name_index");

            entity.Property(e => e.XrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("xref_id");

            entity.Property(e => e.Synonym)
                .HasMaxLength(100)
                .HasColumnName("synonym");

            entity.HasOne(e => e.Xref)
                  .WithMany()
                  .HasForeignKey(e => e.XrefId);
        });

        modelBuilder.Entity<Gene>(entity =>
        {
            entity.ToTable("gene");

            entity.HasIndex(e => e.AnalysisId, "analysis_idx");

            entity.HasIndex(e => e.CanonicalTranscriptId, "canonical_transcript_id_idx");

            entity.HasIndex(e => new { e.SeqRegionId, e.SeqRegionStart }, "seq_region_idx");

            entity.HasIndex(e => new { e.StableId, e.Version }, "stable_id_idx");

            entity.HasIndex(e => e.DisplayXrefId, "xref_id_index");

            entity.Property(e => e.GeneId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("gene_id");

            entity.Property(e => e.AnalysisId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("analysis_id");

            entity.Property(e => e.Biotype)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("biotype");

            entity.Property(e => e.CanonicalTranscriptId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("canonical_transcript_id");

            entity.Property(e => e.CreatedDate).HasColumnName("created_date");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.DisplayXrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("display_xref_id");

            entity.Property(e => e.IsCurrent)
                .IsRequired()
                .HasColumnName("is_current")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");

            entity.Property(e => e.SeqRegionEnd)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_end");

            entity.Property(e => e.SeqRegionId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_id");

            entity.Property(e => e.SeqRegionStart)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_start");

            entity.Property(e => e.SeqRegionStrand)
                //.HasColumnType("tinyint(2)")
                .HasColumnName("seq_region_strand");

            entity.Property(e => e.Source)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("source");

            entity.Property(e => e.StableId)
                .HasMaxLength(128)
                .HasColumnName("stable_id");

            entity.Property(e => e.Version)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("version");

            entity.HasOne(e => e.SeqRegion)
                  .WithMany()
                  .HasForeignKey(e => e.SeqRegionId);

            entity.HasOne(e => e.Xref)
                  .WithMany()
                  .HasForeignKey(e => e.DisplayXrefId);

            entity.HasOne(e => e.Transcript)
                  .WithOne()
                  .HasForeignKey<Gene>(e => e.CanonicalTranscriptId);
        });

        modelBuilder.Entity<GeneArchive>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("gene_archive");

            entity.HasIndex(e => new { e.GeneStableId, e.GeneVersion }, "gene_idx");

            entity.HasIndex(e => e.PeptideArchiveId, "peptide_archive_id_idx");

            entity.HasIndex(e => new { e.TranscriptStableId, e.TranscriptVersion }, "transcript_idx");

            entity.HasIndex(e => new { e.TranslationStableId, e.TranslationVersion }, "translation_idx");

            entity.Property(e => e.GeneStableId)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("gene_stable_id");

            entity.Property(e => e.GeneVersion)
                .HasColumnType("smallint(6)")
                .HasColumnName("gene_version")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.MappingSessionId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("mapping_session_id");

            entity.Property(e => e.PeptideArchiveId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("peptide_archive_id");

            entity.Property(e => e.TranscriptStableId)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("transcript_stable_id");

            entity.Property(e => e.TranscriptVersion)
                .HasColumnType("smallint(6)")
                .HasColumnName("transcript_version")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.TranslationStableId)
                .HasMaxLength(128)
                .HasColumnName("translation_stable_id");

            entity.Property(e => e.TranslationVersion)
                .HasColumnType("smallint(6)")
                .HasColumnName("translation_version")
                .HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<GeneAttrib>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("gene_attrib");

            entity.HasIndex(e => new { e.GeneId, e.AttribTypeId, e.Value }, "gene_attribx")
                .IsUnique();

            entity.HasIndex(e => e.GeneId, "gene_idx");

            entity.HasIndex(e => new { e.AttribTypeId, e.Value }, "type_val_idx");

            entity.HasIndex(e => e.Value, "val_only_idx");

            entity.Property(e => e.AttribTypeId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("attrib_type_id");

            entity.Property(e => e.GeneId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("gene_id");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Interpro>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("interpro");

            entity.HasIndex(e => new { e.InterproAc, e.Id }, "accession_idx")
                .IsUnique();

            entity.HasIndex(e => e.Id, "id_idx");

            entity.Property(e => e.Id)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("id");

            entity.Property(e => e.InterproAc)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("interpro_ac");
        });

        modelBuilder.Entity<ObjectXref>(entity =>
        {
            entity.ToTable("object_xref");

            entity.HasIndex(e => e.AnalysisId, "analysis_idx");

            entity.HasIndex(e => new { e.EnsemblObjectType, e.EnsemblId }, "ensembl_idx");

            entity.HasIndex(e => new { e.XrefId, e.EnsemblObjectType, e.EnsemblId, e.AnalysisId }, "xref_idx")
                .IsUnique();

            entity.Property(e => e.ObjectXrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("object_xref_id");

            entity.Property(e => e.AnalysisId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("analysis_id");

            entity.Property(e => e.EnsemblId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("ensembl_id");

            entity.Property(e => e.EnsemblObjectType)
                .IsRequired()
                .HasColumnType("enum('RawContig','Transcript','Gene','Translation','Operon','OperonTranscript','Marker','RNAProduct')")
                .HasColumnName("ensembl_object_type");

            entity.Property(e => e.LinkageAnnotation)
                .HasMaxLength(255)
                .HasColumnName("linkage_annotation");

            entity.Property(e => e.XrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("xref_id");
        });

        modelBuilder.Entity<OntologyXref>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("ontology_xref");

            entity.HasIndex(e => e.ObjectXrefId, "object_idx");

            entity.HasIndex(e => new { e.ObjectXrefId, e.SourceXrefId, e.LinkageType }, "object_source_type_idx")
                .IsUnique();

            entity.HasIndex(e => e.SourceXrefId, "source_idx");

            entity.Property(e => e.LinkageType)
                .HasMaxLength(3)
                .HasColumnName("linkage_type");

            entity.Property(e => e.ObjectXrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("object_xref_id");

            entity.Property(e => e.SourceXrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("source_xref_id");
        });

        modelBuilder.Entity<ProteinFeature>(entity =>
        {
            entity.ToTable("protein_feature");

            entity.HasIndex(e => new { e.TranslationId, e.HitName, e.SeqStart, e.SeqEnd, e.HitStart, e.HitEnd, e.AnalysisId }, "aln_idx")
                .IsUnique();

            entity.HasIndex(e => e.AnalysisId, "analysis_idx");

            entity.HasIndex(e => e.HitName, "hitname_idx");

            entity.HasIndex(e => e.TranslationId, "translation_idx");

            entity.Property(e => e.ProteinFeatureId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("protein_feature_id");

            entity.Property(e => e.AlignType)
                .HasColumnType("enum('ensembl','cigar','cigarplus','vulgar','mdtag')")
                .HasColumnName("align_type");

            entity.Property(e => e.AnalysisId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("analysis_id");

            entity.Property(e => e.CigarLine).HasColumnName("cigar_line");

            entity.Property(e => e.Evalue).HasColumnName("evalue");

            entity.Property(e => e.ExternalData).HasColumnName("external_data");

            entity.Property(e => e.HitDescription).HasColumnName("hit_description");

            entity.Property(e => e.HitEnd)
                .HasColumnType("int(10)")
                .HasColumnName("hit_end");

            entity.Property(e => e.HitName)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("hit_name");

            entity.Property(e => e.HitStart)
                .HasColumnType("int(10)")
                .HasColumnName("hit_start");

            entity.Property(e => e.PercIdent).HasColumnName("perc_ident");

            entity.Property(e => e.Score).HasColumnName("score");

            entity.Property(e => e.SeqEnd)
                .HasColumnType("int(10)")
                .HasColumnName("seq_end");

            entity.Property(e => e.SeqStart)
                .HasColumnType("int(10)")
                .HasColumnName("seq_start");

            entity.Property(e => e.TranslationId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("translation_id");

            entity.HasOne(e => e.Translation)
                  .WithMany(e => e.ProteinFeatures)
                  .HasForeignKey(e => e.TranslationId);
        });

        modelBuilder.Entity<SeqRegion>(entity =>
        {
            entity.ToTable("seq_region");

            entity.HasIndex(e => e.CoordSystemId, "cs_idx");

            entity.HasIndex(e => new { e.Name, e.CoordSystemId }, "name_cs_idx")
                .IsUnique();

            entity.Property(e => e.SeqRegionId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_id");

            entity.Property(e => e.CoordSystemId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("coord_system_id");

            entity.Property(e => e.Length)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("length");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(e => e.CoordSystem)
                  .WithMany()
                  .HasForeignKey(e => e.CoordSystemId);
        });

        modelBuilder.Entity<SeqRegionAttrib>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("seq_region_attrib");

            entity.HasIndex(e => new { e.SeqRegionId, e.AttribTypeId, e.Value }, "region_attribx")
                .IsUnique();

            entity.HasIndex(e => e.SeqRegionId, "seq_region_idx");

            entity.HasIndex(e => new { e.AttribTypeId, e.Value }, "type_val_idx");

            entity.HasIndex(e => e.Value, "val_only_idx");

            entity.Property(e => e.AttribTypeId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("attrib_type_id");

            entity.Property(e => e.SeqRegionId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_id");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("value");
        });

        modelBuilder.Entity<SeqRegionSynonym>(entity =>
        {
            entity.ToTable("seq_region_synonym");

            entity.HasIndex(e => e.SeqRegionId, "seq_region_idx");

            entity.HasIndex(e => new { e.Synonym, e.SeqRegionId }, "syn_idx")
                .IsUnique();

            entity.Property(e => e.SeqRegionSynonymId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_synonym_id");

            entity.Property(e => e.ExternalDbId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("external_db_id");

            entity.Property(e => e.SeqRegionId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_id");

            entity.Property(e => e.Synonym)
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnName("synonym");

            entity.HasOne(e => e.SeqRegion)
                  .WithMany(e => e.SeqRegionSynonyms)
                  .HasForeignKey(e => e.SeqRegionId);

            entity.HasOne(e => e.ExternalDb)
                  .WithMany()
                  .HasForeignKey(e => e.ExternalDbId);
        });

        modelBuilder.Entity<Transcript>(entity =>
        {
            entity.ToTable("transcript");

            entity.HasIndex(e => e.AnalysisId, "analysis_idx");

            entity.HasIndex(e => e.CanonicalTranslationId, "canonical_translation_idx")
                .IsUnique();

            entity.HasIndex(e => e.GeneId, "gene_index");

            entity.HasIndex(e => new { e.SeqRegionId, e.SeqRegionStart }, "seq_region_idx");

            entity.HasIndex(e => new { e.StableId, e.Version }, "stable_id_idx");

            entity.HasIndex(e => e.DisplayXrefId, "xref_id_index");

            entity.Property(e => e.TranscriptId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("transcript_id");

            entity.Property(e => e.AnalysisId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("analysis_id");

            entity.Property(e => e.Biotype)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("biotype");

            entity.Property(e => e.CanonicalTranslationId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("canonical_translation_id");

            entity.Property(e => e.CreatedDate).HasColumnName("created_date");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.DisplayXrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("display_xref_id");

            entity.Property(e => e.GeneId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("gene_id");

            entity.Property(e => e.IsCurrent)
                .IsRequired()
                .HasColumnName("is_current")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");

            entity.Property(e => e.SeqRegionEnd)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_end");

            entity.Property(e => e.SeqRegionId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_id");

            entity.Property(e => e.SeqRegionStart)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("seq_region_start");

            entity.Property(e => e.SeqRegionStrand)
                //.HasColumnType("tinyint(2)")
                .HasColumnName("seq_region_strand");

            entity.Property(e => e.Source)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("source")
                .HasDefaultValueSql("'ensembl'");

            entity.Property(e => e.StableId)
                .HasMaxLength(128)
                .HasColumnName("stable_id");

            entity.Property(e => e.Version)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("version");

            entity.HasOne(e => e.Gene)
                  .WithMany(e => e.Transcripts)
                  .HasForeignKey(e => e.GeneId);

            entity.HasOne(e => e.SeqRegion)
                 .WithMany()
                 .HasForeignKey(e => e.SeqRegionId);

            entity.HasOne(e => e.Xref)
                  .WithOne()
                  .HasForeignKey<Transcript>(e => e.DisplayXrefId);

            entity.HasOne(e => e.Translation)
                  .WithOne()
                  .HasForeignKey<Transcript>(e => e.CanonicalTranslationId);

        });

        modelBuilder.Entity<TranscriptAttrib>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("transcript_attrib");

            entity.HasIndex(e => new { e.TranscriptId, e.AttribTypeId, e.Value }, "transcript_attribx")
                .IsUnique();

            entity.HasIndex(e => e.TranscriptId, "transcript_idx");

            entity.HasIndex(e => new { e.AttribTypeId, e.Value }, "type_val_idx");

            entity.HasIndex(e => e.Value, "val_only_idx");

            entity.Property(e => e.AttribTypeId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("attrib_type_id");

            entity.Property(e => e.TranscriptId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("transcript_id");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity.ToTable("translation");

            entity.HasIndex(e => new { e.StableId, e.Version }, "stable_id_idx");

            entity.HasIndex(e => e.TranscriptId, "transcript_idx");

            entity.Property(e => e.TranslationId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("translation_id");

            entity.Property(e => e.CreatedDate).HasColumnName("created_date");

            entity.Property(e => e.EndExonId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("end_exon_id");

            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");

            entity.Property(e => e.SeqEnd)
                .HasColumnType("int(10)")
                .HasColumnName("seq_end");

            entity.Property(e => e.SeqStart)
                .HasColumnType("int(10)")
                .HasColumnName("seq_start");

            entity.Property(e => e.StableId)
                .HasMaxLength(128)
                .HasColumnName("stable_id");

            entity.Property(e => e.StartExonId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("start_exon_id");

            entity.Property(e => e.TranscriptId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("transcript_id");

            entity.Property(e => e.Version)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("version");

            entity.HasOne(e => e.Transcript)
                  .WithMany(e => e.Translations)
                  .HasForeignKey(e => e.TranscriptId);

            entity.HasOne(e => e.StartExon)
                  .WithMany()
                  .HasForeignKey(e => e.StartExonId);

            entity.HasOne(e => e.EndExon)
                  .WithMany()
                  .HasForeignKey(e => e.EndExonId);
        });

        modelBuilder.Entity<TranslationAttrib>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("translation_attrib");

            entity.HasIndex(e => new { e.TranslationId, e.AttribTypeId, e.Value }, "translation_attribx")
                .IsUnique();

            entity.HasIndex(e => e.TranslationId, "translation_idx");

            entity.HasIndex(e => new { e.AttribTypeId, e.Value }, "type_val_idx");

            entity.HasIndex(e => e.Value, "val_only_idx");

            entity.Property(e => e.AttribTypeId)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("attrib_type_id");

            entity.Property(e => e.TranslationId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("translation_id");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Xref>(entity =>
        {
            entity.ToTable("xref");

            entity.HasIndex(e => e.DisplayLabel, "display_index");

            entity.HasIndex(e => new { e.DbprimaryAcc, e.ExternalDbId, e.InfoType, e.InfoText, e.Version }, "id_index")
                .IsUnique();

            entity.HasIndex(e => e.InfoType, "info_type_idx");

            entity.Property(e => e.XrefId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("xref_id");

            entity.Property(e => e.DbprimaryAcc)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("dbprimary_acc");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.DisplayLabel)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("display_label");

            entity.Property(e => e.ExternalDbId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("external_db_id");

            entity.Property(e => e.InfoText)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("info_text")
                .HasDefaultValueSql("''");

            entity.Property(e => e.InfoType)
                .IsRequired()
                .HasColumnType("enum('NONE','PROJECTION','MISC','DEPENDENT','DIRECT','SEQUENCE_MATCH','INFERRED_PAIR','PROBE','UNMAPPED','COORDINATE_OVERLAP','CHECKSUM')")
                .HasColumnName("info_type")
                .HasDefaultValueSql("'NONE'");

            entity.Property(e => e.Version)
                .HasMaxLength(10)
                .HasColumnName("version");

            entity.HasOne(e => e.ExternalDb)
                  .WithMany()
                  .HasForeignKey(e => e.ExternalDbId);
        });
    }
}

