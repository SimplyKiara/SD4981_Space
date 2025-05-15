using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCraterUI : MonoBehaviour
{
    public GameObject StartingUI;
    public GameObject[] CraterUI;
    public GameObject[] RocketUI;

    public GameManager gameManager1;
    public GameManager gameManager2;
    public GameManager gameManager3;

    private bool suppliesPanelFound = false;
    private bool rocketUIShown = false;
    private bool startUiDestroyed = false;

    void Start()
    {
        SetUIState(CraterUI, false);
        SetUIState(RocketUI, false);
    }

    void Update()
    {
        // Monitor StartingUI destruction
        if (StartingUI == null && !startUiDestroyed)
        {
            Debug.Log("Starting UI has been destroyed!");
            SetUIState(CraterUI, true);
            startUiDestroyed = true;
        }

        // Check for SuppliesPanel only once
        if (!suppliesPanelFound && GameObject.Find("SuppliesPanel") != null)
        {
            Debug.Log("Supplies Panel has been found for the first time!");
            suppliesPanelFound = true;
        }

        // Check if SuppliesPanel has been destroyed
        if (suppliesPanelFound && !rocketUIShown && GameObject.Find("SuppliesPanel") == null)
        {
            Debug.Log("Supplies Panel has been destroyed, showing RocketUI!");
            SetUIState(RocketUI, true);
            rocketUIShown = true;
        }

        // Check if any GameManager's resources drop below 10
        if (!rocketUIShown && ShouldShowRocketUI())
        {
            Debug.Log("A GameManager's resources dropped below 10! Showing RocketUI.");
            SetUIState(RocketUI, true);
            rocketUIShown = true; // Prevent repeated activation
        }
    }

    bool ShouldShowRocketUI()
    {
        return IsResourceLow(gameManager1) || IsResourceLow(gameManager2) || IsResourceLow(gameManager3);
    }

    bool IsResourceLow(GameManager manager)
    {
        return manager != null &&
               (manager.ironOre < 10 ||
                manager.rocks < 10 ||
                manager.water < 10);
    }

    void SetUIState(GameObject[] uiElements, bool state)
    {
        foreach (GameObject obj in uiElements)
        {
            obj.SetActive(state);
        }
    }
}