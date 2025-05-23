using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// / Structure of saved data
/// </summary>

[System.Serializable]
public class GameData
{
    public int ironOre;
    public int rocks;
    public float veggies;
    public float water;
    public int solarPanelsSet;

    public float currentResources;
    public float resourcePercentage;

    public static float maxWater = 30f;
    public static float maxResources = 80f;

    public GameData()
    {
        this.ironOre = 0;
        this.rocks = 0; 
        this.veggies = 0;
        this.water = 5f;
        this.solarPanelsSet = 0;

        this.currentResources = ironOre + rocks + veggies + water;
        this.resourcePercentage = currentResources / maxResources * 100;
    }
}
