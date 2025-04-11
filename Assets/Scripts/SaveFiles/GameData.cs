using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public float maxWater = 50f;
    public float maxResources = 100f;

    // structuer of saved data
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
