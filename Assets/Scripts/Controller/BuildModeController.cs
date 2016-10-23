using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

public class BuildModeController : MonoBehaviour
{

    bool _buildModeIsFurniture = false;
    TileType _buildModeTile = TileType.Floor;
    private string _buildModeFurnitureType;

    // Use this for initialization
    void Start()
    {
    }

    public void DoBuild(Tile tile)
    {
        if (_buildModeIsFurniture)
        {
            if (WorldController.Instance.World.IsFurniturePlacementValid(_buildModeFurnitureType, tile) == false
                || tile.PendingFurnatureJob != null)
            {
                return;
            }

            Job job = new Job(tile, _buildModeFurnitureType);
            string furnatureType = _buildModeFurnitureType;
            job.JobComplete += (j) =>  WorldController.Instance.World.PlaceFurniture(furnatureType, tile);

            WorldController.Instance.World.JobQueue.Enqueue(job);

        }
        else
        {
            tile.TileType = _buildModeTile;
        }
    }

    public void SetMode_BuildFloor()
    {
        _buildModeIsFurniture = false;
        _buildModeTile = TileType.Floor;
    }

    public void SetMode_Bulldoze()
    {
        _buildModeIsFurniture = false;
        _buildModeTile = TileType.Empty;
    }

    public void SetMode_BuildFurniture(string furnitureType)
    {
        _buildModeIsFurniture = true;
        _buildModeFurnitureType = furnitureType;
    }
}
