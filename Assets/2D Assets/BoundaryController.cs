using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    public Transform terrain; // Assign your terrain's Transform
    public Vector2 offset = new Vector2(0.5f, 0.5f); // Adjust for padding around the bounds

    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        var terrainBounds = terrain.GetComponent<SpriteRenderer>().bounds;

        minBounds = terrainBounds.min + (Vector3)offset;
        maxBounds = terrainBounds.max - (Vector3)offset;
    }

    void Update()
    {
        // Restrict character's position within the bounds
        var clampedX = Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x);
        var clampedY = Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}

