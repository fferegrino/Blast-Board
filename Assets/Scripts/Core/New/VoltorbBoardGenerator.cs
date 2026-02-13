public class VoltorbBoardGenerator
{
    private System.Random rng = new System.Random();

    public VoltorbBoard GenerateBoard(int boardSize, VoltorbDifficulty diff, int maxTries = 1000)
    {
        for (int attempt = 0; attempt < maxTries; attempt++)
        {
            var board = new VoltorbBoard(boardSize);

            // 1. Set all to 1
            for (int i = 0; i < board.Tiles.Length; i++)
                board.Tiles[i].Type = TileType.One;

            // 2. Place Voltorbs
            PlaceRandomTiles(board, TileType.Voltorb, diff.Voltorbs);

            // 3. Place 2s
            PlaceRandomTiles(board, TileType.Two, diff.Twos);

            // 4. Place 3s
            PlaceRandomTiles(board, TileType.Three, diff.Threes);

            // 5. Validate constraints (free high tiles)
            if (IsBoardValid(board, diff))
            {
                ComputeLineHints(board);
                return board;
            }
        }

        // Fallback: return last attempt even if invalid (like HGSS does after many tries)[web:2]
        var fallback = new VoltorbBoard(boardSize);
        ComputeLineHints(fallback);
        return fallback;
    }

    private void PlaceRandomTiles(VoltorbBoard board, TileType type, int count)
    {
        int placed = 0;
        int failures = 0;
        int maxFailures = 100; // similar spirit to original[web:2]
        int total = board.Tiles.Length;

        while (placed < count && failures < maxFailures)
        {
            int idx = rng.Next(0, total);
            if (board.Tiles[idx].Type == TileType.One)
            {
                board.Tiles[idx].Type = type;
                placed++;
            }
            else
            {
                failures++;
            }
        }
    }

    private bool IsBoardValid(VoltorbBoard board, VoltorbDifficulty diff)
    {
        int size = board.Size;
        int freeHighTotal = 0;

        // Precompute Voltorb counts per row/col
        int[] rowVolts = new int[size];
        int[] colVolts = new int[size];
        int[] rowHigh = new int[size];
        int[] colHigh = new int[size];

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var t = board.Get(r, c).Type;
                bool isHigh = (t == TileType.Two || t == TileType.Three);

                if (t == TileType.Voltorb)
                {
                    rowVolts[r]++;
                    colVolts[c]++;
                }
                else if (isHigh)
                {
                    rowHigh[r]++;
                    colHigh[c]++;
                }
            }
        }

        // Count "free" high tiles: high tiles in lines with 0 Voltorbs
        for (int r = 0; r < size; r++)
        {
            if (rowVolts[r] == 0)
                freeHighTotal += rowHigh[r];
            if (rowHigh[r] > 0 && rowVolts[r] == 0 && rowHigh[r] > diff.MaxFreeHighLine)
                return false;
        }

        for (int c = 0; c < size; c++)
        {
            if (colVolts[c] == 0)
                freeHighTotal += colHigh[c];
            if (colHigh[c] > 0 && colVolts[c] == 0 && colHigh[c] > diff.MaxFreeHighLine)
                return false;
        }

        if (freeHighTotal > diff.MaxFreeHighTotal)
            return false;

        return true;
    }

    private void ComputeLineHints(VoltorbBoard board)
    {
        int size = board.Size;

        for (int i = 0; i < size; i++)
        {
            board.RowSum[i] = 0;
            board.RowVoltorbs[i] = 0;
            board.ColSum[i] = 0;
            board.ColVoltorbs[i] = 0;
        }

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var t = board.Get(r, c).Type;
                if (t == TileType.Voltorb)
                {
                    board.RowVoltorbs[r]++;
                    board.ColVoltorbs[c]++;
                }
                else
                {
                    int val = (int)t; // 1, 2, or 3
                    board.RowSum[r] += val;
                    board.ColSum[c] += val;
                }
            }
        }
    }
}
