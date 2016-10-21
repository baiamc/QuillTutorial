using System.Collections;

public enum TileType { Empty, Floor }

public class Tile
{

    TileType _tileType = TileType.Empty;

    World _world;
    int _x;
    int _y;

    LooseObject _looseObject;
    InstalledObject _installedObject;

    public Tile(World world, int x, int y)
    {
        _world = world;
        _x = x;
        _y = y;

    }
}
