using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class FurnitureSpriteController : MonoBehaviour
{
    Dictionary<Furniture, GameObject> _furnitureGameObjectMap;
    Dictionary<string, Sprite> _furnitureSprites;

    World World { get { return WorldController.Instance.World; } }

    // Use this for initialization
    void Start()
    {
        _furnitureSprites = Resources.LoadAll<Sprite>("Images/Furniture").ToDictionary(s => s.name);

        _furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        World.FurnitureCreated += OnFurnitureCreated;
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

    public Sprite GetSpriteForFurniture(Furniture obj)
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

    public Sprite GetSpriteForFurniture(string objectType)
    {
        Sprite sprite;
        if (_furnitureSprites.TryGetValue(objectType, out sprite))
        {
            return sprite;
        }

        if (_furnitureSprites.TryGetValue(objectType + "_", out sprite))
        {
            return sprite;
        }

        Debug.LogError("GetSpriteForFurniture: No sprites with name: " + objectType);
        return null;
    }
}
