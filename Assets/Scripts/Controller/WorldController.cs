using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; }

    public World World { get; private set; }

    static bool _loadWorld;

    // Use this for initialization
    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("Should not be multiple instances of WorldController.");
            return;
        }

        Instance = this;

        if (_loadWorld)
        {

            _loadWorld = false;
            CreateWorldFromSaveFile();
        }
        else
        {
            CreateEmptyWorld();
        }

    }

    void Update()
    {
        // TODO: Add pause/unpause, speed controls, etc.
        World.Update(Time.deltaTime);
    }

    private void CreateEmptyWorld()
    {
        World = new World(100, 100);

        // Center the camera
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
    }

    private void CreateWorldFromSaveFile()
    {

    }

    public void NewWorld()
    {
        Debug.Log("NewWorld");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveWorld()
    {
        var serializer = new XmlSerializer(typeof(World));
        TextWriter writer = new StringWriter();
        serializer.Serialize(writer, World);
        writer.Close();
        Debug.Log(writer.ToString());
    }

    public void LoadWorld()
    {
        // Reload the Scene
        _loadWorld = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetupPathfindingExample()
    {
        World.SetupPathfindingExample();

        var graph = new Pathfinding.TileGraph(World);
    }


    public Tile GetTileAtWorldCoords(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);
        return World.GetTileAt(x, y);
    }
}
