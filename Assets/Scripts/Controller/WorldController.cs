using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; }

    public World World { get; private set; }

    // Use this for initialization
    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("Should not be multiple instances of WorldController.");
            return;
        }

        Instance = this;
        World = new World();

        // Center the camera
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
    }

    void Update()
    {
        // TODO: Add pause/unpause, speed controls, etc.
        World.Update(Time.deltaTime);
    }

    public void SetupPathfindingExample()
    {
        World.SetupPathfindingExample();
    }


    public Tile GetTileAtWorldCoords(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);
        return World.GetTileAt(x, y);
    }
}
