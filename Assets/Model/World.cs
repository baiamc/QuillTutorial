using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World
{

    Tile[,] _tiles;
    int _width;
    int _height;

    public int Width
    {
        get
        {
            return _width;
        }

    }

    public int Height
    {
        get
        {
            return _height;
        }

    }

    public World(int width = 100, int height = 100)
    {
        _width = width;
        _height = height;

        _tiles = new Tile[Width, Height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
            }
        }
    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
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
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            return null;
        }

        return _tiles[x, y];
    }
}
