using UnityEngine;
using System.Collections;

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
        return new Furniture
        {
            FurnitureType = furnitureType,
            _movementCost = movementCost,
            _width = width,
            _height = height,
            LinksToNeighbor = linksToNeighbor
        };
    }

    static public Furniture PlaceFurniture(Furniture proto, Tile tile)
    {
        var obj = new Furniture
        {
            FurnitureType = proto.FurnitureType,
            _movementCost = proto._movementCost,
            _width = proto._width,
            _height = proto._height,
            LinksToNeighbor = proto.LinksToNeighbor,
            Tile = tile
        };

        if (!tile.PlaceFurniture(obj))
        {
            return null;
        }

        return obj;
    }

}
