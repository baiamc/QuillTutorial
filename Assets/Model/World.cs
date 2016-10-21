using System.Collections;

public class World {

    Tile[,] _tiles;
    int _width;
    int _height;

    public World(int width = 100, int height = 100)
    {
        _width = width;
        _height = height;

        _tiles = new Tile[_width, _height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x > _width || x < 0 || y > _height || y < 0)
        {
            return null;
        }

        return _tiles[x, y];
    }

}
