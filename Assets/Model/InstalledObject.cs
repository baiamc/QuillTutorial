using UnityEngine;
using System.Collections;

public class InstalledObject {

    // This represents the BASE tile of the object, but in practice large objects may actually occupy multiple tiles
    Tile _tile;  

    string _objectType;

    // This is a multiplier so a value of 2 means you move twice as slowly (ie at half speed)
    // SPECIAL: If _movementCost is 0, then this tile is impassible (e.g. a wall)
    float _movementCost;

    int _width;
    int _height;

    // Used by object factory to create the prototypical object
    public InstalledObject(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        _objectType = objectType;
        _movementCost = movementCost;
        _width = width;
        _height = height;
    }

    public InstalledObject(InstalledObject proto, Tile tile)
    {
        _objectType = proto._objectType;
        _movementCost = proto._movementCost;
        _width = proto._width;
        _height = proto._height;

        _tile = tile;
        tile._installedObject = this;
    }

}
