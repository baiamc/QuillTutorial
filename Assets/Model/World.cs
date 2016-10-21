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
    }

}
