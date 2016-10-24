using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Xml;

public class Furniture {

    // This represents the BASE tile of the furniture, but in practice large furniture may actually occupy multiple tiles
    public Tile Tile { get; protected set; }  

    public string FurnitureType { get; protected set; }
    public bool LinksToNeighbor { get; protected set; }

    // This is a multiplier so a value of 2 means you move twice as slowly (ie at half speed)
    // SPECIAL: If _movementCost is 0, then this tile is impassible (e.g. a wall)
    public float MovementCost { get; protected set; }

    int _width;
    int _height;

    // TODO: Implement larger furniture
    // TODO: Implement furniture rotation

    protected Furniture()
    {

    }

    public delegate void FurnitureChangedHandler(Furniture obj);
    public event FurnitureChangedHandler FurnitureChanged;

    public Func<Tile, bool> IsPositionValid { get; protected set; }

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
            MovementCost = movementCost,
            _width = width,
            _height = height,
            LinksToNeighbor = linksToNeighbor
        };

        furn.IsPositionValid = furn.IsValidPosition;

        return furn;
    }

    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if (proto.IsPositionValid(tile) == false)
        {
            Debug.LogError("PlaceInstance -- Position Validity Function returned FALSE.");
            return null;
        }

        var furn = new Furniture
        {
            FurnitureType = proto.FurnitureType,
            MovementCost = proto.MovementCost,
            _width = proto._width,
            _height = proto._height,
            LinksToNeighbor = proto.LinksToNeighbor,
            IsPositionValid = proto.IsPositionValid,
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

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Type", FurnitureType);
        writer.WriteAttributeString("X", Tile.X.ToString());
        writer.WriteAttributeString("Y", Tile.Y.ToString());
    }

    public void ReadXml(XmlReader reader)
    {
        // Not used yet, but needed later for more complicated furniture
    }
}
