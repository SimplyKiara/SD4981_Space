using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextProgressManager : MonoBehaviour
{
    public Text ProgressText;
    public GameObject popUpPanel;

    private int completedCount;
    private int totalCount;
    private bool finished = false;

    GameObject[] objectsWithTag;

    private void Start()
    {
        objectsWithTag = GameObject.FindGameObjectsWithTag("Pots");
        totalCount = objectsWithTag.Length;
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
            PotController potController = obj.GetComponent<PotController>();
            if (potController != null && potController.HaveMud && potController.HaveSeed && potController.Watered)
            {
                completedCount++;
            }
        }

        ProgressText.text = "Pots to be filled: " + (totalCount - completedCount);
    }

    public void CallPanel()
    {
        popUpPanel.SetActive(true);
    }
}
