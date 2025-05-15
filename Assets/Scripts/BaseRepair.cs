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
    public GameObject instructions;
    // Start is called before the first frame update

    void Start()
    {
        warning.SetActive(false);
        instructions.SetActive(false);
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        else
        {
            instructions.SetActive(true);
        }
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
        // Disable gestures
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

        while (true)
        {
            yield return new WaitForSeconds(5f);

            // If GameObject is inactive, stop deductions
            if (!gameObject.activeInHierarchy)
            {
                Debug.Log("Base structure is inactive, stopping rust check!");
                yield break;
            }

            // If the material is repaired, stop deductions
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
    }

    IEnumerator ChangeMaterialRoutine()
    {
        while (true)
        {
            float randomTime = Random.Range(10f, 15f);
            yield return new WaitForSeconds(randomTime);

            // **Check if GameObject is still active before applying rust**
            if (!gameObject.activeInHierarchy)
            {
                Debug.Log("Base structure became inactive! Not applying rust.");
                yield break;
            }

            GetComponent<Renderer>().material = rustMaterial;
            if (gameObject.activeInHierarchy) {
                warning.SetActive(true);
            }

            Debug.Log("Rust material applied, starting countdown...");

            // Stop previous rust check
            if (checkRustCoroutine != null)
            {
                StopCoroutine(checkRustCoroutine);
            }

            checkRustCoroutine = StartCoroutine(CheckRustMaterialRoutine());
            Debug.Log("Check rust coroutine has started!");
        }
    }
}
