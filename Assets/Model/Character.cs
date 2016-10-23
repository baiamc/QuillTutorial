using UnityEngine;
using System.Collections;

public class Character {

    public float X
    {
        get
        {
            return Mathf.Lerp(CurrTile.X, DestTile.X, movementPercentage);
        }
    }

    public float Y
    {
        get
        {
            return Mathf.Lerp(CurrTile.Y, DestTile.Y, movementPercentage);
        }
    }

    public Tile CurrTile { get; protected set; }
    public Tile DestTile { get; protected set; }
    float movementPercentage;

    float speed = 2f; // Moves 2 tiles per second

    public Character(Tile tile)
    {
        CurrTile = DestTile = tile;
    }

    public void SetDestination(Tile tile)
    {
        if (CurrTile.IsNeighbour(tile) == false)
        {
            Debug.Log("Character::SetDestination -- Destination tile is not a neighbour of the current tile.");
        }

        DestTile = tile;
        movementPercentage = 0f;
    }

    public void Update(float deltaTime)
    {
        if (CurrTile == DestTile)
        {
            return;
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(CurrTile.X - DestTile.X, 2) + Mathf.Pow(CurrTile.Y - DestTile.Y, 2));

        float distThisFrame = speed * deltaTime;

        float percThisFrame = distToTravel / distThisFrame;

        movementPercentage += percThisFrame;

        if (movementPercentage >= 1)
        {
            // Reached destination
            CurrTile = DestTile;
            movementPercentage = 0;
        }
    }
}
