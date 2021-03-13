using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SubShip    // should add a type of array that stores what kind of block at x,y; i should use a Tiles[,] array due to it not being inharreted from mono so i can still thread it
{
    public Vector2Int Origin; // 0,0 of this array corrisponds to the X,Y of the Parent array
    public int[,] ShipArray;
    public int[,] WallArray;
    public List<Vector2Int> AllBlocks;
    public SubShip(int[,] ShipChunk, int[,] ShipWall,Vector2Int Zeros, List<Vector2Int> Tiles)
    {
        WallArray = ShipWall;
        ShipArray = ShipChunk;
        Origin = Zeros;
        AllBlocks = Tiles;
    }

}

class GetSurroundings
{
    public Vector2Int up;
    public Vector2Int down;
    public Vector2Int left;
    public Vector2Int right;
    public GetSurroundings(Vector2Int CenterPos)
    {
        up = new Vector2Int(CenterPos.x, CenterPos.y + 1);
        down = new Vector2Int(CenterPos.x, CenterPos.y - 1);
        right = new Vector2Int(CenterPos.x + 1, CenterPos.y);
        left = new Vector2Int(CenterPos.x - 1, CenterPos.y);

    }
    public GetSurroundings(Vector2Int CenterPos, int[,] array)
    {
        up = new Vector2Int(CenterPos.x, CenterPos.y + 1);
        down = new Vector2Int(CenterPos.x, CenterPos.y - 1);
        right = new Vector2Int(CenterPos.x + 1, CenterPos.y);
        left = new Vector2Int(CenterPos.x - 1, CenterPos.y);

        up = up.y > array.GetLength(1) - 1 ? CenterPos : up;
        right = right.x > array.GetLength(0) - 1 ? CenterPos : right;
        down = down.y < 0 ? CenterPos : down;
        left = left.x < 0 ? CenterPos : left;

    }
}


public class ShipSeperator : MonoBehaviour
{
    public GameObject ShipLayer;
    // Start is called before the first frame update
    void Start()
    {
        //print("ShipSeperatorStart");
        ShipLayer = GameObject.FindGameObjectWithTag("ShipLayer");
        SeperateShips();

        
    }
    
    public bool SeperateShips() // This works! // This has to be on the main thread
    {
        ShipDataClass ThisShip = GetComponent<ShipDataClass>();

        Tilemap FloorLayoutTilemap = ThisShip.Floor.GetComponent<Tilemap>();
        Tilemap WallLayoutTilemap = ThisShip.Wall.GetComponent<Tilemap>();  // and here
        FloorLayoutTilemap.CompressBounds();
        BoundsInt Bounds = FloorLayoutTilemap.cellBounds;
        print("X: " + Bounds.size.x + " , Y: " + Bounds.size.y);

        int[,] FloorShipArray = new int[Bounds.size.x, Bounds.size.y+1];
        int[,] WallShipArray = new int[Bounds.size.x, Bounds.size.y+1];     //here 

        for(int x = 0; x < Bounds.size.x; x++) 
        {
            for(int y = 0; y < Bounds.size.y + 1; y++)
            {
                //LayoutTilemap.origin;
                FloorShipArray[x, y] = FloorLayoutTilemap.GetTile(new Vector3Int(x, y-1,0) + FloorLayoutTilemap.origin) != null ? 1 : 0;
                WallShipArray[x, y] = WallLayoutTilemap.GetTile(new Vector3Int(x, y-1,0) + FloorLayoutTilemap.origin) != null ? 1 : 0; // maby cahnge to wall origin ?

            }

        }
        arrayPrint(FloorShipArray);

        List<SubShip> SubShips = new List<SubShip>();
        List<Vector2Int> ListOfBlockPos = new List<Vector2Int>();
        for (int x = 0; x < Bounds.size.x; x++)
        {
            for (int y = 0; y < Bounds.size.y + 1; y++)
            {
                //LayoutTilemap.origin;
                if(!ListOfBlockPos.Contains(new Vector2Int(x,y)))
                    if (FloorShipArray[x, y] == 1)
                    {
                        //int[,] a = new int[ShipArray.GetLength(0), ShipArray.GetLength(1)];
                        //a = ShipArray;
                        //a[x,y] = 2;
                        //arrayPrint(a);
                        SubShip shipChunk = FloodSearch(new Vector2Int(x, y), FloorShipArray, WallShipArray);   // add a print here to show where its found a 1
                        SubShips.Add(shipChunk);
                        ListOfBlockPos.AddRange(shipChunk.AllBlocks);
                    }



            }

        }

        
        ShipCoordinator ShipCoord = ShipLayer.GetComponent<ShipCoordinator>();
        ShipDataClass ShipData = GetComponent<ShipDataClass>();


        if(SubShips.Count > 1)
        {
            foreach(var k in SubShips)
            {


                arrayPrint(k.ShipArray);//could make it save the tilebase at in a shupchunk then delete the whole gameobject and then make a new ship
                ShipCoord.CreateShip(gameObject, k, ShipData.Grid.transform.position, ShipData.Grid.GetComponent<Rigidbody2D>().velocity, ShipData.Grid.transform.rotation, new Vector2(FloorLayoutTilemap.origin.x, FloorLayoutTilemap.origin.y));

                // provide reference ship and then a shipChunk to go with it. it will then go through and split it into a second ship removing the first one


                //LoopThorugh the array and match up floor with ship array and copy the wall above it


                //ShipLayer.NewShip(k.ShipArrayFloor, k.ShipArrayWall, k.Origin/offset)
                //ship constructor should automatically assign the correct orientation of tile to each thing.
                //use the strat of cube marching with hex numbering
            }
            ShipData.Grid.GetComponent<Tilemap>().ClearAllTiles();
            Destroy(gameObject,1);
        }
        else
        {

        }

        //Delete This GameObject? might cause errors
        return false;
    }


    private void arrayPrint(int[,] arry)
    {

        for(int x = arry.GetLength(1) -1; x > 0; x--)
        {
            string Line = "";
            for(int y = 0; y < arry.GetLength(0); y++)
            {
                Line += arry[y,x] == 1 ? "▓" : arry[y,x] == 0 ? "░" : "▒";
                Line += "      ";
            }
            print(Line);
        }
        print("═══════════════════════════════════════════════════════════════");
    }

    private SubShip FloodSearch(Vector2Int Start, int[,] SearchArray, int[,] wallArray) // im 90% sure this can be pushed to another thread with minor reworks
    {
        List<Vector2Int> Searched = new List<Vector2Int>();
        List<Vector2Int> toSearch = new List<Vector2Int>();
        toSearch.Add(Start);

        int maxX = Start.x;
        int minX = Start.x;
        int maxY = Start.y;
        int minY = Start.y;


        while (toSearch.Count > 0)
        {
            var tempDel = toSearch;
            for(var k = 0; k < toSearch.Count; k++)// error here idk wtf to do 
            {
                var i = toSearch[k];

                GetSurroundings Adj = new GetSurroundings(i); // adjacent vars

                if(!(Adj.up.y > SearchArray.GetLength(1)-1))
                    if (SearchArray[Adj.up.x, Adj.up.y] == 1 && !Searched.Contains(Adj.up))
                    {
                        toSearch.Add(Adj.up);
                        maxY = Adj.up.y > maxY ? Adj.up.y : maxY;
                        //SearchArray[Adj.up.x, Adj.up.y] = 0;
                    }
                if (!(Adj.down.y < 0))
                    if (SearchArray[Adj.down.x, Adj.down.y] == 1 && !Searched.Contains(Adj.down))
                    {
                        toSearch.Add(Adj.down);
                        minY = Adj.down.y < minY ? Adj.down.y : minY;
                        //SearchArray[Adj.down.x, Adj.down.y] = 0;
                    }
                if (!(Adj.right.x > SearchArray.GetLength(0)-1))
                    if (SearchArray[Adj.right.x, Adj.right.y] == 1 && !Searched.Contains(Adj.right))
                    {
                        toSearch.Add(Adj.right);
                        maxX = Adj.right.x > maxX ? Adj.right.x : maxX;
                        //SearchArray[Adj.right.x, Adj.right.y] = 0;
                    }
                if (!(Adj.left.x < 0))
                    if (SearchArray[Adj.left.x, Adj.left.y] == 1 && !Searched.Contains(Adj.left))
                    {
                        toSearch.Add(Adj.left);
                        minX = Adj.left.x < minX ? Adj.left.x : minX;
                        //SearchArray[Adj.left.x, Adj.left.y] = 0;
                    }
                // if i. x is > / < max



                Searched.Add(i);
            }
            for(var l = 0; l < tempDel.Count; l++)
                toSearch.Remove(tempDel[l]);

        }

        // search func
        print(" minx " + minX + " , miny " + minY + " \t " + "maxX " + maxX + " , maxY " + maxY +"\n" + "ARRAY SIZE: " + (maxX - minX + 1) + ", " + (maxY - minY + 1));
        int[,] ReturnArray = new int[maxX - minX + 1, maxY - minY + 2];
        int[,] ReturnWallArray = new int[maxX - minX + 1, maxY - minY + 2];
        foreach(var pos in Searched)
        {
            //print("x "+(pos.x - minX) + " , y " + (pos.y - minY));
            ReturnArray[pos.x - minX, pos.y - minY + 1] = 1;    // out of bounds
            ReturnWallArray[pos.x - minX, pos.y - minY + 1] = wallArray[pos.x,pos.y] == 1 ? 1 : 0;
        }
        //int[,] NewArray = new int[maxX,maxY];
        
        return new SubShip(ReturnArray, ReturnWallArray, new Vector2Int(minX,minY), Searched);

    }


}
