using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    public int food = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Eat()
    {
        food = food - 1;
        if(food == 0)
        {
            Destroy(gameObject);
        }
    }

}
