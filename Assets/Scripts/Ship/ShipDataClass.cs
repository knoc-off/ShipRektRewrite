using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileHealthCoordinator
{
    public Dictionary<Vector2Int, BlockStatus> BlockData = new Dictionary<Vector2Int, BlockStatus>();
}


public class ShipDataClass : MonoBehaviour
{
    public TileHealthCoordinator tileHealth = new TileHealthCoordinator();
    public Tiles[,] FloorArray;
    public Tiles[,] WallArray;
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
        GetComponent<ShipSeperator>().SeperateShips();
    }
}
