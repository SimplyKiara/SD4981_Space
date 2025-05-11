using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlantingController : MonoBehaviour
{
    public Text ResourcesText;
    public Text AnnounceText;
    public WallConnection wallConnection;
    public GameManager groupManager;
    public string groupName = "Group 1";

    private bool called = false;
    private float currentWater;
    private float maxWater;
    private string gpName;

    void Start()
    {
        AnnounceText.text = "";

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

    void OnEnable()
    {
        if (groupManager != null)
        {
            currentWater = groupManager.water;
            maxWater = groupManager.waterCap;

            ResourcesText.text = $"Water resources: {currentWater} / {maxWater}";
        }

        AnnounceText.text = "";
    }
    
    void Update()
    {
        if (groupManager != null)
        {
            currentWater = groupManager.water;
            maxWater = groupManager.waterCap;

            ResourcesText.text = $"Water resources: {currentWater} / {maxWater}";
        }
    }

    public void callPlanting()
    {
        if (!called)
        {
            if ((currentWater >= 8) && (groupName != null))
            {
                AnnounceText.text = "Action called! Check your tablet.";
                wallConnection.TriggerTask("Planting", groupName);
            }
            else
            {
                AnnounceText.text = "Not anough resources!";
            }
            AnnounceText.text = "Action called! Check your tablet.";
        }
        else
        {
            AnnounceText.text = "Action called before! Finish the task first.";
        }
    }

    public void DeactivatePopUp()
    {
        gameObject.SetActive(false);
    }
}

