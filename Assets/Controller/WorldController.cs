using UnityEngine;
using System.Collections.Generic;
using System;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; }

    public Sprite FloorSprite;
    public Sprite WallSprite; // FIXME

    Dictionary<Tile, GameObject> _tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> _installedObjectGameObjectMap;

    public World World { get; private set; }

    // Use this for initialization
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Should not be multiple instances of WorldController.");
            return;
        }
        Instance = this;
        World = new World();

        World.InstalledObjectCreated += OnInstalledObjectCreated;

        _tileGameObjectMap = new Dictionary<Tile, GameObject>();
        _installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

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

                tileGo.AddComponent<SpriteRenderer>();
                _tileGameObjectMap.Add(tileData, tileGo);
                tileData.TileTypeChanged += TileTypeChanged;
            }
        }

        World.RandomizeTiles();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public Tile GetTileAtWorldCoords(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);
        return World.GetTileAt(x, y);
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

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        // Create a GameObject linked to this data

        // FIXME: Does not consider multi-tile objects nor rotated objects

        var tileGo = new GameObject
        {
            name = obj.ObjectType + "_" + obj.Tile.X + "_" + obj.Tile.Y
        };

        tileGo.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y);
        tileGo.transform.SetParent(this.transform, true);

        // FIXME: We assume that the object must be a wall, so use
        // the hard coded reference to the wall sprite
        var tileSr = tileGo.AddComponent<SpriteRenderer>();
        tileSr.sprite = WallSprite;
        tileSr.sortingLayerName = "InstalledObject";

        _installedObjectGameObjectMap.Add(obj, tileGo);
        obj.InstalledObjectChanged += OnInstalledObjectChanged;
    }

    private void OnInstalledObjectChanged(InstalledObject obj)
    {
        Debug.LogError("OnInstalledObjectChanged -- Not Implemented");
    }
}
