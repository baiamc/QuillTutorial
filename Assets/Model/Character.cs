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

    public delegate void CharacterChangedHandler(Character character);
    public event CharacterChangedHandler CharacterChanged;

    protected void RaiseCharacterChanged()
    {
        if (CharacterChanged != null)
        {
            CharacterChanged(this);
        }
    }

    public Tile CurrTile { get; protected set; }
    public Tile DestTile { get; protected set; }
    float movementPercentage;

    float speed = 2f; // Moves 2 tiles per second

    Job myJob;

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
        if (myJob == null)
        {
            myJob = CurrTile.World.JobQueue.Dequeue();

            if (myJob != null)
            {
                DestTile = myJob.Tile;
                myJob.JobComplete += OnJobFinished;
                myJob.JobCanceled += OnJobFinished;
            }
        }

        if (CurrTile == DestTile)
        {
            if (myJob == null)
            {
                return;
            }

            myJob.DoWork(deltaTime);
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(CurrTile.X - DestTile.X, 2) + Mathf.Pow(CurrTile.Y - DestTile.Y, 2));

        float distThisFrame = speed * deltaTime;

        float percThisFrame = distThisFrame / distToTravel;

        movementPercentage += percThisFrame;
        
        if (movementPercentage >= 1)
        {
            // Reached destination
            CurrTile = DestTile;
            movementPercentage = 0;
        }

        RaiseCharacterChanged();
    }

    public void OnJobFinished(Job job)
    {
        if (job != myJob)
        {
            Debug.LogError("Character being told about job that isn't his.  You forgot to unregister something.");
        }
        myJob = null;

    }
}
