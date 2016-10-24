using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class World : IXmlSerializable
{

    Tile[,] _tiles;
    Dictionary<string, Furniture> _furniturePrototypes;
    List<Character> _characters;
    List<Furniture> _furnitures;

    public Pathfinding.TileGraph TileGraph;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    // TODO: Most likely replaced with a dedicated class for managing job queues
    public JobQueue JobQueue { get; protected set; }

    public delegate void FurnitureCreatedHandler(Furniture obj);
    public event FurnitureCreatedHandler FurnitureCreated;

    public delegate void TileChangedHandler(Tile tile);
    public event TileChangedHandler TileChanged;

    public delegate void CharacterCreatedHandler(Character character);
    public event CharacterCreatedHandler CharacterCreated;

    public delegate void CharacterChangedHandler(Character character);
    public event CharacterChangedHandler CharacterChanged;

    private void RaiseFurnitureCreated(Furniture obj)
    {
        if (FurnitureCreated != null)
        {
            FurnitureCreated(obj);
        }
    }

    private void RaiseTileChanged(Tile tile)
    {
        if (TileChanged != null)
        {
            TileChanged(tile);
        }
    }
    private void RaiseCharacterCreated(Character character)
    {
        if (CharacterCreated != null)
        {
            CharacterCreated(character);
        }
    }

    private void RaiseCharacterChanged(Character character)
    {
        if (CharacterChanged != null)
        {
            CharacterChanged(character);
        }
    }



    public World(int width, int height)
    {
        SetupWorld(width, height);
        CreateCharacter(_tiles[Width / 2, Height / 2]);
    }

    private void SetupWorld(int width, int height)
    {
        Width = width;
        Height = height;

        _tiles = new Tile[Width, Height];
        JobQueue = new JobQueue();
        _characters = new List<Character>();
        _furnitures = new List<Furniture>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
                _tiles[x, y].TileTypeChanged += OnTileChanged;
            }
        }

        CreateFurniturePrototypes();
    }

    public void Update(float deltaTime)
    {
        foreach (var c in _characters)
        {
            c.Update(deltaTime);
        }
    }

    public Character CreateCharacter(Tile tile)
    {
        var c = new Character(tile);
        _characters.Add(c);
        c.CharacterChanged += RaiseCharacterChanged;
        RaiseCharacterCreated(c);
        return c;
    }

    private void CreateFurniturePrototypes()
    {
        _furniturePrototypes = new Dictionary<string, Furniture>();

        _furniturePrototypes.Add("Wall",
            Furniture.CreatePrototype("Wall", 0f, 1, 1, true));
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
        {
            return null;
        }

        return _tiles[x, y];
    }

    public Furniture PlaceFurniture(string furnitureType, Tile tile)
    {
        Furniture proto;
        if (!_furniturePrototypes.TryGetValue(furnitureType, out proto))
        {
            Debug.LogError("_furniturePrototypes doesn't contain a proto for key: " + furnitureType);
            return null;
        }

        var furn = Furniture.PlaceInstance(proto, tile);
        if (furn == null)
        {
            return null;
        }
        _furnitures.Add(furn);
        RaiseFurnitureCreated(furn);
        InvalidatePathfinding();
        return furn;
    }

    public bool IsFurniturePlacementValid(string furnatureType, Tile tile)
    {
        return _furniturePrototypes[furnatureType].IsPositionValid(tile);
    }

    public Furniture GetFurniturePrototype(string objectType)
    {
        Furniture furn;
        if (_furniturePrototypes.TryGetValue(objectType, out furn))
        {
            Debug.LogError("GetFurniturePrototype: No furniture of type: " + objectType);
            return null;
        }

        return furn;
    }

    public IEnumerable<Tile> Tiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                yield return _tiles[x, y];
            }
        }
    }

    public IEnumerable<Furniture> FurnitureList()
    {
        return _furnitures;
    }

    public IEnumerable<Character> Characters()
    {
        return _characters;
    }

    private void OnTileChanged(Tile tile)
    {
        InvalidatePathfinding();
        RaiseTileChanged(tile);
    }

    private void InvalidatePathfinding()
    {
        TileGraph = null;
    }

    public void SetupPathfindingExample()
    {
        int l = Width / 2 - 5;
        int b = Height / 2 - 5;

        for (int x = l - 5; x < l + 15; x++)
        {
            for (int y = b - 5; y < b + 15; y++)
            {
                _tiles[x, y].TileType = TileType.Floor;

                if (x == l || x == (l + 9) || y == b || y == (b + 9))
                {
                    if (x != (l + 9) && y != (b + 4))
                    {
                        PlaceFurniture("Wall", _tiles[x, y]);
                    }
                }
            }
        }
    }

    #region Saving/Loading

    public World()
    {

    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        SetupWorld(int.Parse(reader.GetAttribute("Width")), int.Parse(reader.GetAttribute("Height")));
        reader.MoveToElement();

        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "Tiles":
                    ReadXml_Tiles(reader);
                    break;
                case "Furnitures":
                    ReadXml_Furniture(reader);
                    break;
                case "Characters":
                    ReadXml_Characters(reader);
                    break;
                default:
                    break;
            }
        }
    }

    public void ReadXml_Tiles(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name != "Tile")
            {
                return;
            }
            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));
            _tiles[x, y].ReadXml(reader);
        }
    }

    private void ReadXml_Furniture(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name != "Furniture")
            {
                return;
            }

            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));
            string type = reader.GetAttribute("Type");
            var furn = PlaceFurniture(type, _tiles[x, y]);
            furn.ReadXml(reader);
        }
    }

    private void ReadXml_Characters(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name != "Character")
            {
                return;
            }

            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));
            var c = CreateCharacter(_tiles[x, y]);
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Width", Width.ToString());
        writer.WriteAttributeString("Height", Height.ToString());

        writer.WriteStartElement("Tiles");
        foreach (var tile in Tiles())
        {
            writer.WriteStartElement("Tile");
            tile.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Furnitures");
        foreach (var furn in FurnitureList())
        {
            writer.WriteStartElement("Furniture");
            furn.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Characters");
        foreach (var c in _characters)
        {
            writer.WriteStartElement("Character");
            c.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    #endregion
}
