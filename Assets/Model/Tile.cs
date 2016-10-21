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

    public TileType TileType
    {
        get
        {
            return _tileType;
        }

        set
        {
            _tileType = value;
            RaiseTileTypeChanged();
        }
    }

    public int X
    {
        get
        {
            return _x;
        }
    }

    public int Y
    {
        get
        {
            return _y;
        }
    }

    public delegate void TileTypeChangedEventHandler(Tile sender);
    public event TileTypeChangedEventHandler TileTypeChanged;

    public Tile(World world, int x, int y)
    {
        _world = world;
        _x = x;
        _y = y;

    }

    private void RaiseTileTypeChanged()
    {
        if (TileTypeChanged != null)
        {
            TileTypeChanged(this);
        }
    }
}
