using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCraterUI : MonoBehaviour
{
    public GameObject StartingUI;
    public GameObject CraterUI;
    public GameObject RocketUI;
    private UICloser uICloser;
    private bool suppliesPanelFound = false;
    private bool rocketUIShown = false;

    void Start()
    {
        CraterUI.SetActive(false);
        RocketUI.SetActive(false);
        /*if (buttonHandler != null) {
            buttonHandler.buttonPressedEvent.AddListener(OnButtonPressDetected);
        }*/
    }

    void Update()
    {
        // Monitor StartingUI destruction
        if (StartingUI == null)
        {
            Debug.Log("Starting UI has been destroyed!");
            CraterUI.SetActive(true);
        }

        // Find all active GameObjects
       GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if SuppliesPanel appears in the hierarchy for the first time
            if (!suppliesPanelFound && obj.name.Contains("SuppliesPanel"))
            {
                Debug.Log("Supplies Panel has been found for the first time!");
                suppliesPanelFound = true; // Mark as found
            }

            // Check if SuppliesPanel has been destroyed (and RocketUI hasn't been shown yet)
            if (suppliesPanelFound && !rocketUIShown && GameObject.Find("SuppliesPanel") == null)
            {
                Debug.Log("Supplies Panel has been destroyed, showing RocketUI!");
                RocketUI.SetActive(true); // Activate RocketUI
                rocketUIShown = true;     // Prevent RocketUI from popping up again
            }
        }
    }

    /*void OnButtonPressDetected() {
        Debug.Log("The UI is closed");
        RocketUI.SetActive(true);
    }*/
}