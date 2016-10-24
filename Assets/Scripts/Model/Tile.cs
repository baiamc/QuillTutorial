using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

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

    public World World { get { return _world; } }

    public int X { get; protected set; }

    public int Y { get; protected set; }

    public float MovementCost
    {
        get
        {
            if (_tileType == TileType.Empty)
            {
                return 0;
            }
            if (Furniture == null)
            {
                return 1;
            }

            return Furniture.MovementCost;
        }
    }

    public Furniture Furniture
    {
        get
        {
            return _furniture;
        }
    }

    public Job PendingFurnatureJob { get; set; }

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

    public Tile[] GetNeighbors(bool diagOkay = false)
    {
        Tile[] tiles;
        if (diagOkay == false)
        {
            tiles = new Tile[4];  // Tile order N E S W
        }
        else
        {
            tiles = new Tile[8]; // Tile order N E S W NE SE SW NW
        }

        tiles[0] = World.GetTileAt(X, Y + 1);
        tiles[1] = World.GetTileAt(X + 1, Y);
        tiles[2] = World.GetTileAt(X, Y - 1);
        tiles[3] = World.GetTileAt(X - 1, Y);

        if (diagOkay)
        {
            tiles[4] = World.GetTileAt(X + 1, Y + 1);
            tiles[5] = World.GetTileAt(X + 1, Y - 1);
            tiles[6] = World.GetTileAt(X - 1, Y - 1);
            tiles[7] = World.GetTileAt(X - 1, Y + 1);
        }

        return tiles;
    }

    public bool IsNeighbour(Tile tile, bool diagOkay = false)
    {
        if (Math.Abs(this.X - tile.X) + Math.Abs(this.Y - tile.Y) == 1)
        {
            return true;
        }

        if (diagOkay)
        {
            if (Math.Abs(this.X - tile.X) == 1 && Math.Abs(this.Y - tile.Y) == 1)
            {
                return true;
            }
        }

        return false;
    }

    #region Save/Load Code

    public void ReadXml(XmlReader reader)
    {
        TileType = (TileType)Enum.Parse(typeof(TileType), reader.GetAttribute("Type"));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Type", TileType.ToString());
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
    }

    #endregion
}
