using NUnit.Framework;

namespace BoardBlast.Tests;

public class GameStateTests
{
    [Test]
    public void Indexer_ReturnsBoardValues()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 2, 3, 0, 1 },
            { 2, 1, 0, 3, 2 },
            { 0, 3, 1, 2, 0 },
            { 3, 0, 2, 1, 3 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        Assert.That(state[0, 0], Is.EqualTo(1));
        Assert.That(state[0, 3], Is.EqualTo(0));
        Assert.That(state[2, 1], Is.EqualTo(3));
        Assert.That(state[4, 4], Is.EqualTo(1));
    }

    // [Test]
    // public void RowValues_AreComputedAsOnePlusRowSum()
    // {
    //     // Row 0: 1+2+3+0+1 = 7 -> 1+7 = 8
    //     // Row 1: 2+1+0+3+2 = 8 -> 1+8 = 9
    //     var board = new RawBoard(new int[,]
    //     {
    //         { 1, 2, 3, 0, 1 },
    //         { 2, 1, 0, 3, 2 },
    //         { 0, 0, 0, 0, 0 },
    //         { 3, 3, 3, 3, 3 },
    //         { 1, 1, 1, 1, 1 }
    //     });
    //     var state = new GameState(board);

    //     Assert.That(state.RowSumValues, Has.Length.EqualTo(5));
    //     Assert.That(state.RowSumValues[0], Is.EqualTo(8));  // 1 + (1+2+3+0+1)
    //     Assert.That(state.RowSumValues[1], Is.EqualTo(9));  // 1 + (2+1+0+3+2)
    //     Assert.That(state.RowSumValues[2], Is.EqualTo(1));  // 1 + 0
    //     Assert.That(state.RowSumValues[3], Is.EqualTo(16)); // 1 + 15
    //     Assert.That(state.RowSumValues[4], Is.EqualTo(6));  // 1 + 5
    // }

    // [Test]
    // public void ColumnValues_AndBombArrays_ExistWithLengthFive()
    // {
    //     var board = new RawBoard(new int[,]
    //     {
    //         { 1, 0, 0, 0, 0 },
    //         { 0, 0, 0, 0, 0 },
    //         { 0, 0, 0, 0, 0 },
    //         { 0, 0, 0, 0, 0 },
    //         { 0, 0, 0, 0, 0 }
    //     });
    //     var state = new GameState(board);

    //     Assert.That(state.ColumnSumValues, Has.Length.EqualTo(5));
    //     Assert.That(state.RowBombs, Has.Length.EqualTo(5));
    //     Assert.That(state.ColumnBombs, Has.Length.EqualTo(5));
    // }

    [Test]
    public void ColumnValuesAreComputedAsExpected()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 0, 0, 0, 1 },
            { 0, 2, 0, 0, 2 },
            { 0, 3, 3, 0, 3 },
            { 0, 0, 3, 0, 3 },
            { 0, 0, 0, 0, 3 }
        });
        var state = new GameState(board);

        int[] expectedColumnSumValues = { 1, 5, 6, 0, 12 };
        int[] expectedColumnMultValues = { 1, 6, 9, 0, 54 };
        int[] expectedColumnBombs = { 4, 3, 3, 5, 0 };

        Assert.That(state.ColumnSumValues, Is.EqualTo(expectedColumnSumValues));
        Assert.That(state.ColumnMultValues, Is.EqualTo(expectedColumnMultValues));
        Assert.That(state.ColumnBombs, Is.EqualTo(expectedColumnBombs));

        int[] expectedRowSumValues = { 2, 4, 9, 6, 3 };
        int[] expectedRowMultValues = { 1, 4, 27, 9, 3 };
        int[] expectedRowBombs = { 3, 3, 2, 3, 4 };

        Assert.That(state.RowSumValues, Is.EqualTo(expectedRowSumValues));
        Assert.That(state.RowMultValues, Is.EqualTo(expectedRowMultValues));
        Assert.That(state.RowBombs, Is.EqualTo(expectedRowBombs));
    }

    [Test]
    public void TryRevealCell_OnBomb_ReturnsTrueAndSetsOutcomeToLost()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 2, 0, 2, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        var revealed = state.TryRevealCell(0, 2);

        Assert.That(revealed, Is.True);
        Assert.That(state.Outcome, Is.EqualTo(GameOutcome.Lost));
    }

    [Test]
    public void TryRevealCell_WhenProductReachesPointsToWin_SetsOutcomeToWon()
    {
        // PointsToWin = 1*2*2*1 * 1s... = 4. Reveal (0,0)=1 then (0,1)=2 then (0,3)=2 -> 1*2*2=4
        var board = new RawBoard(new int[,]
        {
            { 1, 2, 0, 2, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        Assert.That(state.TryRevealCell(0, 0), Is.True);
        Assert.That(state.CurrentPoints, Is.EqualTo(1));
        Assert.That(state.TryRevealCell(0, 1), Is.True);
        Assert.That(state.CurrentPoints, Is.EqualTo(2));
        Assert.That(state.TryRevealCell(0, 3), Is.True);

        Assert.That(state.Outcome, Is.EqualTo(GameOutcome.Won));
        Assert.That(state.CurrentPoints, Is.EqualTo(4));
    }

    [Test]
    public void TryRevealCell_WhenAlreadyRevealed_ReturnsFalseAndDoesNotDoubleCountPoints()
    {
        var board = new RawBoard(new int[,]
        {
            { 2, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        Assert.That(state.TryRevealCell(0, 0), Is.True);
        Assert.That(state.CurrentPoints, Is.EqualTo(2));
        Assert.That(state.TryRevealCell(0, 0), Is.False); // same cell again
        Assert.That(state.CurrentPoints, Is.EqualTo(2));
    }

    [Test]
    public void TryRevealCell_WhenGameOver_ReturnsFalse()
    {
        var board = new RawBoard(new int[,]
        {
            { 0, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.TryRevealCell(0, 0); // lose

        var revealed = state.TryRevealCell(0, 1);

        Assert.That(revealed, Is.False);
        Assert.That(state.Outcome, Is.EqualTo(GameOutcome.Lost));
    }

    [Test]
    public void NewGame_HasNoMarks()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        Assert.That(state.GetCellMark(0, 0), Is.EqualTo(CellMarks.None));
        Assert.That(state.GetCellMark(2, 3), Is.EqualTo(CellMarks.None));
    }

    [Test]
    public void SetCellMark_StoresMark_GetCellMarkReturnsIt()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        state.SetCellMark(1, 2, CellMarks.Mark1);

        Assert.That(state.GetCellMark(1, 2), Is.EqualTo(CellMarks.Mark1));
    }

    [Test]
    public void SetCellMark_WithNone_ClearsMark()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.SetCellMark(0, 0, CellMarks.Mark2);

        state.SetCellMark(0, 0, CellMarks.None);

        Assert.That(state.GetCellMark(0, 0), Is.EqualTo(CellMarks.None));
    }

    [Test]
    public void SetCellMark_DoesNotAffectOtherCells()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        state.SetCellMark(2, 2, CellMarks.Mark3);

        Assert.That(state.GetCellMark(2, 1), Is.EqualTo(CellMarks.None));
        Assert.That(state.GetCellMark(2, 3), Is.EqualTo(CellMarks.None));
        Assert.That(state.GetCellMark(2, 2), Is.EqualTo(CellMarks.Mark3));
    }

    [Test]
    public void SetCellMark_AcceptsCombinedMarks()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        state.SetCellMark(0, 0, CellMarks.Mark0 | CellMarks.Mark2);

        Assert.That(state.GetCellMark(0, 0), Is.EqualTo(CellMarks.Mark0 | CellMarks.Mark2));
    }

    [Test]
    public void AddCellMark_CombinesWithExistingMarks()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.SetCellMark(1, 1, CellMarks.Mark0);

        state.AddCellMark(1, 1, CellMarks.Mark1);

        Assert.That(state.GetCellMark(1, 1), Is.EqualTo(CellMarks.Mark0 | CellMarks.Mark1));
    }

    [Test]
    public void RemoveCellMark_RemovesOnlySpecifiedMarks()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.SetCellMark(3, 3, CellMarks.Mark0 | CellMarks.Mark1 | CellMarks.Mark2);

        state.RemoveCellMark(3, 3, CellMarks.Mark1);

        Assert.That(state.GetCellMark(3, 3), Is.EqualTo(CellMarks.Mark0 | CellMarks.Mark2));
    }

    [Test]
    public void RemoveCellMark_WhenNoneLeft_ResultsInNone()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.SetCellMark(4, 4, CellMarks.Mark2);

        state.RemoveCellMark(4, 4, CellMarks.Mark2);

        Assert.That(state.GetCellMark(4, 4), Is.EqualTo(CellMarks.None));
    }

    [Test]
    public void SetTargetedCell_WhenHidden_SetsTarget_GetCellStateReturnsTargeted()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);

        var set = state.SetTargetedCell(2, 3);

        Assert.That(set, Is.True);
        Assert.That(state.TargetedRow, Is.EqualTo(2));
        Assert.That(state.TargetedColumn, Is.EqualTo(3));
        Assert.That(state.GetCellState(2, 3), Is.EqualTo(CellState.Targeted));
        Assert.That(state.GetCellState(0, 0), Is.EqualTo(CellState.Hidden));
    }

    [Test]
    public void SetTargetedCell_WhenRevealed_ReturnsFalse()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.TryRevealCell(1, 1);

        var set = state.SetTargetedCell(1, 1);

        Assert.That(set, Is.False);
        Assert.That(state.GetCellState(1, 1), Is.EqualTo(CellState.Revealed));
    }

    [Test]
    public void TryRevealCell_ClearsTargetedCell()
    {
        var board = new RawBoard(new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.SetTargetedCell(0, 0);

        state.TryRevealCell(0, 0);

        Assert.That(state.TargetedRow, Is.EqualTo(-1));
        Assert.That(state.TargetedColumn, Is.EqualTo(-1));
    }
}
