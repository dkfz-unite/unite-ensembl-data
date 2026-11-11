namespace Ensembl.Data.Helpers;

public static class NucleotideSequence
{
    public static string ReverseComplement(string sequence)
    {
        return string.Create(sequence.Length, sequence, (result, source) =>
        {
            for (var index = 0; index < result.Length; index++)
                result[index] = Complement(source[source.Length - index - 1]);
        });
    }

    private static char Complement(char nucleotide)
    {
        return nucleotide switch
        {
            'A' => 'T',
            'C' => 'G',
            'G' => 'C',
            'T' => 'A',
            'R' => 'Y', 'Y' => 'R', 'S' => 'S', 'W' => 'W',
            'K' => 'M', 'M' => 'K', 'B' => 'V', 'V' => 'B', 'D' => 'H', 'H' => 'D',
            _ => 'N'
        };
    }
}
