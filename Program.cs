using System.Text.Json;

var rootPath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
var manifestPath = Path.Combine(rootPath, "problems.json");

try
{
    var categories = GetCategories(rootPath);
    var manifestCategories = new List<ManifestCategory>();

    foreach (var categoryDir in categories)
    {
        var htmlFiles = Directory.EnumerateFiles(categoryDir, "*.html", SearchOption.TopDirectoryOnly)
            .Where(f => !string.Equals(Path.GetFileName(f), "index.html", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!htmlFiles.Any())
            continue;

        var categoryName = Path.GetFileName(categoryDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var problems = htmlFiles
            .Select(file =>
            {
                var relativePath = GetRelativePath(rootPath, file).Replace(Path.DirectorySeparatorChar, '/');
                var baseName = Path.GetFileNameWithoutExtension(file);
                var titleFallback = MakeTitleFromFileName(baseName);

                var metadataPath = Path.Combine(Path.GetDirectoryName(file)!, baseName + ".json");
                ProblemMetadata? metadata = null;
                if (File.Exists(metadataPath))
                {
                    try
                    {
                        var json = File.ReadAllText(metadataPath);
                        metadata = JsonSerializer.Deserialize<ProblemMetadata>(json);
                    }
                    catch
                    {
                        // Ignore malformed metadata; fall back to basic info.
                    }
                }

                var title = metadata?.ProblemName ?? titleFallback;

                return new ManifestProblem(
                    Title: title,
                    Path: relativePath,
                    ProblemUri: metadata?.ProblemUri,
                    Difficulty: metadata?.Difficulty,
                    Topics: metadata?.Topics
                );
            })
            .ToList();

        manifestCategories.Add(new ManifestCategory(categoryName, problems));
    }

    var manifest = new ManifestRoot(DateTime.UtcNow, manifestCategories);
    var jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    var newContent = JsonSerializer.Serialize(manifest, jsonOptions);
    var existing = File.Exists(manifestPath) ? File.ReadAllText(manifestPath) : string.Empty;

    if (!string.Equals(existing, newContent, StringComparison.Ordinal))
    {
        File.WriteAllText(manifestPath, newContent);
        Console.WriteLine($"problems.json updated at: {manifestPath}");
    }
    else
    {
        Console.WriteLine("problems.json is already up to date.");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("Error while generating problems.json:");
    Console.Error.WriteLine(ex);
    return 1;
}

return 0;

static IEnumerable<string> GetCategories(string rootPath)
{
    var topDirectories = Directory.EnumerateDirectories(rootPath)
        .Where(d => !IsHidden(Path.GetFileName(d)))
        .ToList();

    var defaultCategories = new[]
    {
        "Array",
        "Matrix",
        "Tree",
        "Graph",
        "String",
        "DP",
        "Greedy",
        "Stack",
        "Queue"
    };

    // Prefer known category names if they exist, otherwise fall back to any directory containing html files.
    var existingDefaults = topDirectories
        .Where(d => defaultCategories.Contains(Path.GetFileName(d), StringComparer.OrdinalIgnoreCase))
        .ToList();

    var dynamicDirs = topDirectories
        .Except(existingDefaults)
        .Where(d => Directory.EnumerateFiles(d, "*.html", SearchOption.TopDirectoryOnly).Any())
        .ToList();

    return existingDefaults.Concat(dynamicDirs).OrderBy(d => d, StringComparer.OrdinalIgnoreCase);
}

static bool IsHidden(string? name)
{
    if (string.IsNullOrEmpty(name)) return false;
    return name.StartsWith(".", StringComparison.Ordinal);
}

static string MakeTitleFromFileName(string fileNameWithoutExtension)
{
    var parts = fileNameWithoutExtension
        .Replace('_', ' ')
        .Replace('-', ' ')
        .Split(' ', StringSplitOptions.RemoveEmptyEntries);

    var words = parts
        .Select(word =>
        {
            if (word.Length == 0) return word;
            if (word.Length == 1) return word.ToUpperInvariant();
            return char.ToUpperInvariant(word[0]) + word[1..];
        });

    return string.Join(' ', words);
}

static string GetRelativePath(string rootPath, string filePath)
{
    var rootUri = new Uri(AppendDirectorySeparatorChar(rootPath));
    var fileUri = new Uri(filePath);
    var relativeUri = rootUri.MakeRelativeUri(fileUri);
    var relative = Uri.UnescapeDataString(relativeUri.ToString());
    return relative.Replace('/', Path.DirectorySeparatorChar);
}

static string AppendDirectorySeparatorChar(string path)
{
    if (!path.EndsWith(Path.DirectorySeparatorChar) && !path.EndsWith(Path.AltDirectorySeparatorChar))
    {
        return path + Path.DirectorySeparatorChar;
    }

    return path;
}

file sealed record ManifestRoot(DateTime GeneratedAt, IReadOnlyList<ManifestCategory> Categories);

file sealed record ManifestCategory(string Name, IReadOnlyList<ManifestProblem> Problems);

file sealed record ManifestProblem(string Title, string Path, string? ProblemUri, string? Difficulty, IReadOnlyList<string>? Topics);

file sealed record ProblemMetadata(
    string? ProblemUri,
    string? ProblemName,
    string? ProblemDescription,
    string? ProblemSolution,
    string? Difficulty,
    IReadOnlyList<string>? Topics
);
