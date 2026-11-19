using System.Diagnostics;
using System.Globalization;
using Ensembl.Data.Models.Liftover;
using Ensembl.Data.Services.Configuration.Options;

namespace Ensembl.Data.Services;

public class CoordinateLiftoverService
{
    private readonly ILiftoverOptions _options;

    public CoordinateLiftoverService(ILiftoverOptions options)
    {
        _options = options;
    }

    public Task<CoordinateRegion[]> Lift(byte sourceGenome, byte targetGenome, CoordinateRegion[] regions)
    {
        return Lift(sourceGenome, targetGenome, regions, 1.0);
    }

    public async Task<CoordinateRegion[]> Lift(
        byte sourceGenome,
        byte targetGenome,
        CoordinateRegion[] regions,
        double minimumMatch)
    {
        if (regions.Length == 0)
            return [];

        var directory = Directory.CreateTempSubdirectory("ensembl-liftover-");

        try
        {
            var inputPath = Path.Combine(directory.FullName, "input.bed");
            var outputPath = Path.Combine(directory.FullName, "output.bed");
            var unmappedPath = Path.Combine(directory.FullName, "unmapped.bed");
            var chainPath = GetChainPath(sourceGenome, targetGenome);

            await File.WriteAllLinesAsync(inputPath, regions.Select(Format));
            await Run(inputPath, chainPath, outputPath, unmappedPath, minimumMatch);

            return File.ReadLines(outputPath).Select(Parse).ToArray();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    private string GetChainPath(byte sourceGenome, byte targetGenome)
    {
        var file = (sourceGenome, targetGenome) switch
        {
            (37, 38) => "hg19ToHg38.over.chain.gz",
            (38, 37) => "hg38ToHg19.over.chain.gz",
            _ => throw new NotSupportedException($"Liftover from GRCh{sourceGenome} to GRCh{targetGenome} is not supported.")
        };

        var path = Path.Combine(_options.ChainsDirectoryPath, file);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Liftover chain file was not found: {path}", path);

        return path;
    }

    private async Task Run(
        string inputPath,
        string chainPath,
        string outputPath,
        string unmappedPath,
        double minimumMatch)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _options.ExecutablePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add($"-minMatch={minimumMatch.ToString(CultureInfo.InvariantCulture)}");
        startInfo.ArgumentList.Add(inputPath);
        startInfo.ArgumentList.Add(chainPath);
        startInfo.ArgumentList.Add(outputPath);
        startInfo.ArgumentList.Add(unmappedPath);

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"Liftover failed with exit code {process.ExitCode}: {await error}");

        await output;
    }

    private static string Format(CoordinateRegion region)
    {
        var chromosome = region.Chromosome == "MT" ? "chrM" : $"chr{region.Chromosome}";
        var start = region.Start - 1;

        return string.Join('\t', chromosome, start, region.End, region.Id, 0, "+");
    }

    private static CoordinateRegion Parse(string value)
    {
        var fields = value.Split('\t');
        var chromosome = fields[0][3..];
        if (chromosome == "M")
            chromosome = "MT";

        return new CoordinateRegion
        {
            Id = int.Parse(fields[3], CultureInfo.InvariantCulture),
            Chromosome = chromosome,
            Start = int.Parse(fields[1], CultureInfo.InvariantCulture) + 1,
            End = int.Parse(fields[2], CultureInfo.InvariantCulture),
            Strand = fields[5] == "+"
        };
    }
}
