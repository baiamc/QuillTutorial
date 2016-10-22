using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

    private World _world;

    float soundCooldown = 0;

    // Use this for initialization
    void Start () {
        _world = WorldController.Instance.World;
        _world.FurnitureCreated += OnFurnitureCreated;
        _world.TileChanged += OnTileChanged;
	}
	
	// Update is called once per frame
	void Update () {
        soundCooldown -= Time.deltaTime;
	}

    void OnTileChanged(Tile tile)
    {
        // FIXME
        if (soundCooldown > 0)
        {
            return;
        }
        var clip = Resources.Load<AudioClip>("Sounds/Floor_OnCreated");
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        soundCooldown = 0.1f;
    }

    void OnFurnitureCreated(Furniture furn)
    {
        // FIXME
        if (soundCooldown > 0)
        {
            return;
        }
        var clip = Resources.Load<AudioClip>("Sounds/" + furn.FurnitureType + "_OnCreated");

        if (clip == null)
        {
            // No specific sounds for this Furnature, use a default sound
            clip = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");
        }

        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        soundCooldown = 0.1f;

    }
}
