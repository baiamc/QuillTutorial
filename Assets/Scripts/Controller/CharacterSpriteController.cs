using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterSpriteController : MonoBehaviour {

    Dictionary<Character, GameObject> _characterGameObjectMap;
    Dictionary<string, Sprite> _characterSprites;

    World World { get { return WorldController.Instance.World; } }

    // Use this for initialization
    void Start()
    {
        _characterSprites = Resources.LoadAll<Sprite>("Images/Characters").ToDictionary(s => s.name);

        _characterGameObjectMap = new Dictionary<Character, GameObject>();

        World.CharacterCreated += OnCharacterCreated;

        // DEBUG
        var c = World.CreateCharacter(World.GetTileAt(World.Width / 2, World.Height / 2));
    }

    public void OnCharacterCreated(Character character)
    {
        // Create a GameObject linked to this data

        // FIXME: Does not consider multi-tile objects nor rotated objects

        var tileGo = new GameObject
        {
            name = "Character"
        };

        tileGo.transform.position = new Vector3(character.X, character.Y);
        tileGo.transform.SetParent(this.transform, true);

        var tileSr = tileGo.AddComponent<SpriteRenderer>();
        tileSr.sprite = _characterSprites["p1_front"];
        tileSr.sortingLayerName = "Characters";

        _characterGameObjectMap.Add(character, tileGo);
        World.CharacterChanged += OnCharacterChanged;
    }

    private void OnCharacterChanged(Character character)
    {
        // Make sure the furniture's graphics are correct
        GameObject char_go;
        if (_characterGameObjectMap.TryGetValue(character, out char_go) == false)
        {
            Debug.LogError("OnCharacter Changed: Tried to update vsuals for character not in our map.");
        }

        char_go.transform.position = new Vector3(character.X, character.Y);
        
    }

    //public Sprite GetSpriteForFurniture(Furniture obj)
    //{
    //    if (obj.LinksToNeighbor == false)
    //    {
    //        return _furnitureSprites[obj.FurnitureType];
    //    }

    //    string spriteName = obj.FurnitureType + "_";

    //    int x = obj.Tile.X;
    //    int y = obj.Tile.Y;
    //    // North, East, South, West

    //    Tile t = World.GetTileAt(x, y + 1);
    //    if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
    //    {
    //        spriteName += "N";
    //    }
    //    t = World.GetTileAt(x + 1, y);
    //    if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
    //    {
    //        spriteName += "E";
    //    }
    //    t = World.GetTileAt(x, y - 1);
    //    if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
    //    {
    //        spriteName += "S";
    //    }
    //    t = World.GetTileAt(x - 1, y);
    //    if (t != null && t.Furniture != null && t.Furniture.FurnitureType == obj.FurnitureType)
    //    {
    //        spriteName += "W";
    //    }

    //    if (_furnitureSprites.ContainsKey(spriteName) == false)
    //    {
    //        Debug.LogError("Could not find furniture sprite with name: " + spriteName);
    //        return null;
    //    }

    //    return _furnitureSprites[spriteName];
    //}

    //public Sprite GetSpriteForFurniture(string objectType)
    //{
    //    Sprite sprite;
    //    if (_furnitureSprites.TryGetValue(objectType, out sprite))
    //    {
    //        return sprite;
    //    }

    //    if (_furnitureSprites.TryGetValue(objectType + "_", out sprite))
    //    {
    //        return sprite;
    //    }

    //    Debug.LogError("GetSpriteForFurniture: No sprites with name: " + objectType);
    //    return null;
    //}
}
