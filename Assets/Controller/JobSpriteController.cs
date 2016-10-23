using UnityEngine;
using System.Collections.Generic;

public class JobSpriteController : MonoBehaviour {

    FurnitureSpriteController _fsc;
    Dictionary<Job, GameObject> _jobGameObjectMap;

    World World { get { return WorldController.Instance.World; } }

	// Use this for initialization
	void Start () {
        _fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
        _jobGameObjectMap = new Dictionary<Job, GameObject>();

        World.JobQueue.JobCreated += OnJobCreated;
        World.JobQueue.JobCanceled += OnJobEnded;
        World.JobQueue.JobCompleted += OnJobEnded;
	}
	
	void OnJobCreated(Job job)
    {
        var jobGo = new GameObject
        {
            name = "JOB_" + job.Tile.X + "_" + job.Tile.Y
        };

        jobGo.transform.position = new Vector3(job.Tile.X, job.Tile.Y);
        jobGo.transform.SetParent(this.transform, true);

        var jobSr = jobGo.AddComponent<SpriteRenderer>();
        jobSr.sprite = _fsc.GetSpriteForFurniture(job.JobObjectType);
        jobSr.sortingLayerName = "Jobs";
        jobSr.color = new Color(0.5f, 1f, 0.5f, 0.25f);

        _jobGameObjectMap.Add(job, jobGo);
    }

    void OnJobEnded(Job job)
    {
        var jobGo = _jobGameObjectMap[job];
        Destroy(jobGo);
    }
}
