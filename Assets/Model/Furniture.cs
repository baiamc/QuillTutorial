using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class Furniture {

    // This represents the BASE tile of the furniture, but in practice large furniture may actually occupy multiple tiles
    public Tile Tile { get; protected set; }  

    public string FurnitureType { get; protected set; }
    public bool LinksToNeighbor { get; protected set; }

    // This is a multiplier so a value of 2 means you move twice as slowly (ie at half speed)
    // SPECIAL: If _movementCost is 0, then this tile is impassible (e.g. a wall)
    float _movementCost;

    int _width;
    int _height;

    // TODO: Implement larger furniture
    // TODO: Implement furniture rotation

    protected Furniture()
    {

    }

    public delegate void FurnitureChangedHandler(Furniture obj);
    public event FurnitureChangedHandler FurnitureChanged;

    public Func<Tile, bool> funcPositionValidation { get; protected set; }

    protected void RaiseFurnitureChanged()
    {
        if (FurnitureChanged != null)
        {
            FurnitureChanged(this);
        }
    }

    // Used by object factory to create the prototypical furniture
    static public Furniture CreatePrototype(string furnitureType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false)
    {
        var furn = new Furniture
        {
            FurnitureType = furnitureType,
            _movementCost = movementCost,
            _width = width,
            _height = height,
            LinksToNeighbor = linksToNeighbor
        };

        furn.funcPositionValidation = furn.IsValidPosition;

        return furn;
    }

    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if (proto.funcPositionValidation(tile) == false)
        {
            Debug.LogError("PlaceInstance -- Position Validity Function returned FALSE.");
            return null;
        }

        var furn = new Furniture
        {
            FurnitureType = proto.FurnitureType,
            _movementCost = proto._movementCost,
            _width = proto._width,
            _height = proto._height,
            LinksToNeighbor = proto.LinksToNeighbor,
            funcPositionValidation = proto.funcPositionValidation,
            Tile = tile
        };

        if (!tile.PlaceFurniture(furn))
        {
            return null;
        }

        if (furn.LinksToNeighbor)
        {
            foreach (var t in tile.GetNeighbors())
            {
                if (t.Furniture != null && t.Furniture.FurnitureType == furn.FurnitureType)
                {
                    t.Furniture.RaiseFurnitureChanged();
                }
            }
        }

        return furn;
    }

    public bool IsValidPosition(Tile tile)
    {
        // Make sure tile is FLOOR
        // Make sure tile in empty
        if (tile.TileType != TileType.Floor)
        {
            return false;
        }

        if (tile.Furniture != null)
        {
            return false;
        }

        return true;
    }

    public bool IsValidPosition_Door(Tile tile)
    {
        // Must have valid N/S Walls or E/W Walls
        if (IsValidPosition(tile) == false) 
        {
            return false;
        }

        return true;
    }

}
