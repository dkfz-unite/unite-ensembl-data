using System;
using System.Linq.Expressions;
using Ensembl.Data.Models;
using Ensembl.Data.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services
{
	public class TranscriptSearchService
	{
        private readonly EnsemblDbContext _dbContext;
        private readonly ProteinSearchService _proteinRepository;


        public TranscriptSearchService(EnsemblDbContext dbContext)
        {
            _dbContext = dbContext;
            _proteinRepository = new ProteinSearchService(dbContext);
        }

        /// <summary>
        /// Finds transcript by Ensembl stable identifier.
        /// </summary>
        /// <param name="id">Stable identifier (with or without version)</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Found transcript.</returns>
        public Transcript Find(string id, bool expand = false)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(nameof(id));
            }

            var predicate = GetIdPredicate(id);

            return Find(predicate, expand) ?? Find(IdentifierHelper.Extract(id).Id, expand);
        }

        /// <summary>
        /// Finds transcripts by their Ensembl stable identifiers.
        /// </summary>
        /// <param name="ids">Stable identifiers list (with or without versions)</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Array of found transcripts.</returns>
        public Transcript[] Find(IEnumerable<string> ids, bool expand = false)
        {
            if (ids == null)
            {
                throw new ArgumentException(nameof(ids));
            }

            var transcripts = ids.Distinct().Select(id => Find(id, expand));

            return transcripts.Where(transcript => transcript != null).DistinctBy(transcript => new { transcript.Id, transcript.Version }).ToArray();
        }

        internal Transcript Get(int id, bool expand = false)
        {
            Expression<Func<Entities.Transcript, bool>> predicate = (entity) => entity.TranscriptId == id;

            return Find(predicate, expand);
        }

        private Transcript Find(Expression<Func<Entities.Transcript, bool>> predicate, bool expand = false)
        {
            var entity = _dbContext.Transcripts
                .Include(e => e.Gene)
                .Include(e => e.SeqRegion)
                .Include(e => e.Xref)
                .FirstOrDefault(predicate);

            if (entity != null)
            {
                var transcript = new Transcript(entity);

                transcript.ExonicLength = GetExonicLength(entity);

                if (expand)
                {
                    transcript.Protein = GetProtein(entity);
                }

                return transcript;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves transcript canonical protein.
        /// </summary>
        /// <param name="entity">Transcript</param>
        /// <returns>Transcript canonical protein.</returns>
        private Protein GetProtein(Entities.Transcript entity)
        {
            var id = entity.CanonicalTranslationId;

            return id == null ? null : _proteinRepository.Get(id.Value, true);
        }

        /// <summary>
        /// Retrieves exonic length of a transcript (SUM of lengths of all transcript exons).
        /// </summary>
        /// <param name="entity">Transcript</param>
        /// <returns>Transcript exonic length.</returns>
        private int GetExonicLength(Entities.Transcript entity)
        {
            return _dbContext.ExonTranscripts
                .Include(e => e.Exon)
                .Where(e => e.TranscriptId == entity.TranscriptId)
                .Sum(e => e.Exon.SeqRegionEnd - e.Exon.SeqRegionStart + 1);
        }

        private static Expression<Func<Entities.Transcript, bool>> GetIdPredicate(string id)
        {
            var identifier = IdentifierHelper.Extract(id);

            return identifier.Version.HasValue
                ? (entity) => entity.StableId == identifier.Id && entity.Version == identifier.Version
                : (entity) => entity.StableId == identifier.Id;
        }
    }
}

