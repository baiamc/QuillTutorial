using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; }

    public Sprite FloorSprite;
    public Sprite EmptySprite;

    Dictionary<Tile, GameObject> _tileGameObjectMap;
    Dictionary<Furniture, GameObject> _furnitureGameObjectMap;
    Dictionary<string, Sprite> _furnitureSprites;

    public World World { get; private set; }

    // Use this for initialization
    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("Should not be multiple instances of WorldController.");
            return;
        }

        _furnitureSprites = Resources.LoadAll<Sprite>("Images/Furniture").ToDictionary(s => s.name);

        Instance = this;
        World = new World();

        World.FurnitureCreated += OnFurnitureCreated;

        _tileGameObjectMap = new Dictionary<Tile, GameObject>();
        _furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

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

                tileGo.AddComponent<SpriteRenderer>().sprite = EmptySprite;
                _tileGameObjectMap.Add(tileData, tileGo);
            }
        }

        World.TileChanged += OnTileChanged;

        // Center the camera
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
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

    public void OnTileChanged(Tile tile)
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

    public void OnFurnitureCreated(Furniture obj)
    {
        // Create a GameObject linked to this data

        // FIXME: Does not consider multi-tile objects nor rotated objects

        var tileGo = new GameObject
        {
            name = obj.FurnitureType + "_" + obj.Tile.X + "_" + obj.Tile.Y
        };

        tileGo.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y);
        tileGo.transform.SetParent(this.transform, true);

        var tileSr = tileGo.AddComponent<SpriteRenderer>();
        tileSr.sprite = GetSpriteForFurniture(obj);
        tileSr.sortingLayerName = "Furniture";

        _furnitureGameObjectMap.Add(obj, tileGo);
        obj.FurnitureChanged += OnFurnitureChanged;
    }

    private void OnFurnitureChanged(Furniture furn)
    {
        // Make sure the furniture's graphics are correct
        GameObject furn_go = _furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
    }

    Sprite GetSpriteForFurniture(Furniture obj)
    {
        if (obj.LinksToNeighbor == false)
        {
            return _furnitureSprites[obj.FurnitureType];
        }

        string spriteName = obj.FurnitureType + "_";

        int x = obj.Tile.X;
        int y = obj.Tile.Y;
        // North, East, South, West

        Tile t = World.GetTileAt(x, y + 1);
        if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
        {
            spriteName += "N";
        }
        t = World.GetTileAt(x + 1, y);
        if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
        {
            spriteName += "E";
        }
        t = World.GetTileAt(x, y - 1);
        if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
        {
            spriteName += "S";
        }
        t = World.GetTileAt(x - 1, y);
        if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
        {
            spriteName += "W";
        }

        if (_furnitureSprites.ContainsKey(spriteName) == false)
        {
            Debug.LogError("Could not find furniture sprite with name: " + spriteName);
            return null;
        }

        return _furnitureSprites[spriteName];
    }
}
