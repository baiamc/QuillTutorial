using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public Sprite FloorSprite;

    Dictionary<Tile, GameObject> _tileGameObjectMap;
    World _world;

    // Use this for initialization
    void Start()
    {
        _world = new World();
        _tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Create GameObject for each tile
        for (int x = 0; x < _world.Width; x++)
        {
            for (int y = 0; y < _world.Height; y++)
            {
                var tileData = _world.GetTileAt(x, y);
                var tileGo = new GameObject
                {
                    name = "Tile_" + x + "_" + y
                };

                tileGo.transform.position = new Vector3(x, y);
                tileGo.transform.SetParent(this.transform, true);

                tileGo.AddComponent<SpriteRenderer>();
                _tileGameObjectMap.Add(tileData, tileGo);
                tileData.TileTypeChanged += TileTypeChanged;
            }
        }

        _world.RandomizeTiles();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void TileTypeChanged(Tile tile)
    {
        GameObject tileGo;
        if (!_tileGameObjectMap.TryGetValue(tile, out tileGo)) {
            Debug.LogError("Could not get game object for tile at (" + tile.X + ", " + tile.Y + ")");
            return;
        }

        var tileSr = tileGo.GetComponent<SpriteRenderer>();
        if (tileSr == null)
        {
            Debug.LogError("Could not get SpriteRenderer for tile at (" + tile.X + ", " + tile.Y + ")");
            return;
        }


        switch (tile.TileType)
        {
            case TileType.Empty:
                tileSr.sprite = null;
                break;
            case TileType.Floor:
                tileSr.sprite = FloorSprite;
                break;
            default:
                Debug.LogError("Unknown TileType");
                break;
        }
    }
}
