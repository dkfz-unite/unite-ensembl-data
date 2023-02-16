using System;
using System.Linq.Expressions;
using Ensembl.Data.Models;
using Ensembl.Data.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services
{
	public class GeneSearchService
	{
		private readonly EnsemblDbContext _dbContext;
		private readonly TranscriptSearchService _transcriptRepository;


		public GeneSearchService(EnsemblDbContext dbContext)
		{
			_dbContext = dbContext;
			_transcriptRepository = new TranscriptSearchService(dbContext);
        }

        /// <summary>
        /// Finds gene by Ensembl stable identifier.
        /// </summary>
        /// <param name="id">Stable identifier (with or without version)</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Found gene.</returns>
        public Gene Find(string id, bool expand = false)
		{
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(nameof(id));
            }

            var predicate = GetIdPredicate(id);

			return Find(predicate, expand) ?? Find(IdentifierHelper.Extract(id).Id, expand);
		}

        /// <summary>
        /// Finds genes by their Ensembl stable identifiers.
        /// </summary>
        /// <param name="ids">Stable identifiers list (with or without versions)</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Array of found genes.</returns>
        public Gene[] Find(IEnumerable<string> ids, bool expand = false)
		{
            if (ids == null)
            {
                throw new ArgumentException(nameof(ids));
            }

            var genes = ids.Distinct().Select(id => Find(id, expand));

			return genes.Where(gene => gene != null).DistinctBy(gene => new { gene.Id, gene.Version }).ToArray();
        }

		private Gene Find(Expression<Func<Entities.Gene, bool>> predicate, bool expand = false)
		{
            var entity = _dbContext.Genes
                .Include(e => e.SeqRegion)
                .Include(e => e.Xref)
                .FirstOrDefault(predicate);

			if (entity != null)
			{
                var gene = new Gene(entity);

                gene.ExonicLength = GetExonicLength(entity);

                if (expand)
                {
                    gene.Transcript = GetTranscript(entity);
                }

				return gene;
            }
			else
			{
				return null;
			}
        }


		/// <summary>
		/// Retrieves gene canonical transcript.
		/// </summary>
		/// <param name="entity">Gene</param>
		/// <returns>Gene canonical transcript.</returns>
		private Transcript GetTranscript(Entities.Gene entity)
		{
			var id = entity.CanonicalTranscriptId;

			return _transcriptRepository.Find(id, true);
		}

		/// <summary>
		/// Retrieves exonic length of gene canonical transcript (SUM of length of all transcript exons).
		/// </summary>
		/// <param name="entity">Gene</param>
		/// <returns>Gene canonical transcript exonic length.</returns>
		private int GetExonicLength(Entities.Gene entity)
		{
			return _dbContext.ExonTranscripts
				.Include(e => e.Exon)
				.Where(e => e.TranscriptId == entity.CanonicalTranscriptId)
				.Sum(e => e.Exon.SeqRegionEnd - e.Exon.SeqRegionStart + 1);
		}

		private static Expression<Func<Entities.Gene, bool>> GetIdPredicate(string id)
		{
			var identifier = IdentifierHelper.Extract(id);

			return identifier.Version.HasValue
				? (entity) => entity.StableId == identifier.Id && entity.Version == identifier.Version
				: (entity) => entity.StableId == identifier.Id;
		}
    }
}

