using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    public GameObject[] buttonList;
    public Text statusText;

    private bool[] active;

    private void Start()
    {
        foreach (var button in buttonList)
        {
            button.SetActive(false);
        }

        statusText.text = "No tasks availble: Wait or refresh.";
    }

    public void ActivateButton(string buttonName)
    {
        foreach (var button in buttonList)
        {
            button.SetActive(false);
        }

        int activeTasks = 0;

        // Activate the button that matches the given name
        foreach (var button in buttonList)
        {
            if (button.name == buttonName)
            {
                button.SetActive(true);
                Debug.Log($"Button activated: {button.name}");
                activeTasks++;
                break;
            }
        }

        if (activeTasks >= 0)
        {
            statusText.text = "Task(s) available:";
        }
        else
        {
            statusText.text = "No tasks availble: Wait or refresh.";
        }
        
    }

    public void OnClick(GameObject clickedButton)
    {
        int index = System.Array.IndexOf(buttonList, clickedButton);
        if (index != -1)
        {
            Debug.Log($"Button clicked: {clickedButton.name}, Index: {index}");

            switch (index)
            {
                case 1:
                    SceneManager.LoadScene("Planting");
                    break;
                case 2:
                    SceneManager.LoadScene("Harvest");
                    break;
                case 3:
                    SceneManager.LoadScene("IceMining_P1");
                    break;
                default:
                    Debug.Log("No scene provided yet"); 
                    break;
            }
        }
        else
        {
            Debug.Log("Button not found in the list.");
        }
    }
}
