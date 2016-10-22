using UnityEngine;
using System.Collections.Generic;
using System;

public class World
{

    Tile[,] _tiles;
    Dictionary<string, Furniture> _furniturePrototypes;

    public int Width { get; protected set; }

    public int Height { get; protected set; }

    public delegate void FurnitureCreatedHandler(Furniture obj);
    public event FurnitureCreatedHandler FurnitureCreated;

    private void RaiseFurnitureCreated(Furniture obj)
    {
        if (FurnitureCreated != null)
        {
            FurnitureCreated(obj);
        }
    }


    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        _tiles = new Tile[Width, Height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
            }
        }

        CreateFurniturePrototypes();
    }

    private void CreateFurniturePrototypes()
    {
        _furniturePrototypes = new Dictionary<string, Furniture>();

        _furniturePrototypes.Add("Wall", 
            Furniture.CreatePrototype("Wall", 0f, 1, 1, true));
    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    _tiles[x, y].TileType = TileType.Empty;
                }
                else
                {
                    _tiles[x, y].TileType = TileType.Floor;
                }
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
        {
            return null;
        }

        return _tiles[x, y];
    }

    public bool PlaceFurniture(string furnitureType, Tile tile)
    {
        Furniture proto;
        if (!_furniturePrototypes.TryGetValue(furnitureType, out proto))
        {
            Debug.LogError("_furniturePrototypes doesn't contain a proto for key: " + furnitureType);
            return false;
        }

        var obj = Furniture.PlaceInstance(proto, tile);
        if (obj == null)
        {
            return false;
        }
        RaiseFurnitureCreated(obj);
        return true;
    }
}
