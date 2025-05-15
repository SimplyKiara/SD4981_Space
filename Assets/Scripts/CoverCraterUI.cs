using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCraterUI : MonoBehaviour
{
    public GameObject StartingUI;
    public GameObject[] CraterUI;
    public GameObject gp1_RocketUI;
    public GameObject gp2_RocketUI;
    public GameObject gp3_RocketUI;

    public GameManager gameManager1;
    public GameManager gameManager2;
    public GameManager gameManager3;

    private bool suppliesPanelFound_1 = false;
    private bool suppliesPanelFound_2 = false;
    private bool suppliesPanelFound_3 = false;
    private bool rocketUIShown_1 = false;
    private bool rocketUIShown_2 = false;
    private bool rocketUIShown_3 = false;
    private bool startUiDestroyed = false;

    void Start()
    {
        SetUIState(CraterUI, false);
        gp1_RocketUI.SetActive(false);
        gp2_RocketUI.SetActive(false);
        gp3_RocketUI.SetActive(false);
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
        if (!suppliesPanelFound_1 && GameObject.FindWithTag("gp1_SuppliesPanel") != null)
        {
            Debug.Log("Supplies Panel has been found for the first time!");
            suppliesPanelFound_1 = true;
        }

        if (!suppliesPanelFound_2 && GameObject.FindWithTag("gp2_SuppliesPanel") != null)
        {
            Debug.Log("Supplies Panel has been found for the first time!");
            suppliesPanelFound_2 = true;
        }
        
        if (!suppliesPanelFound_3 && GameObject.FindWithTag("gp3_SuppliesPanel") != null)
        {
            Debug.Log("Supplies Panel has been found for the first time!");
            suppliesPanelFound_3 = true;
        }

        // Check if SuppliesPanel has been destroyed
        if (suppliesPanelFound_3 && !rocketUIShown_3 && GameObject.FindWithTag("gp3_SuppliesPanel") == null)
        {
            Debug.Log("Supplies Panel has been destroyed, showing RocketUI!");
            gp3_RocketUI.SetActive(true);
            rocketUIShown_3 = true;
        }

        if (suppliesPanelFound_2 && !rocketUIShown_2 && GameObject.FindWithTag("gp2_SuppliesPanel") == null)
        {
            Debug.Log("Supplies Panel has been destroyed, showing RocketUI!");
            gp2_RocketUI.SetActive(true);
            rocketUIShown_2 = true;
        }

        if (suppliesPanelFound_1 && !rocketUIShown_1 && GameObject.FindWithTag("gp1_SuppliesPanel") == null)
        {
            Debug.Log("Supplies Panel has been destroyed, showing RocketUI!");
            gp1_RocketUI.SetActive(true);
            rocketUIShown_1 = true;
        }

        // Check if any GameManager's resources drop below 10
        /*if (!rocketUIShown && ShouldShowRocketUI())
        {
            Debug.Log("A GameManager's resources dropped below 10! Showing RocketUI.");
            SetUIState(RocketUI, true);
            rocketUIShown = true; // Prevent repeated activation
        }*/
    }

    bool ShouldShowRocketUI()
    {
        return IsResourceLow(gameManager1) || IsResourceLow(gameManager2) || IsResourceLow(gameManager3);
    }

    bool IsResourceLow(GameManager manager)
    {
        Debug.Log("Resources are low!");
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