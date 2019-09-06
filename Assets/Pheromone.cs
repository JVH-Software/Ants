using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{

    public enum PheromoneType {Food,Home,Self}

    public int startingStrength = 100;
    public int dropRate = 5;
    public int strength = -1;
    private int colony;
    public PheromoneType type;

    void Start()
    {
        if (strength < 0)
        {
            strength = startingStrength;
        }
    }

    public int GetStrength()
    {
        return strength;
    }

    public int GetColony()
    {
        return colony;
    }

    public void SetColony(int colony)
    {
        this.colony = colony;
    }

    public PheromoneType GetPheromoneType()
    {
        return type;
    }

    public int GetStartingStrength()
    {
        return startingStrength;
    }

    public void Update()
    {
        strength -= dropRate;
        if(strength <= 0)
        {
            Destroy(gameObject);
        }
    }

}
