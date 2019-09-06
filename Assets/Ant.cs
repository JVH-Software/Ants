using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour {

    public int colony = 0;
    public int pheromoneDropFrequency = 100;
    public float speed = 10;
    public float range = 5;

    public float momentumRatio = 0.5f;
    public float targetRatio = 0.4f;
    public float randomRatio = 0.05f;
    public float hitRatio = 0.05f;

    public GameObject[] pheromones;

    private int pheromoneDropCount = 0;

    public bool hasFood = false;

    private Controller world;
    private Vector3 direction;
    private Rigidbody rigidbody;

    private int foodCount = 0;
    private int homeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("Controller").GetComponent<Controller>();
        direction = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDirection();
        Move();
        UpdatePheromones();
    }

    void Move()
    {
        Vector3 rot = new Vector3(direction.x, 0, direction.z);
        transform.rotation = Quaternion.LookRotation(rot);

        //transform.position += direction * speed;
        rigidbody.velocity = direction*speed;
    }

    void UpdateDirection()
    {

        Vector3 prevDirection = direction;

        // Momentum
        direction = direction * momentumRatio;

        // Target
        int topStrength = 0;
        Vector3 topDirection = Vector3.zero;
        foreach (GameObject pheromoneObj in world.GetPheromones())
        {
            if (Vector3.Distance(transform.position, pheromoneObj.transform.position) < range)
            {
                Pheromone pheromone = pheromoneObj.GetComponent<Pheromone>();
                switch (pheromone.GetPheromoneType())
                {
                    case Pheromone.PheromoneType.Food:
                        if (!hasFood)
                        {
                            if(pheromone.GetStrength() > topStrength)
                            {
                                topStrength = pheromone.GetStrength();
                                topDirection = (pheromoneObj.transform.position - transform.position).normalized;
                            }
                        }
                        break;
                    case Pheromone.PheromoneType.Home:
                        if (hasFood)
                        {
                            if (pheromone.GetStrength() > topStrength)
                            {
                                topStrength = pheromone.GetStrength();
                                topDirection = (pheromoneObj.transform.position - transform.position).normalized;
                            }
                        }
                        break;
                }
            }
        }
        direction += topDirection * targetRatio;

        /*
        foreach(GameObject ant in world.GetAnts())
        {
            if (Vector3.Distance(transform.position, ant.transform.position) < 2)
            {
                direction += (transform.position - ant.transform.position).normalized*2;
            }
        } 
        */


        //Randomize slightly
        direction += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * randomRatio;

        //Wall avoidance
        Vector3 myPosition = transform.position;
        RaycastHit hit;

        if (Physics.Raycast(myPosition, prevDirection, out hit, range))
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                direction += (transform.position - hit.point).normalized * hitRatio;
            }
        }

        //No climbing
        if (direction.y > 0)
        {
            direction.y = 0;
        }

        direction.Normalize();
    }

    void UpdatePheromones()
    {
        if (hasFood)
        {
            foodCount += 1;
        }
        else
        {
            homeCount += 1;
        }

        //Pheromones
        pheromoneDropCount += 1;
        if (pheromoneDropCount >= pheromoneDropFrequency)
        {
            pheromoneDropCount = 0;
            DropPheromones();
        }
    }

    void DropPheromones()
    {

        foreach(GameObject pheromoneObj in pheromones)
        {
            Pheromone pheromone = pheromoneObj.GetComponent<Pheromone>();
            bool drop = false;
            float strength = 0;
            switch (pheromone.GetPheromoneType())
            {
                case Pheromone.PheromoneType.Food:
                    if(hasFood)
                    {
                        drop = true;
                        strength = pheromone.GetStartingStrength() - (pheromone.dropRate * foodCount * 2);
                    }
                    break;
                case Pheromone.PheromoneType.Home:
                    if(!hasFood)
                    {
                        drop = true;
                        strength = pheromone.GetStartingStrength() - (pheromone.dropRate * homeCount * 2);
                    }
                    break;
            }
            if(strength < 0)
            {
                strength = 0;
            }

            if (drop)
            {
                Debug.Log(strength);
                GameObject droppedPheromone = Instantiate(pheromoneObj);
                droppedPheromone.GetComponent<Pheromone>().SetColony(colony);
                droppedPheromone.GetComponent<Pheromone>().strength = (int)strength;
                droppedPheromone.transform.position = transform.position;
            }
            
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {

        switch(collisionInfo.transform.gameObject.tag)
        {
            case "Food":
                if (!hasFood)
                {
                    collisionInfo.transform.gameObject.GetComponent<Food>().Eat();
                    hasFood = true;
                    foodCount = 0;
                }
                break;
            case "Home":
                if(hasFood && collisionInfo.transform.gameObject.GetComponent<Home>().IsHomeColony(colony))
                {
                    hasFood = false;
                    homeCount = 0;
                }
                break;
        }
    }
}
