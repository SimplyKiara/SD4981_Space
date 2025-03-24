using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanelController : MonoBehaviour
{
    public Button myButton;
    public Text description;

    bool returning = false;

    public void TaskOnClick()
    {
        Debug.Log("Item called");
        description.text = "Returning to menu...";
        StartCoroutine(WaitCoroutine());
        SceneManager.LoadScene("ClientMenu");
        myButton.enabled = false;
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(4);
    }
}
