using System;
using UnityEngine;
using UnityEngine.UI;

public class PlantingController : MonoBehaviour
{
    public Text ResourcesText;
    public Text AnnounceText;
    public WallConnection wallConnection;
    public GameManager groupManager;

    private string groupName = "";
    private bool called = false;
    private float currentWater;
    private float maxWater;
    private string gpName;

    private void OnEnable()
    {
        AnnounceText.text = "";
        called = true;
    }

    void Start()
    {
        if (groupManager == null)
        {
            Debug.LogError("SupplyPanel: Manager not assigned!");
        }

        // Initialise group name
        if (groupManager.name == "Gp1_GameManager")
        {
            groupName = "Group 1";
        }
        else if (groupManager.name == "Gp2_GameManager")
        {
            groupName = "Group 2";
        }
        else if (groupManager.name == "Gp3_GameManager")
        {
            groupName = "Group 3";
        }
        else
        {
            Debug.LogError("Game Manager not identified correctly!");
        }
    }
    
    // Update UI
    void Update()
    {
        if (groupManager != null)
        {
            currentWater = groupManager.water;
            maxWater = groupManager.waterCap;

            ResourcesText.text = $"Water resources: {currentWater} / {maxWater}";
        }
    }

    // Unlock planting if enough resources
    public void callPlanting()
    {
        if ((currentWater >= 8) && (groupName != null))
        {
            AnnounceText.text = "Action called! Check your tablet.";
            groupManager.ChangeCollectedWater(-8);
            wallConnection.TriggerTask("Planting", groupName);
            Debug.Log("Planting Task called");
        }
        else
        {
            AnnounceText.text = "Not anough resources!";
            Debug.Log("Too little resources");
        }
    }

    public void DeactivatePopUp()
    {
        gameObject.SetActive(false);
    }
}

