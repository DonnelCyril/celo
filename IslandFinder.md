    ```csharp
    public class Program
    {

        //wei.pan@celohealth.com
        //CreateHostBuilder(args).Build().Run();

        public static void Main(string[] args)
        {

            var mat = new[,]
            {
                {1, 1, 0, 0, 0},
                {0, 1, 0, 0, 1},
                {1, 0, 0, 1, 1},
                {1, 1, 0, 0, 0},
                {0, 1, 0, 0, 1},
                {1, 0, 0, 1, 1},
            };
            var count = GetIslandCounter(mat);
            Console.WriteLine($"Islands: {count}");
        }
        static int GetIslandCounter(int[,] matrix)
        {
            var knownOneIndexes = new HashSet<(int, int)>();
            var islandCounter = 0;
            if (matrix == null) return 0;
            for (var row = 0; row <= matrix.GetUpperBound(0); row++)
            {
                for (var column = 0; column <= matrix.GetUpperBound(1); column++)
                {
                    var item = matrix[row, column];
                    if (item != 1) continue;
                    var surroundingOnes = GetAllSurroundingOneIndexes((row, column), matrix);
                    islandCounter += surroundingOnes.Any(knownOneIndexes.Contains) ? 0 : 1;
                    knownOneIndexes.UnionWith(surroundingOnes);
                }
            }
            return islandCounter;
        }

        static HashSet<(int, int)> GetAllSurroundingOneIndexes((int, int) currentIndex, int[,] matrix, HashSet<(int, int)> knowsOneIndexes = default)
        {
            knowsOneIndexes ??= new HashSet<(int, int)>();
            var surroundingOneIndexes = GetImmediateSurroundingOneIndexes(currentIndex, matrix);
            if (!surroundingOneIndexes.Any()) return knowsOneIndexes;
            foreach (var index in surroundingOneIndexes.Where(i => !knowsOneIndexes.Contains(i)))
            {
                knowsOneIndexes.Add(index);
                knowsOneIndexes.UnionWith(GetAllSurroundingOneIndexes(index, matrix, knowsOneIndexes));
            }
            return knowsOneIndexes;
        }

        static List<(int, int)> GetImmediateSurroundingOneIndexes((int, int) currentIndex, int[,] matrix)
        {
            var matrixBounds = (matrix.GetUpperBound(0), matrix.GetUpperBound(1));
            var listOfIndexesToBeChecked = GetIndexesToBeCheckedFromPosition(currentIndex);
            return listOfIndexesToBeChecked.Where(i => IsValidIndex(matrixBounds, i) && matrix[i.row, i.column] == 1).Select(i => i).ToList();
        }

        static IEnumerable<(int row, int column)> GetIndexesToBeCheckedFromPosition((int row, int column) index) =>
            Positions.Select(position => (position.rowPosition, position.columnPosition) switch
            {
                ("Top", "Left") => (index.row - 1, index.column - 1),
                ("Top", "Right") => (index.row - 1, index.column + 1),
                ("Middle", "Left") => (index.row, index.column - 1),
                ("Middle", "Right") => (index.row, index.column + 1),
                ("Bottom", "Left") => (index.row + 1, index.column - 1),
                ("Bottom", "Right") => (index.row + 1, index.column + 1),
                _ => (-1, -1)
            });

        static readonly IEnumerable<(string rowPosition, string columnPosition)> Positions = new[] { "Top", "Middle", "Bottom" }.SelectMany(_ => new[] { "Left", "Right" }, (rowPosition, colPosition) => (rowPosition, colPosition));

        static bool IsValidIndex((int maxRowIndex, int maxColumnIndex) bounds, (int row, int column) index) =>
            index.row >= 0 &&
            index.row < bounds.maxRowIndex &&
            index.column >= 0 &&
            index.column <= bounds.maxColumnIndex;

    }
```
