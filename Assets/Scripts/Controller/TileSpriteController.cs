using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TileSpriteController : MonoBehaviour
{
    public Sprite FloorSprite;
    public Sprite EmptySprite;

    Dictionary<Tile, GameObject> _tileGameObjectMap;

    World World { get { return WorldController.Instance.World; } }

    // Use this for initialization
    void Start()
    {
        _tileGameObjectMap = new Dictionary<Tile, GameObject>();

        World.TileChanged += OnTileChanged;

        // Create GameObject for each tile
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                var tileData = World.GetTileAt(x, y);
                var tileGo = new GameObject
                {
                    name = "Tile_" + x + "_" + y
                };

                tileGo.transform.position = new Vector3(x, y);
                tileGo.transform.SetParent(this.transform, true);

                var sr = tileGo.AddComponent<SpriteRenderer>();
                sr.sprite = EmptySprite;
                sr.sortingLayerName = "Tiles";
                _tileGameObjectMap.Add(tileData, tileGo);

                OnTileChanged(tileData);
            }
        }
    }

    public void OnTileChanged(Tile tile)
    {
        GameObject tileGo;
        if (!_tileGameObjectMap.TryGetValue(tile, out tileGo))
        {
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
                tileSr.sprite = EmptySprite;
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
