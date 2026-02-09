using System.Linq;
using NUnit.Framework;

namespace BoardBlast.Tests;

public class RawBoardTests
{
    [Test]
    public void Constructor_WithArray_StoresValues()
    {
        var grid = new int[,]
        {
            { 1, 2, 3, 0, 1 },
            { 0, 1, 2, 3, 0 },
            { 2, 0, 1, 0, 2 },
            { 1, 3, 0, 1, 3 },
            { 0, 2, 1, 2, 0 }
        };
        var board = new RawBoard(grid);

        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                Assert.That(board[r, c], Is.EqualTo(grid[r, c]));
            }
        }
    }

    [Test]
    public void Indexer_ReturnsCorrectValues()
    {
        var board = new RawBoard(new int[,]
        {
            { 3, 0, 1 },
            { 0, 2, 0 },
            { 1, 1, 2 }
        });
        Assert.That(board[0, 0], Is.EqualTo(3));
        Assert.That(board[1, 1], Is.EqualTo(2));
        Assert.That(board[2, 2], Is.EqualTo(2));
    }

    [Test]
    public void Constructor_WithLevelDefinition_Produces5x5BoardWithCorrectDistribution()
    {
        var level = new LevelDefinition(24, 3, 1, 6);
        var expectedCells = level.GetCells().OrderBy(x => x).ToList();

        var board = new RawBoard(level);
        var actualCells = new System.Collections.Generic.List<int>();
        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                actualCells.Add(board[r, c]);
            }
        }

        actualCells.Sort();

        Assert.That(actualCells, Has.Count.EqualTo(25));
        Assert.That(actualCells, Is.EquivalentTo(expectedCells));
    }
}
