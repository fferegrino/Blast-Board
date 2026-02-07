using System.Collections.Generic;
using System;

public enum CellState
{
    Hidden = 0,
    Revealed = 1,
    Targeted = 2,
}

[Flags]
public enum CellMarks {
    None      = 0b_0000_0000,  // 0
    Mark0     = 0b_0000_0001,  // 1
    Mark1     = 0b_0000_0010,  // 2
    Mark2     = 0b_0000_0100,  // 4
    Mark3     = 0b_0000_1000,  // 8
}

public class GameState
{
    private readonly RawBoard board;

    private CellState[,] cellStates;
    private CellMarks[,] cellMarks;

    public int[] ColumnValues { get; private set; }
    public int[] RowValues { get; private set; }

    public int[] ColumnBombs { get; private set; }
    public int[] RowBombs { get; private set; }


    public GameState(RawBoard board)
    {
        this.board = board;
        this.cellStates = new CellState[5, 5];
        this.cellMarks = new CellMarks[5, 5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                this.cellStates[i, j] = CellState.Hidden;
                this.cellMarks[i, j] = CellMarks.None;
            }
        }

        this.ColumnValues = new int[5];
        this.RowValues = new int[5];
        this.ColumnBombs = new int[5];
        this.RowBombs = new int[5];

        for (int i = 0; i < 5; i++)
        {
            int points = 1;
            for (int j = 0; j < 5; j++)
            {
                points += board[i, j];
            }
            RowValues[i] = points;
        }
    }

    public int this[int row, int col]
    {
        get => board[row, col];
    }
}