using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeteorShower3D : MonoBehaviour
{
    public GameObject[] meteorPrefabs;
    public Button showerButton;
    public float meteorCount = 100f;
    public float spawnRangeX = 50f;
    public float spawnRangeZ = 50f;
    public float spawnHeight = 20f;
    public float fallSpeed = 5f;
    public float fallAngle = 30f; // Angle in degrees
    public float destroyDelay = 2f; // Time delay before destroying the meteor after collision
    public Vector3 centralPosition = new Vector3(0, 0, 0);
    public float spawnRadius = 20f;
    private bool canPress = true;

    private bool isButtonPressed = false;

    void Start()
    {
        showerButton.onClick.AddListener(RainMeteors);
    }

    public void RainMeteors()
    {
        StartCoroutine(SpawnMeteors());
        StartCoroutine(Cooldown());
        isButtonPressed = true;
    }

    IEnumerator Cooldown() {
        if (isButtonPressed) {
            Debug.Log("Button is pressed!");
            canPress = false;
            yield return new WaitForSeconds(5f);
        }
        
        canPress = true;
    }

IEnumerator SpawnMeteors()
{
    if (canPress == true)
    {
        for (int i = 0; i < meteorCount; i++)
        {
            // Calculate a random position within the radius
            float angle = Random.Range(0, Mathf.PI * 2);
            float distance = Random.Range(0, spawnRadius);
            Vector3 spawnPosition = new Vector3(
                centralPosition.x + distance * Mathf.Cos(angle),
                spawnHeight,
                centralPosition.z + distance * Mathf.Sin(angle)
            );

            GameObject meteorPrefab = meteorPrefabs[Random.Range(0, meteorPrefabs.Length)];
            GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

            // Calculate the velocity vector based on the fall angle
            float angleInRadians = fallAngle * Mathf.Deg2Rad;
            Vector3 velocity = new Vector3(Mathf.Sin(angleInRadians) * fallSpeed, -fallSpeed, Mathf.Cos(angleInRadians) * fallSpeed);
            
            meteor.GetComponent<Rigidbody>().velocity = velocity;
            yield return new WaitForSeconds(0.2f);
        }
    }
}

}



