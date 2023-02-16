using System;

namespace Ensembl.Data.Services.Helpers
{
	internal static class IdentifierHelper
	{
		internal static (string Id, int? Version) Extract(string id)
		{
            var blocks = id.Split('.');

			var identifier = blocks[0];

			if (blocks.Length == 1)
			{
				return (identifier, null);
			}
			else
			{
				var version = int.Parse(blocks[1]);

				return (identifier, version);
			}
        }
	}
}

