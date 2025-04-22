using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures;
using System;
using Random = UnityEngine.Random;

public class BaseRepair : MonoBehaviour
{
    private LongPressGesture longPressGesture;
    private PressGesture pressGesture;
    private MeshRenderer rnd;
    public Material rustMaterial;
    public Material oldMaterial;
    private Coroutine materialCoroutine;
    private Coroutine checkRustCoroutine;
    private bool isPressed;
    public GameObject warning;
    // Start is called before the first frame update

    void Start()
    {
        warning.SetActive(false);
        GetComponent<Renderer>().material = oldMaterial;
        StartNewCoroutine();
    }

    void StartNewCoroutine() {
        if (materialCoroutine != null) {
            StopCoroutine(materialCoroutine);
        }
        materialCoroutine = StartCoroutine(ChangeMaterialRoutine());
    }
    private void OnEnable()
    {
        rnd = GetComponent<MeshRenderer>();
        longPressGesture = GetComponent<LongPressGesture>();
        pressGesture = GetComponent<PressGesture>();

        longPressGesture.StateChanged += longPressedHandler;
        pressGesture.Pressed += pressedHandler;
    }

    private void OnDisable()
    {
        longPressGesture.StateChanged -= longPressedHandler;
        pressGesture.Pressed -= pressedHandler;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ResetPress()
    {
        isPressed = false;
    }

    private void longPressedHandler(object sender, GestureStateChangeEventArgs e) {
		if (e.State == Gesture.GestureState.Recognized) {
			isPressed = true;
			Debug.Log("the Thing is touched.");
            GetComponent<Renderer>().material = oldMaterial;
            warning.SetActive(false);
            StartNewCoroutine();

            // Stop rust check when material is repaired
            if (checkRustCoroutine != null)
            {
                StopCoroutine(checkRustCoroutine);
                Debug.Log("Stopped rust check since material was reset!");
            }
		}
	}

	private void pressedHandler(object sender, EventArgs e) {
        if (isPressed) return;
        isPressed = true;
        Invoke("ResetPress", 0.1f);
	}

IEnumerator CheckRustMaterialRoutine()
{
    Debug.Log("CheckRustMaterialRoutine started!");
    
    //float totalTime = 0f;

    while (GetComponent<Renderer>().material = rustMaterial)
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds before each deduction
        //totalTime += 5f;

        //Debug.Log("Checking material after " + totalTime + " seconds...");
        
        // If the material is repaired, stop the coroutine
        if (GetComponent<Renderer>().material.name.Contains(oldMaterial.name))
        {
            Debug.Log("Material was repaired! Stopping resource deductions.");
            yield break;
        }

        // Deduct resources every 5 seconds
        Debug.Log("Rust material is still active. Deducting resources...");
        GameManager.instance.ChangeCollectedWater(-5);
        GameManager.instance.AddCollectedIron(-5);
        GameManager.instance.AddCollectedRocks(-5);
    }
    
    Debug.Log("Rust material remained for full duration. Resources deducted 3 times!");
}

    IEnumerator ChangeMaterialRoutine() {
        while (true) {
            float randomTime = Random.Range(10f, 15f);
            yield return new WaitForSeconds(randomTime);
            GetComponent<Renderer>().material = rustMaterial;
            warning.SetActive(true);
            if (checkRustCoroutine != null) {
                StopCoroutine(checkRustCoroutine);
            } else {
                Debug.Log("It is null");
            }
            checkRustCoroutine = StartCoroutine(CheckRustMaterialRoutine());
            Debug.Log("Check rust coroutine has started!");
        }
    }
}
