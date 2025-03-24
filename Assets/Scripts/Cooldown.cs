using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonCooldown : MonoBehaviour
{
    public Button myButton;
    public GameObject spaceship;
    public int maxPresses = 8;
    private int currentPresses = 0;
    private bool isCooldown = false;
    private float cooldownTime = 2.0f;

    private float cooldownTime2 = 3.0f;

    private Rigidbody rb;

    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick); // Add listener in Start
        rb = spaceship.GetComponent<Rigidbody>();
    }

    void Update() {

    }

    public void OnButtonClick()
    {
        currentPresses++;
        Debug.Log("currentPresses: " + currentPresses);
        if (!isCooldown)
        {
            StartCoroutine(ButtonClickCooldown());
        }

        if (currentPresses >= maxPresses) {
            Destroy(myButton);
            Debug.Log("Max button pressed");
        }
    }

    IEnumerator ButtonClickCooldown()
    {
        isCooldown = true;
        myButton.interactable = false;

        yield return new WaitForSeconds(cooldownTime);
        myButton.interactable = true;
        isCooldown = false;
    }
}
