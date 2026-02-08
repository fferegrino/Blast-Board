using System.Collections.Generic;
using System.Linq;

using System;
public class RawBoard
{
    private static readonly Random _random = new ();
    private readonly int[,] board;

    public RawBoard(int[,] board) {
        this.board = board;
    }

    public RawBoard(LevelDefinition levelDefinition)
    {
        var cells = levelDefinition.GetCells();
        Shuffle(cells);
        this.board = new int[5, 5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                this.board[i, j] = cells[i * 5 + j];
            }
        }
    }

    public int this[int row, int col]
    {
        get => board[row, col];
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
