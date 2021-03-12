using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipDataClass : MonoBehaviour
{
    public GameObject Grid;
    public GameObject Floor;
    public GameObject Wall;

    // Start is called before the first frame update
    void Start()
    {
        //DEBUG THIS
        SetData();

        // hopefully this at least sorta works
        //it dont



    }

    public void SetData()
    {
        foreach (Transform g in transform.GetComponentsInChildren<Transform>())
        {
            //Debug.Log(g.name);
            if (g.tag == "Grid")
                Grid = g.gameObject;
            if (g.tag == "Floor")
                Floor = g.gameObject;
            if (g.tag == "Wall")
                Wall = g.gameObject;
        }
    }
}
