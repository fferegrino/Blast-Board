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

    public int[] ColumnSumValues { get; private set; }
    public int[] RowSumValues { get; private set; }
    public int[] ColumnMultValues { get; private set; }
    public int[] RowMultValues { get; private set; }
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

        this.ColumnSumValues = new int[5];
        this.RowSumValues = new int[5];
        this.ColumnMultValues = new int[5];
        this.RowMultValues = new int[5];
        this.ColumnBombs = new int[5];
        this.RowBombs = new int[5];

        for (int i = 0; i < 5; i++)
        {
            int sumPoints = 0;
            int multPoints = 0;
            int bombs = 0;
            for (int j = 0; j < 5; j++)
            {
                sumPoints += board[i, j];
                if (board[i, j] == 0)
                {
                    bombs++;
                }
                else
                {
                    if (multPoints == 0)
                    {
                        multPoints = 1;
                    }
                    multPoints *= board[i, j];
                }
            }
            RowSumValues[i] = sumPoints;
            RowMultValues[i] = multPoints;
            RowBombs[i] = bombs;
        }

        for (int j = 0; j < 5; j++)
        {
            int sumPoints = 0;
            int multPoints = 0;
            int bombs = 0;
            for (int i = 0; i < 5; i++)
            {
                sumPoints += board[i, j];
                if (board[i, j] == 0)
                {
                    bombs++;
                }
                else
                {
                    if (multPoints == 0)
                    {
                        multPoints = 1;
                    }
                    multPoints *= board[i, j];
                }
            }
            ColumnSumValues[j] = sumPoints;
            ColumnMultValues[j] = multPoints;
            ColumnBombs[j] = bombs;
        }
    }

    public int this[int row, int col]
    {
        get => board[row, col];
    }
}