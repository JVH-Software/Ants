using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour {

    public int colony = 0;
    public int pheromoneDropFrequency = 100;
    public float speed = 10;
    public float range = 5;

    public GameObject[] pheromones;

    private int pheromoneDropCount = 0;

    public bool hasFood = false;

    private Controller world;
    private Vector3 direction;
    private Rigidbody rigidbody;

    private float foodCount = 1.0f;
    private float homeCount = 0.0f;

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
        transform.rotation = Quaternion.LookRotation(direction);

        //transform.position += direction * speed;
        rigidbody.velocity = direction*speed;
    }

    void UpdateDirection()
    {
        direction = direction * 8.0f;
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
                            direction += (pheromoneObj.transform.position - transform.position).normalized* pheromone.GetStrength() / 1000000.0f;
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
                            direction += (pheromoneObj.transform.position - transform.position).normalized * pheromone.GetStrength() / 1000000.0f;
                        }
                        break;
                    case Pheromone.PheromoneType.Self:
                        if (Vector3.Distance(transform.position, pheromoneObj.transform.position) < 1)
                        {
                            direction += (pheromoneObj.transform.position - transform.position).normalized * pheromone.GetStrength() * 1;
                        }
                        break;
                }
            }
        }

        foreach(GameObject ant in world.GetAnts())
        {
            if (Vector3.Distance(transform.position, ant.transform.position) < 2)
            {
                direction += (transform.position - ant.transform.position).normalized*2;
            }
        } 

        //Top Direction
        direction += topDirection;

        //Randomize slightly
        direction += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * 1.0f;

        direction.y = 0;
        direction.Normalize();
    }

    void UpdatePheromones()
    {
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
        if(hasFood)
        {
            foodCount -= 0.1f;
            if (foodCount < 0)
            {
                foodCount = 0;
            }
        }
        else
        {
            homeCount -= 0.1f;
            if (homeCount < 0)
            {
                homeCount = 0;
            }
        }


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
                        strength = pheromone.GetStartingStrength() * foodCount;
                    }
                    break;
                case Pheromone.PheromoneType.Home:
                    if(!hasFood)
                    {
                        drop = true;
                        strength = pheromone.GetStartingStrength() * homeCount;
                    }
                    break;
                case Pheromone.PheromoneType.Self:
                    drop = true;
                    break;
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
                    foodCount = 1.0f;
                }
                break;
            case "Home":
                if(hasFood && collisionInfo.transform.gameObject.GetComponent<Home>().IsHomeColony(colony))
                {
                    hasFood = false;
                    homeCount = 1.0f;
                }
                break;
        }
    }
}
