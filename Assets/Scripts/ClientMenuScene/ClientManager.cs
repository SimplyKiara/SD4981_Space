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

    // Deactivate all buttons first
    private void Start()
    {
        Screen.SetResolution(1728, 1080, FullScreenMode.Windowed);
        foreach (var button in buttonList)
        {
            button.SetActive(false);
        }

        statusText.text = "No tasks available: Wait or refresh.";
    }

    public void Reset()
    {
        foreach (var button in buttonList)
        {
            button.SetActive(false);
        }
    }

    public void ActivateButton(string buttonName)
    {
        int activeTasks = 0;

        // Activate the button that matches the given name
        foreach (var button in buttonList)
        {
            if (button.name == buttonName)
            {
                button.SetActive(true);
                Debug.Log($"Button activated: {button.name}");
                activeTasks++;
                // break;
            }
        }

        // Change text accordingly
        if (activeTasks > 0)
        {
            statusText.text = "Task(s) available:";
        }
        else
        {
            statusText.text = "No tasks available: Wait or refresh.";
        }
    }

    // Change scenes when clicking button
    public void OnClick(GameObject clickedButton)
    {
        int index = System.Array.IndexOf(buttonList, clickedButton);
        if (index != -1)
        {
            Debug.Log($"Button clicked: {clickedButton.name}, Index: {index}");

            switch (index)
            {
                case 0:
                    SceneManager.LoadScene("TabletScene");
                    break;
                case 1:
                    SceneManager.LoadScene("IceMining_P1");
                    break;
                case 2:
                    SceneManager.LoadScene("Planting");
                    break;
                case 3:
                    SceneManager.LoadScene("Harvest");
                    break;
                case 4:
                    SceneManager.LoadScene("IceMining_P2");
                    break;
                default:
                    Debug.Log("Scene(s) not provided yet");
                    break;
            }
        }
        else
        {
            Debug.Log("Button not found in the list.");
        }
    }
}
