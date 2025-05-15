using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    public Transform nonBlockCanvas;
    public GameObject clickHerePrefab;

    public void CreateCircle(Transform locationToSpawn)
    {
        GameObject spawnedPrefabs = Instantiate(clickHerePrefab, nonBlockCanvas);
        spawnedPrefabs.transform.SetParent(nonBlockCanvas);
        spawnedPrefabs.transform.localPosition = locationToSpawn.localPosition;
        // return spawnedPrefabs;
    }
    public void MountUItoObject(Transform obj)
    {

    }
}
