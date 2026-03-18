## Animate Data Structures

Standalone HTML visualizations for data structure and algorithm problems. Open any `.html` file (for example under `Matrix/`, `Array/`) directly in a browser.

`problems.json` at the repo root is a generated index created by the `IndexGenerator.csproj` console app.

Regenerate it locally:

```bash
dotnet run --project IndexGenerator.csproj --configuration Release
```

GitHub Actions (`.github/workflows/generate-index.yml`) runs the same command on pushes to `main` and commits updated `problems.json` if it changes.

