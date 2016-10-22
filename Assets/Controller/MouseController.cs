using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class MouseController : MonoBehaviour
{

    public GameObject CircleCursorPrefab;

    Vector3 _currentMousePosition;
    Vector3 _lastFrameMousePosition;
    Vector3 _leftClickDragStart;

    List<GameObject> _dragPreviewGameObjects;

    // Use this for initialization
    void Start()
    {
        _dragPreviewGameObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        _currentMousePosition = GetMouseWorldCoords();
        HandleLeftClick();
        HanldeCameraMovement();

        _lastFrameMousePosition = GetMouseWorldCoords();
    }

    private void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _leftClickDragStart = _currentMousePosition;
        }

        // Clean old drag previews
        _dragPreviewGameObjects.ForEach(SimplePool.Despawn);
        _dragPreviewGameObjects.Clear();

        if (Input.GetMouseButton(0))
        {
            foreach (var tile in GetDragArea())
            {
                var go = SimplePool.Spawn(CircleCursorPrefab, new Vector3(tile.X, tile.Y), Quaternion.identity);
                go.transform.SetParent(this.transform);
                _dragPreviewGameObjects.Add(go);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (var tile in GetDragArea())
            {
                tile.TileType = TileType.Floor;
            }
        }
    }

    private IEnumerable<Tile> GetDragArea()
    {
        int startX = Mathf.RoundToInt(Mathf.Min(_leftClickDragStart.x, _currentMousePosition.x));
        int endX = Mathf.RoundToInt(Mathf.Max(_leftClickDragStart.x, _currentMousePosition.x));
        int startY = Mathf.RoundToInt(Mathf.Min(_leftClickDragStart.y, _currentMousePosition.y));
        int endY = Mathf.RoundToInt(Mathf.Max(_leftClickDragStart.y, _currentMousePosition.y));

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                if (tile != null)
                {
                    yield return tile;
                }
            }
        }
    }

    private void HanldeCameraMovement()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            var diff = _lastFrameMousePosition - _currentMousePosition;
            Camera.main.transform.Translate(diff);
        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
    }

    private Vector3 GetMouseWorldCoords()
    {
        var vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vector.z = 0;
        return vector;
    }
}
