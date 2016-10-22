using UnityEngine;
using System.Collections;

public class InstalledObject {

    // This represents the BASE tile of the object, but in practice large objects may actually occupy multiple tiles
    public Tile Tile { get; protected set; }  

    public string ObjectType { get; protected set; }

    // This is a multiplier so a value of 2 means you move twice as slowly (ie at half speed)
    // SPECIAL: If _movementCost is 0, then this tile is impassible (e.g. a wall)
    float _movementCost;

    int _width;
    int _height;

    // TODO: Implement larger objects
    // TODO: Implement object rotation

    protected InstalledObject()
    {

    }

    public delegate void InstalledObjectChangedHandler(InstalledObject obj);
    public event InstalledObjectChangedHandler InstalledObjectChanged;

    protected void RaiseInstalledObjectChanged()
    {
        if (InstalledObjectChanged != null)
        {
            InstalledObjectChanged(this);
        }
    }


    // Used by object factory to create the prototypical object
    static public InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        return new InstalledObject
        {
            ObjectType = objectType,
            _movementCost = movementCost,
            _width = width,
            _height = height,
        };
    }

    static public InstalledObject PlaceObject(InstalledObject proto, Tile tile)
    {
        var obj = new InstalledObject
        {
            ObjectType = proto.ObjectType,
            _movementCost = proto._movementCost,
            _width = proto._width,
            _height = proto._height,
            Tile = tile
        };

        if (!tile.PlaceObject(obj))
        {
            return null;
        }

        return obj;
    }

}
