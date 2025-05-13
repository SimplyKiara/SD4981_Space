using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTextManager : MonoBehaviour
{
    public Text ProgressText;
    public GameObject popUpPanel;

    private int completedCount;
    private int totalCount;
    private bool finished = false;

    GameObject[] objectsWithTag;

    private void Start()
    {
        objectsWithTag = GameObject.FindGameObjectsWithTag("Veggies");
        totalCount = objectsWithTag.Length;
        //Debug.Log("Veggies found:" + totalCount);
    }

    void Update()
    {
        UpdateProgressText();

        if (!finished && (completedCount == totalCount))
        {
            finished = true;
            CallPanel();
        }
    }

    void UpdateProgressText()
    {
        completedCount = 0;

        foreach (GameObject obj in objectsWithTag)
        {
            VeggiesController vegController = obj.GetComponent<VeggiesController>();
            if (vegController != null && vegController.HvCollected && vegController.HvWatered)
            {
                completedCount++;
                break;
            }
        }

        ProgressText.text = (totalCount - completedCount) + " pots of plants left";
    }

    public void CallPanel()
    {
        popUpPanel.SetActive(true);
    }
}
