using System;
using System.Linq.Expressions;
using Ensembl.Data.Models;
using Ensembl.Data.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services
{
    public class ProteinSearchService
    {
        private readonly EnsemblDbContext _dbContext;


        public ProteinSearchService(EnsemblDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Finds protein by Ensembl stable identifier.
        /// </summary>
        /// <param name="id">Stable identifier (with or without version)</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Found protein.</returns>
        public Protein Find(string id, bool expand = false)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(id));

            var predicate = GetIdPredicate(id);

            return Find(predicate, expand) ?? Find(IdentifierHelper.Extract(id).Id, expand);
        }

        /// <summary>
        /// Finds proteins by their stable identifiers.
        /// </summary>
        /// <param name="ids">Stable identifiers list (with or without versions)</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Array of found proteins.</returns>
        public Protein[] Find(IEnumerable<string> ids, bool expand = false)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(ids));

            var proteins = ids.Distinct().Select(id => Find(id, expand));

            return proteins.Where(protein => protein != null).DistinctBy(protein => new { protein.Id, protein.Version }).ToArray();
        }

        internal Protein Find(int id, bool expand = false)
        {
            Expression<Func<Entities.Translation, bool>> predicate = (entity) => entity.TranslationId == id;

            return Find(predicate, expand);
        }

        private Protein Find(Expression<Func<Entities.Translation, bool>> predicate, bool expand = false)
        {
            var entity = _dbContext.Translations
                .Include(e => e.StartExon)
                .Include(e => e.EndExon)
                .FirstOrDefault(predicate);

            if (entity != null)
            {
                var protein = new Protein(entity);

                protein.Length = GetProteinLength(entity, protein.Start, protein.End);

                if (expand)
                {
                    protein.Features = GetProteinFeatures(entity);
                }

                return protein;
            }
            else
            {
                return null;
            }
            
        }


        /// <summary>
        /// Retrieves number of amino acids in a protein.
        /// </summary>
        /// <param name="entity">Translation</param>
        /// <param name="start">Genomic start of the translation</param>
        /// <param name="end">Genomic end of the translation</param>
        /// <returns>Protein length in amino acids.</returns>
        private int GetProteinLength(Entities.Translation entity, int start, int end)
        {
            var length = 0;

            var strand = entity.StartExon.SeqRegionStrand;

            var exons = _dbContext.ExonTranscripts
                    .Include(e => e.Exon)
                    .Where(e => e.TranscriptId == entity.TranscriptId)
                    .Where(e => e.Exon.SeqRegionStart > start)
                    .Where(e => e.Exon.SeqRegionEnd < end)
                    .Select(e => e.Exon);

            if (strand == 1)
            {
                length += entity.StartExon.SeqRegionEnd - start;
                length += end - entity.EndExon.SeqRegionStart;
                length += exons.Sum(e => e.SeqRegionEnd - e.SeqRegionStart + 1);
            }
            else
            {
                length += end - entity.StartExon.SeqRegionStart;
                length += entity.EndExon.SeqRegionEnd - start;
                length += exons.Sum(e => e.SeqRegionEnd - e.SeqRegionStart + 1);
            }

            return length / 3;
        }

        /// <summary>
        /// Retrieves pfam features of a protein.
        /// </summary>
        /// <param name="entity">Translation</param>
        /// <returns>Array of pfam protein features.</returns>
        private ProteinFeature[] GetProteinFeatures(Entities.Translation entity)
        {
            return _dbContext.ProteinFeatures
                .Where(e => e.TranslationId == entity.TranslationId)
                .Where(e => e.HitName.StartsWith("PF"))
                .Select(e => new ProteinFeature(e))
                .ToArray();
        }

        private static Expression<Func<Entities.Translation, bool>> GetIdPredicate(string id)
        {
            var identifier = IdentifierHelper.Extract(id);

            return identifier.Version.HasValue
                ? (entity) => entity.StableId == identifier.Id && entity.Version == identifier.Version
                : (entity) => entity.StableId == identifier.Id;
        }
    }
}

