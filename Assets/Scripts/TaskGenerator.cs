using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TaskGenerator : MonoBehaviour
{
    private string[] tasks = { "Rocket Landing", "Planting Crops", "Crop Harvesting", "Ice Mining" };

    public string GetRandomTask()
    {
        int randomIndex = Random.Range(0, tasks.Length);
        return tasks[randomIndex];
    }
}
