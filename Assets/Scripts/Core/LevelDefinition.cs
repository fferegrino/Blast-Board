using System.Collections.Generic;
public class LevelDefinition
{

    public static readonly Dictionary<int, List<LevelDefinition>> LEVEL_DEFINITIONS = new Dictionary<int, List<LevelDefinition>>()
    {
        [1] = new()
        {
            new LevelDefinition(24, 3, 1, 6),
            new LevelDefinition(27, 0, 3, 6),
            new LevelDefinition(32, 5, 0, 6),
            new LevelDefinition(36, 2, 2, 6),
            new LevelDefinition(48, 4, 1, 6)
        },
        [2] = new()
        {
            new LevelDefinition(54, 1, 3, 7),
            new LevelDefinition(64, 6, 0, 7),
            new LevelDefinition(72, 3, 2, 7),
            new LevelDefinition(81, 0, 4, 7),
            new LevelDefinition(96, 5, 1, 7)
        },
        [3] = new()
        {
            new LevelDefinition(108, 2, 3, 8),
            new LevelDefinition(128, 7, 0, 8),
            new LevelDefinition(144, 4, 2, 8),
            new LevelDefinition(162, 1, 4, 8),
            new LevelDefinition(192, 6, 1, 8)
        },
        [4] = new()
        {
            new LevelDefinition(216, 3, 3, 8),
            new LevelDefinition(243, 0, 5, 8),
            new LevelDefinition(256, 8, 0, 10),
            new LevelDefinition(288, 5, 2, 10),
            new LevelDefinition(324, 2, 4, 10)
        },
        [5] = new()
        {
            new LevelDefinition(384, 7, 1, 10),
            new LevelDefinition(432, 4, 3, 10),
            new LevelDefinition(486, 1, 5, 10),
            new LevelDefinition(512, 9, 0, 10),
            new LevelDefinition(576, 6, 2, 10)
        },
        [6] = new()
        {
            new LevelDefinition(648, 3, 4, 10),
            new LevelDefinition(729, 0, 6, 10),
            new LevelDefinition(768, 8, 1, 10),
            new LevelDefinition(864, 5, 3, 10),
            new LevelDefinition(972, 2, 5, 10)
        },
        [7] = new()
        {
            new LevelDefinition(1152, 7, 2, 10),
            new LevelDefinition(1296, 4, 4, 10),
            new LevelDefinition(1458, 1, 6, 13),
            new LevelDefinition(1536, 9, 1, 13),
            new LevelDefinition(1728, 6, 3, 10)
        },
        [8] = new()
        {
            new LevelDefinition(2187, 0, 7, 10),
            new LevelDefinition(2304, 8, 2, 10),
            new LevelDefinition(2592, 5, 4, 10),
            new LevelDefinition(2916, 2, 6, 10),
            new LevelDefinition(3456, 7, 3, 10)
        }
    };

    public int Points { get; }
    public int X2 { get; }
    public int X3 { get; }
    public int X0 { get; }

    public int X1
    {
        get
        {
            return 25 - X2 - X3 - X0;
        }
    }

    public List<int> GetCells()
    {
        var cells = new List<int>();

        for (int i = 0; i < X1; i++)
        {
            cells.Add(1);
        }
        for (int i = 0; i < X2; i++)
        {
            cells.Add(2);
        }
        for (int i = 0; i < X3; i++)
        {
            cells.Add(3);
        }
        for (int i = 0; i < X0; i++)
        {
            cells.Add(0);
        }

        return cells;
    }

    public LevelDefinition(int points, int x2, int x3, int x0)
    {
        Points = points;
        X2 = x2;
        X3 = x3;
        X0 = x0;
    }
}
