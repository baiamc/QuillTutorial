using UnityEngine;
using System.Collections;
using System;

public class MouseController : MonoBehaviour {

    public GameObject CircleCursor;

	// Use this for initialization
	void Start () {
	
	}

    Vector3 _currentMousePosition;
    Vector3 _lastFrameMousePosition;
	
	// Update is called once per frame
	void Update () {
        _currentMousePosition = GetMouseWorldCoords();

        CircleCursor.transform.position = _currentMousePosition;

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            var diff = _lastFrameMousePosition - _currentMousePosition;
            Camera.main.transform.Translate(diff);
        }

        _lastFrameMousePosition = GetMouseWorldCoords();
	}

    private Vector3 GetMouseWorldCoords()
    {
        var vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vector.z = 0;
        return vector;
    }
}
