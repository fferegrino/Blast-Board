using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BoardBlast.Tests;

public class LevelDefinitionTests
{
    [Test]
    public void GetCells_Returns25Cells()
    {
        var level = new LevelDefinition(24, 3, 1, 6);
        var cells = level.GetCells();
        Assert.That(cells, Has.Count.EqualTo(25));
    }

    [Test]
    public void GetCells_ReturnsCorrectCountsOfEachValue()
    {
        var level = new LevelDefinition(24, 3, 1, 6); // X2=3, X3=1, X0=6 -> X1=15
        var cells = level.GetCells();

        Assert.That(cells.Count(c => c == 1), Is.EqualTo(15));
        Assert.That(cells.Count(c => c == 2), Is.EqualTo(3));
        Assert.That(cells.Count(c => c == 3), Is.EqualTo(1));
        Assert.That(cells.Count(c => c == 0), Is.EqualTo(6));
    }

    [Test]
    public void X1_Equals25MinusX2MinusX3MinusX0()
    {
        var level = new LevelDefinition(100, 5, 4, 2);
        Assert.That(level.X1, Is.EqualTo(25 - 5 - 4 - 2));
        Assert.That(level.X1, Is.EqualTo(14));
    }

    [Test]
    public void Constructor_StoresPointsAndMultipliers()
    {
        var level = new LevelDefinition(48, 4, 1, 6);
        Assert.That(level.Points, Is.EqualTo(48));
        Assert.That(level.X2, Is.EqualTo(4));
        Assert.That(level.X3, Is.EqualTo(1));
        Assert.That(level.X0, Is.EqualTo(6));
    }

    [Test]
    public void LevelDefinitions_HasEightChaptersWithFiveLevelsEach()
    {
        var defs = LevelDefinition.LEVEL_DEFINITIONS;
        Assert.That(defs.Keys.Count, Is.EqualTo(8));

        for (int chapter = 1; chapter <= 8; chapter++)
        {
            Assert.That(defs[chapter], Has.Count.EqualTo(5), $"Chapter {chapter} should have 5 levels");
        }
    }
}
