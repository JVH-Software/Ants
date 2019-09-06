using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(LineRenderer))]
public class DisplayRadius : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;

    public bool circle = true;
    [Range(0, 50)]
    public float xradius = 15;
    [Range(0, 50)]
    public float yradius = 15;
    LineRenderer line;

    public float drawSpeed = 0.3f;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = (segments + 1);
        line.useWorldSpace = false;

        StartCoroutine(CreatePoints());
    }

    IEnumerator CreatePoints()
    {
        if (circle == true)
            yradius = xradius;

        float x;
        float z;

        float change = 2 * (float)Math.PI / segments;
        float angle = 0;

        x = Mathf.Sin(angle) * xradius;
        line.SetPosition(0, new Vector3(x, 0.5f, 0));

        for (int i = 1; i < (segments); i++)
        {
            x = Mathf.Sin(angle) * xradius;
            z = Mathf.Cos(angle) * yradius;

            yield return new WaitForSeconds(drawSpeed);
            line.SetPosition((int)i, new Vector3(x, 0.5f, z));

            angle += change;
        }
    }
}