using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    GameObject[] pheromones = new GameObject[0];
    GameObject[] ants = new GameObject[0];

    // Start is called before the first frame update
    void Start()
    {
        ants = GameObject.FindGameObjectsWithTag("Ant");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        pheromones = GameObject.FindGameObjectsWithTag("Pheromone");
    }

    public GameObject[] GetPheromones()
    {
        return pheromones;
    }

    public GameObject[] GetAnts()
    {
        return ants;
    }
}
