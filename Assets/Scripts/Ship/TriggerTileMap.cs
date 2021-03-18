using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


using System.Reflection;
using UnityEditor;
using System;




//[ExecuteInEditMode]
//public class TileSync : MonoBehaviour
//{
//    void Update()
//    {
//        Tilemap Collidemap = GetComponent<Tilemap>();
//        Collidemap = FloorMap;
//    }
//}




public class TriggerTileMap : MonoBehaviour
{
    public static void ClearLogConsole()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Editor));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
    }
    //public CompositeCollider2D collider;
    // Start is called before the first frame update

    private Rigidbody2D ThisShipRB;
    public Tilemap FloorMap;
    void Start()
    {
        //collider = GetComponent<CompositeCollider2D>();
        ThisShipRB = GetComponent<Rigidbody2D>();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag != "Grid")
        try
        {
            Rigidbody2D obj = collision.gameObject.GetComponent<Rigidbody2D>();
            obj.velocity = ThisShipRB.GetPointVelocity(obj.transform.position) * 1.02f; // 1.2? it keeps the player stationary. hope its not mass related
        }
        catch
        {
            //print("TriggerTileMap.cs - no rigid body ");
        }
        //obj.position = new Vector2(0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Trigger Enter : " + collision.name);
        try
        {
            if(collision.tag == "Player")
                GetComponentInParent<ShipSeperator>().SeperateShips();
            
        }
        catch
        {
            //print(collision.gameObject.name + "\t" + collision.gameObject.transform.parent.parent.gameObject.name);
            print("no ShipSeperator Component");
        }
    }

    public void SyncTiles() // this should fire every block change. to save on preformance could write something to check if they are in-view and if i should run it
    {
        ShipDataClass ThisShipData = GetComponentInParent<ShipDataClass>();
        Tilemap LayoutTilemap = ThisShipData.Floor.GetComponent<Tilemap>();
        LayoutTilemap.CompressBounds();
        BoundsInt Bounds = LayoutTilemap.cellBounds;
        //print("X: " + Bounds.size.x + " , Y: " + Bounds.size.y);

        //int[,] ShipArray = new int[Bounds.size.x, Bounds.size.y + 1];
        var TemplateTilemap = ThisShipData.Grid.GetComponent<Tilemap>();

        for (int x = 0; x < Bounds.size.x; x++)
        {
            for (int y = 0; y < Bounds.size.y + 1; y++)// basically just loop through and check for connected blocks
            {
                //LayoutTilemap.origin;

                // make a system where i can see how many connections there are and then grab a tile based off of that. DONE!



                //FloorTilemap.SetTile(new Vector3Int(x , y, 0), ShipChunk.ShipArray[x, y] == 1 ? TileManager.WeakWoodFloor.allTiles[GetTileType.Getindex(ShipChunk.ShipArray, new Vector2Int(x, y))] : null);

                //ShipArray[x, y] = LayoutTilemap.GetTile(new Vector3Int(x, y - 1, 0) + LayoutTilemap.origin) != null ? 1 : 0;
                //LayoutTilemap.GetTile(new Vector3Int(x, y - 1, 0) + LayoutTilemap.origin).name.IndexOf("");
                //TemplateTilemap.SetTile(new Vector3Int(x, y - 1, 0) + LayoutTilemap.origin,);
            }

        }

    }
}
