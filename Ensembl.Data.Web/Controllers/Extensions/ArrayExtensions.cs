namespace Ensembl.Data.Web.Controllers.Extensions;

internal static class ArrayExtensions
{
    public static IEnumerable<string> FilteredDistinct(this IEnumerable<string> source)
    {
        return source
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct();
    }
}
