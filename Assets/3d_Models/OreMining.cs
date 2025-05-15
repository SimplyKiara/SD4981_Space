using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures;
using System;

public class OreMining : MonoBehaviour
{
    public float Power = 5.0f;

    private LongPressGesture longPressGesture;
    private PressGesture pressGesture;
    private MeshRenderer rnd;
    private Color originalColor;

    private bool growing = false;
    private float growingTime = 0;
    private bool isPressed;
    private Vector3 prefabScale;
    private ResourceZone currentZone;

    // spawning directions
    private Vector3[] directions =
    {
        new Vector3(5, 7, -5),
        new Vector3(10, 9, -10),
        new Vector3(5, 9, -5),
        new Vector3(-5, 7, -5),
        new Vector3(8, 7, -8),
        new Vector3(-8, 7, -8)
    };


    private void Start()
    {
        originalColor = GetComponent<MeshRenderer>().material.color; // Save original color
        prefabScale = transform.localScale;
    }

    private void OnEnable()
    {
        rnd = GetComponent<MeshRenderer>();
        longPressGesture = GetComponent<LongPressGesture>();
        pressGesture = GetComponent<PressGesture>();

        longPressGesture.StateChanged += longPressedHandler;
        pressGesture.Pressed += pressedHandler;

        if (pressGesture == null)
        {
            Debug.LogError("PressGesture component missing!");
        }
    }

    private void OnDisable()
    {
        longPressGesture.StateChanged -= longPressedHandler;
        pressGesture.Pressed -= pressedHandler;
    }

    private void Update()
    {
        if (growing)
        {
            // Ensure growingTime increases consistently over time
            growingTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(growingTime / 2f); // Adjust timing as needed
            rnd.material.color = Color.Lerp(Color.white, Color.red, lerpValue);
        }
    }

    private void startGrowing()
    {
        growing = true;
        growingTime = 0; // Reset when long press starts
    }

    private void stopGrowing()
    {
        growing = false;
        growingTime = 0;
        rnd.material.color = originalColor;
    }

    private void ResetPress()
    {
        isPressed = false;
    }

    // Short press to collect materials
    private void pressedHandler(object sender, EventArgs e)
    {
        if (isPressed) return;
        isPressed = true;
        Invoke("ResetPress", 0.1f);

        if (transform.localScale.x < 0.4f && currentZone != null) // Ensure ore is collectible
        {
            GameManager manager = currentZone.gameManager;
            if (manager != null)
            {
                if (gameObject.tag == "Iron ore") manager.AddCollectedIron(1);
                else if (gameObject.tag == "Lunar rocks") manager.AddCollectedRocks(1);
                else if (gameObject.tag == "Ice") manager.ChangeCollectedWater(0.5f);
            }
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No ores collected");
        }
    }

    // Long press for spawning smaller materials
    private void longPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        if (e.State == Gesture.GestureState.Recognized)
        {
            startGrowing();
            if (transform.localScale.x > 0.5f) // size larger than 0.5f => parent ore
            {
                int spawnCount = UnityEngine.Random.Range(3, 6); // Randomizing between 3 to 6

                for (int i = 0; i < spawnCount; i++)
                {
                    GameObject obj = Instantiate(gameObject); // Create a new object
                    Transform cube = obj.transform;

                    cube.parent = transform.parent;
                    cube.name = "Collectible";
                    cube.localScale = 0.3f * transform.localScale;
                    cube.position = transform.position + directions[i % directions.Length] * 0.25f; // Keep them closer to the parent
                    cube.GetComponent<Rigidbody>().AddForce(Power * UnityEngine.Random.insideUnitSphere, ForceMode.Impulse);
                    cube.GetComponent<Renderer>().material.color = originalColor;
                    cube.gameObject.isStatic = false;

                    if (cube.GetComponent<Rigidbody>() is Rigidbody rb)
                    {
                        rb.constraints = RigidbodyConstraints.None;
                    }
                }
                if (gameObject.tag == "Ice") // Destroy parent if it's Ice
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (e.State == Gesture.GestureState.Failed)
        {
            stopGrowing();
        }
    }

    public void SetCurrentZone(ResourceZone zone)
    {
        currentZone = zone;
    }

    public void ClearCurrentZone()
    {
        currentZone = null;
    }
}