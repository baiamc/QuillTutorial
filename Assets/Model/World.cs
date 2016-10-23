using UnityEngine;
using System.Collections.Generic;
using System;

public class World
{

    Tile[,] _tiles;
    Dictionary<string, Furniture> _furniturePrototypes;
    List<Character> _characters;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    // TODO: Most likely replaced with a dedicated class for managing job queues
    public JobQueue JobQueue { get; protected set; }

    public delegate void FurnitureCreatedHandler(Furniture obj);
    public event FurnitureCreatedHandler FurnitureCreated;

    public delegate void TileChangedHandler(Tile tile);
    public event TileChangedHandler TileChanged;

    public delegate void CharacterCreatedHandler(Character character);
    public event CharacterCreatedHandler CharacterCreated;

    private void RaiseFurnitureCreated(Furniture obj)
    {
        if (FurnitureCreated != null)
        {
            FurnitureCreated(obj);
        }
    }

    private void RaiseTileChanged(Tile tile)
    {
        if (TileChanged != null)
        {
            TileChanged(tile);
        }
    }
    private void RaiseCharacterCreated(Character character)
    {
        if (CharacterCreated != null)
        {
            CharacterCreated(character);
        }
    }



    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        _tiles = new Tile[Width, Height];
        JobQueue = new JobQueue();
        _characters = new List<Character>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
                _tiles[x, y].TileTypeChanged += RaiseTileChanged;
            }
        }

        CreateFurniturePrototypes();
    }
    public void CreateCharacter(Tile tile)
    {
        var c = new Character(_tiles[Width / 2, Height / 2]);
        RaiseCharacterCreated(c);
    }

    private void CreateFurniturePrototypes()
    {
        _furniturePrototypes = new Dictionary<string, Furniture>();

        _furniturePrototypes.Add("Wall", 
            Furniture.CreatePrototype("Wall", 0f, 1, 1, true));
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

    public bool IsFurniturePlacementValid(string furnatureType, Tile tile)
    {
        return _furniturePrototypes[furnatureType].IsPositionValid(tile);
    }

    public Furniture GetFurniturePrototype(string objectType)
    {
        Furniture furn;
        if (_furniturePrototypes.TryGetValue(objectType, out furn))
        {
            Debug.LogError("GetFurniturePrototype: No furniture of type: " + objectType);
            return null;
        }

        return furn;
    }
}
