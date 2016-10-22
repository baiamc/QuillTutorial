using UnityEngine;
using System.Collections.Generic;
using System;

public class World
{

    Tile[,] _tiles;
    Dictionary<string, InstalledObject> _installedObjectPrototypes;

    public int Width { get; protected set; }

    public int Height { get; protected set; }


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

        CreateInstalledObjectPrototypes();
    }

    private void CreateInstalledObjectPrototypes()
    {
        _installedObjectPrototypes = new Dictionary<string, InstalledObject>();

        _installedObjectPrototypes.Add("Wall", 
            InstalledObject.CreatePrototype("Wall", 0f));
    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Random.Range(0, 2) == 0)
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

    internal void PlaceInstalledObject(string _buildModeObjectType, Tile tile)
    {
        throw new NotImplementedException();
    }
}
