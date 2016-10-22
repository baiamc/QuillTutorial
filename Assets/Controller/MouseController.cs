using UnityEngine;
using System.Collections.Generic;
using System;

public class MouseController : MonoBehaviour
{

    public GameObject CircleCursor;

    // Use this for initialization
    void Start()
    {

    }

    Vector3 _currentMousePosition;
    Vector3 _lastFrameMousePosition;
    Vector3 _leftClickDragStart;

    // Update is called once per frame
    void Update()
    {
        _currentMousePosition = GetMouseWorldCoords();
        HandleCursorPosition();
        HandleLeftClick();
        HandleMouseScrolling();

        _lastFrameMousePosition = GetMouseWorldCoords();
    }

    private void HandleCursorPosition()
    {
        var tile = GetTileAtWorldCoords(_currentMousePosition);
        if (tile == null)
        {
            CircleCursor.SetActive(false);
            return;
        }
        CircleCursor.transform.position = new Vector3(tile.X, tile.Y);
        CircleCursor.SetActive(true);
    }

    private void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _leftClickDragStart = _currentMousePosition;
        }
        if (Input.GetMouseButtonUp(0))
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
                    if (tile == null)
                    {
                        return;
                    }

                    tile.TileType = TileType.Floor;

                }
            }
        }
    }

    private void HandleMouseScrolling()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            var diff = _lastFrameMousePosition - _currentMousePosition;
            Camera.main.transform.Translate(diff);
        }
    }

    private Vector3 GetMouseWorldCoords()
    {
        var vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vector.z = 0;
        return vector;
    }

    private Tile GetTileAtWorldCoords(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);
        return WorldController.Instance.World.GetTileAt(x, y);
    }
}
