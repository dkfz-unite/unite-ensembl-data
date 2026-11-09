namespace Ensembl.Data.Helpers;

internal static class PeptideSequence
{
    // NCBI tables 1 and 2, codons in T/C/A/G order for each position.
    private const string Bases = "TCAG";
    private const string Standard = "FFLLSSSSYY**CC*WLLLLPPPPHHQQRRRRIIIMTTTTNNKKSSRRVVVVAAAADDEEGGGG";
    private const string Mitochondrial = "FFLLSSSSYY**CCWWLLLLPPPPHHQQRRRRIIMMTTTTNNKKSS**VVVVAAAADDEEGGGG";


    public static string Translate(string coding, bool mitochondrial)
    {
        var table = mitochondrial ? Mitochondrial : Standard;
        var peptide = new char[coding.Length / 3];
        for (var index = 0; index < peptide.Length; index++)
        {
            var codon = coding.AsSpan(index * 3, 3);
            peptide[index] = index == 0 && IsStart(codon, mitochondrial) ? 'M' : TranslateCodon(codon, table);
        }

        var length = peptide.Length;
        if (length > 0 && peptide[^1] == '*')
            length--;

        return new string(peptide, 0, length);
    }


    private static bool IsStart(ReadOnlySpan<char> codon, bool mitochondrial)
    {
        return mitochondrial
            ? codon is "ATT" or "ATC" or "ATA" or "ATG" or "GTG"
            : codon is "TTG" or "CTG" or "ATG";
    }

    private static char TranslateCodon(ReadOnlySpan<char> codon, string table)
    {
        char? result = null;
        foreach (var first in Options(codon[0]))
        foreach (var second in Options(codon[1]))
        foreach (var third in Options(codon[2]))
        {
            var aminoAcid = table[Bases.IndexOf(first) * 16 + Bases.IndexOf(second) * 4 + Bases.IndexOf(third)];
            if (result != null && result != aminoAcid)
                return 'X';

            result = aminoAcid;
        }

        return result ?? 'X';
    }

    private static string Options(char nucleotide)
    {
        return nucleotide switch
        {
            'A' => "A", 'C' => "C", 'G' => "G", 'T' => "T",
            'R' => "AG", 'Y' => "CT", 'S' => "CG", 'W' => "AT", 'K' => "GT", 'M' => "AC",
            'B' => "CGT", 'D' => "AGT", 'H' => "ACT", 'V' => "ACG", 'N' => "ACGT",
            _ => ""
        };
    }
}
