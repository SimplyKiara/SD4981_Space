using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    public Transform nonBlockCanvas;
    public GameObject clickHerePrefab;
    public Camera sceneCamera;


    public void CreateCircle(Transform referenceObj)
    {
        GameObject spawnedPrefabs = Instantiate(clickHerePrefab, nonBlockCanvas);
        spawnedPrefabs.transform.SetParent(nonBlockCanvas);
        spawnedPrefabs.GetComponent<ClickUI>().refObj = referenceObj;

        Vector3 screenPosition = sceneCamera.WorldToScreenPoint(referenceObj.position);
        if (referenceObj.name == "Supplies")
        {
            screenPosition = sceneCamera.WorldToScreenPoint(referenceObj.parent.transform.position);
        }
        Vector3 correctPosition = new Vector3(screenPosition.x, screenPosition.y, 1);
        if (referenceObj.name == "coveringCrater")
        {
            correctPosition = new Vector3(screenPosition.x - 100, screenPosition.y + 150, 1);
        }
        spawnedPrefabs.transform.position = correctPosition;
        Debug.Log($"screen position {screenPosition} ,prefab position: {correctPosition}");
    }
}
