using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class JobQueue {

    Queue<Job> _jobQueue;

    public JobQueue()
    {
        _jobQueue = new Queue<Job>();
    }

    public delegate void JobCreatedHandler(Job job);
    public event JobCreatedHandler JobCreated;

    public delegate void JobCanceledHandler(Job job);
    public event JobCanceledHandler JobCanceled;

    public delegate void JobCompletedHandler(Job job);
    public event JobCompletedHandler JobCompleted;

    private void RaiseJobCreated(Job job)
    {
        if (JobCreated != null)
        {
            JobCreated(job);
        }
    }

    private void RaiseJobCanceled(Job job)
    {
        if (JobCanceled != null)
        {
            JobCanceled(job);
        }
    }

    private void RaiseJobComplete(Job job)
    {
        if (JobCompleted != null)
        {
            JobCompleted(job);
        }
    }

    public void Enqueue(Job job)
    {
        job.Tile.PendingFurnatureJob = job;
        _jobQueue.Enqueue(job);
        RaiseJobCreated(job);
        job.JobComplete += OnJobComplete;
        job.JobCanceled += OnJobCanceled;
    }

    public Job Dequeue()
    {
        if (_jobQueue.Count == 0)
        {
            return null;
        }
        return _jobQueue.Dequeue();
    }

    private void OnJobCanceled(Job job)
    {
        RaiseJobCanceled(job);
        job.Tile.PendingFurnatureJob = null;
    }

    private void OnJobComplete(Job job)
    {
        RaiseJobComplete(job);
        job.Tile.PendingFurnatureJob = null;
    }
}
