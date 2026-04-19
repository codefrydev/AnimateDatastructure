## Animate Data Structures

Standalone HTML visualizations for data structure and algorithm problems. Open a visualization file directly in a browser (for example `Matrix/<problem-slug>/<problem-slug>.html` under `Array/`, `Matrix/`, etc.).

`problems.json` at the repo root is a generated index created by the `IndexGenerator.csproj` console app.

Regenerate it locally:

```bash
dotnet run --project IndexGenerator.csproj --configuration Release
```

GitHub Actions (`.github/workflows/generate-index.yml`) runs the same command on pushes to `main` and commits updated `problems.json` if it changes.

### Adding a new problem

- **1. Pick a category directory**
  - Use one of the existing folders like `Array`, `Matrix`, `Tree`, etc.
  - If you really need a new category, create a new top-level folder and keep the name short and generic (for example `Heap`, `Bit`, `Math`).

- **2. Create a per-problem folder (LeetCode-style slug)**
  - Take the slug from the LeetCode URL and use it as the **folder name** and as the **base name** for both files.
    - Example: LeetCode URL `https://leetcode.com/problems/count-submatrices-with-top-left-element-and-sum-less-than-k/`
    - Create `Matrix/count-submatrices-with-top-left-element-and-sum-less-than-k/`
    - Add `count-submatrices-with-top-left-element-and-sum-less-than-k.html` and `count-submatrices-with-top-left-element-and-sum-less-than-k.json` inside that folder.
  - The folder name must match the `.html` / `.json` basename so the index generator can find the pair. You can add extra assets in the same folder (images, scripts, data files) and reference them with relative URLs from the HTML file.

- **3. Create the JSON metadata file**
  - The JSON file should describe the problem and is what the index generator reads.
  - Recommended shape:

```json
{
  "ProblemUri": "https://leetcode.com/problems/count-submatrices-with-top-left-element-and-sum-less-than-k/",
  "ProblemName": "Count Submatrices With Top Left Element and Sum Less Than K",
  "ProblemDescription": "Short 1–3 sentence description of the problem.",
  "ProblemSolution": "Optional: link to your solution, notes, or explanation.",
  "Difficulty": "Medium",
  "Topics": ["Matrix", "Prefix Sum"],
  "ProblemNumber": 2841
}
```

  - `ProblemUri`, `ProblemName`, `Difficulty`, `Topics`, and `ProblemNumber` are the fields that will surface in `problems.json`. Description and solution are for humans.

- **4. Create the HTML visualization**
  - Create an `.html` file with the same base name as the JSON file.
  - Build the **best interactive animation you can** for that problem:
    - Visualize the input (arrays, matrices, trees, etc.).
    - Animate how the algorithm progresses step-by-step.
    - Use controls like play/pause, next step, speed sliders, and input editors when it makes sense.
  - Prefer a self-contained visualization in the HTML file (no build step required). Optional co-located assets in the same slug folder are fine.
  - Published URLs use the nested path `Category/<slug>/<slug>.html` (not `Category/<slug>.html`); update external links if you move or rename a problem.

- **5. Regenerate the index**
  - From the repo root, run:

```bash
dotnet run --project IndexGenerator.csproj --configuration Release
```

  - This updates `problems.json` with your new problem so the main `index.html` can link to it.
