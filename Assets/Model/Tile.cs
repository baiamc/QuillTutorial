using UnityEngine;
using System.Collections;

public enum TileType { Empty, Floor }

public class Tile
{

    TileType _tileType = TileType.Empty;

    World _world;

    Inventory _inventory;
    Furniture _furniture;


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

    public int X { get; protected set; }

    public int Y { get; protected set; }

    public Furniture Furniture
    {
        get
        {
            return _furniture;
        }
    }

    public delegate void TileTypeChangedEventHandler(Tile sender);
    public event TileTypeChangedEventHandler TileTypeChanged;

    public Tile(World world, int x, int y)
    {
        _world = world;
        X = x;
        Y = y;

    }

    private void RaiseTileTypeChanged()
    {
        if (TileTypeChanged != null)
        {
            TileTypeChanged(this);
        }
    }

    public bool PlaceFurniture(Furniture objInstance)
    {
        if (objInstance == null)
        {
            // Uninstall furniture
            _furniture = null;
            return true;
        }
        if (Furniture != null)
        {
            Debug.LogError("Trying to assign furniture to a tile that already has one!");
            return false;
        }

        _furniture = objInstance;
        return true;
    }
}
