using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameProgressUpdate : MonoBehaviour
{
    public List<Button> buttons;
    public ProgressManager progressManager;

    void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(i == 0);
            int index = i; // Capture the current index
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

    }
    void OnButtonClicked(int index)
    {
        // Update game state
        progressManager.SetGameState((ProgressManager.GameState)index + 1);

        // Hide the clicked button and show the next one
        buttons[index].gameObject.SetActive(false);
        if (index + 1 < buttons.Count)
        {
            buttons[index + 1].gameObject.SetActive(true);
        }
        Debug.Log("current state is: " + progressManager.currentState);
    }

}
