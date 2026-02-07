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
            { 0, 0, 3, 0, 4 },
            { 0, 0, 0, 0, 5 }
        });
        var state = new GameState(board);

        int[] expectedColumnSumValues = { 1, 5, 6, 0, 15 };
        int[] expectedColumnMultValues = { 1, 6, 9, 0, 120 };
        int[] expectedColumnBombs = { 4, 3, 3, 5, 0 };

        Assert.That(state.ColumnSumValues, Is.EqualTo(expectedColumnSumValues));
        Assert.That(state.ColumnMultValues, Is.EqualTo(expectedColumnMultValues));
        Assert.That(state.ColumnBombs, Is.EqualTo(expectedColumnBombs));

        int[] expectedRowSumValues = { 2, 4, 9, 7, 5 };
        int[] expectedRowMultValues = { 1, 4, 27, 12, 5 };
        int[] expectedRowBombs = { 3, 3, 2, 3, 4 };

        Assert.That(state.RowSumValues, Is.EqualTo(expectedRowSumValues));
        Assert.That(state.RowMultValues, Is.EqualTo(expectedRowMultValues));
        Assert.That(state.RowBombs, Is.EqualTo(expectedRowBombs));
    }
}
