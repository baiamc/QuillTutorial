﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public IEnumerable<Tile> GetNeighbors()
    {
        Tile t = World.GetTileAt(X, Y + 1);
        if (t != null)
        {
            yield return t;
        }

        t = World.GetTileAt(X + 1, Y);
        if (t != null)
        {
            yield return t;
        }

        t = World.GetTileAt(X, Y - 1);
        if (t != null)
        {
            yield return t;
        }

        t = World.GetTileAt(X - 1, Y);
        if (t != null)
        {
            yield return t;
        }
    }

    public bool IsNeighbour(Tile tile, bool diagOkay = false)
    {
        if (this.X == tile.X && Math.Abs(this.Y - tile.Y) == 1)
        {
            return true;
        }

        if (this.Y == tile.Y && Math.Abs(this.X - tile.X) == 1)
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
}