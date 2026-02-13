public enum TileType
{
    One = 1,
    Two = 2,
    Three = 3,
    Voltorb = 0
}

public struct VoltorbTile
{
    public TileType Type;
}

public class VoltorbBoard
{
    public int Size;              // 5 for classic
    public VoltorbTile[] Tiles;   // length = Size * Size

    public int[] RowSum;
    public int[] RowVoltorbs;
    public int[] ColSum;
    public int[] ColVoltorbs;

    public VoltorbBoard(int size)
    {
        Size = size;
        Tiles = new VoltorbTile[size * size];
        RowSum = new int[size];
        RowVoltorbs = new int[size];
        ColSum = new int[size];
        ColVoltorbs = new int[size];
    }

    public VoltorbTile Get(int r, int c) => Tiles[r * Size + c];
    public void Set(int r, int c, TileType type) => Tiles[r * Size + c].Type = type;
}
