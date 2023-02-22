using System;
using System.Linq.Expressions;
using Ensembl.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services
{
    public class GeneSearchService
    {
        private int[] _coordinationSystems = { 2, 10004 };
        private string[] _chromosomesNames = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "X", "Y" };
        private readonly EnsemblDbContext _dbContext;
		private readonly TranscriptSearchService _transcriptSearchService;
		private readonly ProteinSearchService _proteinSearchService;


		public GeneSearchService(EnsemblDbContext dbContext)
		{
			_dbContext = dbContext;
			_transcriptSearchService = new TranscriptSearchService(dbContext);
			_proteinSearchService = new ProteinSearchService(dbContext);
        }

        /// <summary>
        /// Finds gene by Ensembl stable identifier.
        /// </summary>
        /// <param name="id">Stable identifier</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Found gene.</returns>
        /// <exception cref="ArgumentException"></exception>
        public Gene Find(string id, bool expand = false)
		{
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(nameof(id));
            }

            var entity = GetQuery().FirstOrDefault(entity => entity.StableId == id);

            return Convert(entity);
        }

        /// <summary>
        /// Finds genes by their Ensembl stable identifiers.
        /// </summary>
        /// <param name="ids">Stable identifiers list</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Array of found genes.</returns>
        /// <exception cref="ArgumentException"></exception>
        public Gene[] Find(IEnumerable<string> ids, bool expand = false)
		{
            if (ids == null)
            {
                throw new ArgumentException(nameof(ids));
            }

            var entities = GetQuery().Where(entity => ids.Contains(entity.StableId)).ToArray();

            return entities.Select(entity => Convert(entity, expand)).ToArray();
        }

        /// <summary>
        /// Finds gene by symbol.
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Found gene.</returns>
        /// <exception cref="ArgumentException"></exception>
        public Gene FindByName(string symbol, bool expand = false)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException(nameof(symbol));
            }

            var entity = GetQuery().FirstOrDefault(entity => entity.Xref.DisplayLabel == symbol);

            return Convert(entity);
        }

        /// <summary>
        /// Finds genes by their symbols.
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <param name="expand">Include child entries</param>
        /// <returns>Array of found genes.</returns>
        /// <exception cref="ArgumentException"></exception>
        public Gene[] FindByName(IEnumerable<string> symbols, bool expand = false)
        {
            if (symbols == null)
            {
                throw new ArgumentException(nameof(symbols));
            }

            var entities = GetQuery().Where(entity => symbols.Contains(entity.Xref.DisplayLabel)).ToArray();

            return entities.Select(entity => Convert(entity, expand)).ToArray();
        }


        private IQueryable<Entities.Gene> GetQuery()
        {
            return _dbContext.Genes
                .Include(e => e.SeqRegion)
                .Include(e => e.Xref)
                .Where(e => _coordinationSystems.Contains(e.SeqRegion.CoordSystemId))
                .Where(e => _chromosomesNames.Contains(e.SeqRegion.Name));
        }

		private Gene Convert(Entities.Gene entity, bool expand = false)
		{
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

        /// <summary>
        /// Retrieves gene canonical transcript.
        /// </summary>
        /// <param name="entity">Gene</param>
        /// <returns>Gene canonical transcript.</returns>
        private Transcript GetTranscript(Entities.Gene entity)
		{
			var id = entity.CanonicalTranscriptId;

			return _transcriptSearchService.Get(id, true);
		}
    }
}

