using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    GameObject[] pheromones = new GameObject[0];
    GameObject[] ants = new GameObject[0];
    public GameObject ant;
    public int numAnts = 40;
    public float explorerChance = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ants.Length < numAnts)
        {
            GameObject a = Instantiate(ant);
            a.transform.position = Vector3.zero;
            if (Random.Range(0.0f,1.0f) < explorerChance)
            {
                a.GetComponent<Ant>().explorer = true;
            }
        }
    }

    void LateUpdate()
    {
        pheromones = GameObject.FindGameObjectsWithTag("Pheromone");
        ants = GameObject.FindGameObjectsWithTag("Ant");
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
