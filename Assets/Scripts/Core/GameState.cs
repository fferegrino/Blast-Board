using System;
using System.Collections.Generic;

public enum CellState
{
    Hidden = 0,
    Revealed = 1,
    Targeted = 2,
}

public enum GameOutcome
{
    InProgress = 0,
    Won = 1,
    Lost = 2,
}

[Flags]
public enum CellMarks
{
    None  = 0b_0000_0000,  // 0
    Mark0 = 0b_0000_0001,  // 1
    Mark1 = 0b_0000_0010,  // 2
    Mark2 = 0b_0000_0100,  // 4
    Mark3 = 0b_0000_1000,  // 8
}

public class GameState
{
    public const int BoardSize = 5;

    private readonly RawBoard board;
    private readonly CellState[,] cellStates;
    private readonly CellMarks[,] cellMarks;
    private readonly List<Tuple<int, int>> zeroLocations;

    public int[] ColumnSumValues { get; }
    public int[] RowSumValues { get; }
    public int[] ColumnMultValues { get; }
    public int[] RowMultValues { get; }
    public int[] ColumnBombs { get; }
    public int[] RowBombs { get; }

    public int PointsToWin { get; }
    public int CurrentPoints { get; private set; }
    public GameOutcome Outcome { get; private set; }
    public IReadOnlyList<Tuple<int, int>> ZeroLocations => zeroLocations;

    public GameState(RawBoard board)
    {
        this.board = board ?? throw new ArgumentNullException(nameof(board));
        cellStates = new CellState[BoardSize, BoardSize];
        cellMarks = new CellMarks[BoardSize, BoardSize];
        zeroLocations = new List<Tuple<int, int>>();

        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                cellStates[i, j] = CellState.Hidden;
                cellMarks[i, j] = CellMarks.None;
            }
        }

        PointsToWin = ComputePointsToWin();
        CurrentPoints = 0;
        Outcome = GameOutcome.InProgress;

        ComputeRowHints(out int[] rowSums, out int[] rowMults, out int[] rowBombs);
        ComputeColumnHints(out int[] colSums, out int[] colMults, out int[] colBombs);

        RowSumValues = rowSums;
        RowMultValues = rowMults;
        RowBombs = rowBombs;
        ColumnSumValues = colSums;
        ColumnMultValues = colMults;
        ColumnBombs = colBombs;
    }

    private int ComputePointsToWin()
    {
        int product = 1;
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                int value = board[i, j];
                if (value != 0)
                    product *= value;
                else
                    zeroLocations.Add(Tuple.Create(i, j));
            }
        }
        return product;
    }

    private void ComputeRowHints(out int[] sums, out int[] mults, out int[] bombs)
    {
        sums = new int[BoardSize];
        mults = new int[BoardSize];
        bombs = new int[BoardSize];

        for (int row = 0; row < BoardSize; row++)
        {
            int sum = 0, mult = 0, bombCount = 0;
            for (int col = 0; col < BoardSize; col++)
            {
                int value = board[row, col];
                if (value == 0)
                {
                    bombCount++;
                }
                else
                {
                    sum += value;
                    mult = mult == 0 ? value : mult * value;
                }
            }
            sums[row] = sum;
            mults[row] = mult;
            bombs[row] = bombCount;
        }
    }

    private void ComputeColumnHints(out int[] sums, out int[] mults, out int[] bombs)
    {
        sums = new int[BoardSize];
        mults = new int[BoardSize];
        bombs = new int[BoardSize];

        for (int col = 0; col < BoardSize; col++)
        {
            int sum = 0, mult = 0, bombCount = 0;
            for (int row = 0; row < BoardSize; row++)
            {
                int value = board[row, col];
                if (value == 0)
                {
                    bombCount++;
                }
                else
                {
                    sum += value;
                    mult = mult == 0 ? value : mult * value;
                }
            }
            sums[col] = sum;
            mults[col] = mult;
            bombs[col] = bombCount;
        }
    }

    /// <summary>
    /// Tries to reveal the cell at (row, col). Fails (returns false) if the game is over or the cell is already revealed.
    /// When a cell is revealed: if it's a bomb (0) the game is lost; otherwise points are multiplied and win is checked.
    /// </summary>
    /// <returns>True if the cell was revealed; false if nothing changed (already revealed or game over).</returns>
    public bool TryRevealCell(int row, int col)
    {
        if (Outcome != GameOutcome.InProgress)
            return false;

        if (cellStates[row, col] == CellState.Revealed)
            return false;

        cellStates[row, col] = CellState.Revealed;

        int value = board[row, col];
        if (value == 0)
        {
            Outcome = GameOutcome.Lost;
            return true;
        }

        CurrentPoints = CurrentPoints == 0 ? value : CurrentPoints * value;
        if (CurrentPoints >= PointsToWin)
            Outcome = GameOutcome.Won;

        return true;
    }

    public CellState GetCellState(int row, int col) => cellStates[row, col];

    public CellMarks GetCellMark(int row, int col) => cellMarks[row, col];

    /// <summary>
    /// Returns true if the cell at (row, col) has the given mark(s) set.
    /// For a combination of flags, returns true only when all specified marks are set.
    /// For <see cref="CellMarks.None"/>, returns true when the cell has no marks.
    /// </summary>
    public bool HasCellMark(int row, int col, CellMarks mark)
    {
        var current = cellMarks[row, col];
        if (mark == CellMarks.None)
            return current == CellMarks.None;
        return (current & mark) == mark;
    }

    /// <summary>
    /// Sets the mark for the cell at (row, col). Use <see cref="CellMarks.None"/> to clear all marks.
    /// <paramref name="mark"/> can be a combination of flags (e.g. <c>CellMarks.Mark0 | CellMarks.Mark1</c>).
    /// </summary>
    public void SetCellMark(int row, int col, CellMarks mark)
    {
        cellMarks[row, col] = mark;
    }

    /// <summary>
    /// Adds the given mark(s) to the cell at (row, col). Combines with any existing marks.
    /// </summary>
    public void AddCellMark(int row, int col, CellMarks mark)
    {
        cellMarks[row, col] |= mark;
    }

    /// <summary>
    /// Removes the given mark(s) from the cell at (row, col). Other marks are unchanged.
    /// </summary>
    public void RemoveCellMark(int row, int col, CellMarks mark)
    {
        cellMarks[row, col] &= ~mark;
    }

    public int this[int row, int col] => board[row, col];

    public int GetRowPoints(int row) => RowSumValues[row];

    public int GetColumnPoints(int column) => ColumnSumValues[column];
}
