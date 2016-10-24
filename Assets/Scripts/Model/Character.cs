using UnityEngine;
using System.Collections;
using System;
using System.Xml;

public class Character {

    public float X
    {
        get
        {
            return Mathf.Lerp(CurrTile.X, NextTile.X, movementPercentage);
        }
    }

    public float Y
    {
        get
        {
            return Mathf.Lerp(CurrTile.Y, NextTile.Y, movementPercentage);
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
    public Tile NextTile { get; protected set; } // Next tile in pathfinding
    float movementPercentage;

    float speed = 5f; // Moves 2 tiles per second
    Pathfinding.AStar _pathAStar;

    Job myJob;

    public Character(Tile tile)
    {
        CurrTile = DestTile = NextTile = tile;
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
        Update_DoJob(deltaTime);
        Update_HandleMovement(deltaTime);
    }

    void Update_DoJob(float deltaTime)
    {
        if (myJob == null)
        {
            myJob = CurrTile.World.JobQueue.Dequeue();

            if (myJob != null)
            {
                // TODO: Check to see if job is reachable

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
    }

    private void Update_HandleMovement(float deltaTime)
    {
        if (CurrTile == DestTile)
        {
            _pathAStar = null;
            return; // No movement needed
        }

        if (NextTile == null || NextTile == CurrTile)
        {
            if (_pathAStar == null)
            {
                _pathAStar = new Pathfinding.AStar(CurrTile.World, CurrTile, DestTile);
            }

            NextTile = _pathAStar.Dequeue();
            if (NextTile == null)
            {
                Debug.LogError("Character::HandleMovement could not find path to destination");
                AbandonJob();
                return;
            }
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(CurrTile.X - NextTile.X, 2) + Mathf.Pow(CurrTile.Y - NextTile.Y, 2));

        float distThisFrame = speed * deltaTime;

        float percThisFrame = distThisFrame / distToTravel;

        movementPercentage += percThisFrame;

        if (movementPercentage >= 1)
        {
            // Reached destination
            CurrTile = NextTile;
            movementPercentage = 0;
        }

        RaiseCharacterChanged();
    }

    public void AbandonJob()
    {
        NextTile = DestTile = CurrTile;
        _pathAStar = null;
        if (myJob != null)
        {
            myJob.JobComplete -= OnJobFinished;
            myJob.JobCanceled -= OnJobFinished;
            CurrTile.World.JobQueue.Enqueue(myJob);
        }

        myJob = null;
    }

    public void OnJobFinished(Job job)
    {
        if (job != myJob)
        {
            Debug.LogError("Character being told about job that isn't his.  You forgot to unregister something.");
        }
        myJob = null;

    }

    #region Save/Load Code

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", CurrTile.X.ToString());
        writer.WriteAttributeString("Y", CurrTile.Y.ToString());
    }

    public void ReadXml(XmlReader reader)
    {

    }

    #endregion
}
