using UnityEngine;
using System.Collections;

/// <summary>
///     This class holds info for a queued up job
/// </summary>
public class Job {

    public Tile Tile { get; protected set; }
    public float JobTime { get; protected set; }

    public delegate void JobCompleteHandler(Job job);
    public event JobCompleteHandler JobComplete;

    public delegate void JobCanceledHandler(Job job);
    public event JobCanceledHandler JobCanceled;

    public string JobObjectType { get; protected set; }

    public Job(Tile tile, string jobObjectType, float jobTime = 1f)
    {
        Tile = tile;
        JobObjectType = jobObjectType;
        JobTime = jobTime;
    }

    private void RaiseJobComplete()
    {
        if (JobComplete != null)
        {
            JobComplete(this);
        }
    }

    private void RaiseJobCanceled()
    {
        if (JobCanceled != null)
        {
            JobCanceled(this);
        }
    }

    public void DoWork(float workTime)
    {
        JobTime -= workTime;
        if (JobTime <= 0)
        {
            RaiseJobComplete();
        }
    }

    public void CancelJob()
    {
        RaiseJobCanceled();
    }
}
