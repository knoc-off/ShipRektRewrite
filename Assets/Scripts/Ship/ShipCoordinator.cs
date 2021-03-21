using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipCoordinator : MonoBehaviour
{
    public GameObject ShipPrefab;
    public TileBase TestingTile;
    public bool CreateShip(GameObject ReferenceShip, SubShip ShipChunk, Vector2 Position, Vector2 Velocity, Quaternion Rotation, Vector2 ParentShipOrigin)// At pos with rotation what velocity, array of floor, array of wall.
    {
        GetTileIndex GetTileType = new GetTileIndex();
        //GameObject Shiplayer = GameObject.FindGameObjectWithTag("ShipLayer");

        

        GameObject newShip = Instantiate(ShipPrefab);
        
        ShipDataClass newShipData = newShip.GetComponent<ShipDataClass>();
        newShipData.SetData();
        newShipData.WallArray = ShipChunk.WallArray;
        newShipData.FloorArray = ShipChunk.ShipArray;

        newShipData.tileHealth = ShipChunk.HealthTracker;

        newShip.transform.parent = gameObject.transform;

        newShipData.Grid.GetComponent<Rigidbody2D>().velocity = Velocity;
        newShipData.Grid.transform.position = Position;
        newShipData.Grid.transform.rotation = Rotation;

        Tilemap FloorTilemap = newShipData.Floor.GetComponent<Tilemap>();
        Tilemap WallTilemap = newShipData.Wall.GetComponent<Tilemap>();
        Tilemap GridTemplate = newShipData.Grid.GetComponent<Tilemap>();

        ShipDataClass ReferenceShipData = ReferenceShip.GetComponent<ShipDataClass>();
        ReferenceShipData.SetData();

        //print("\t\t\tTEST: " + RefFloorTilemap.origin);

        for (int x = 0; x < ShipChunk.ShipArray.GetLength(0); x++)
        {
            for (int y = 0; y < ShipChunk.ShipArray.GetLength(1); y++)
            {

                //TileManager.WeakWoodFloor.allTiles[GetTileType.Getindex(ShipChunk.ShipArray, new Vector2Int(x, y))];  < -- this is a fucking work of art

                //FloorTilemap.SetTile(new Vector3Int(x + ShipChunk.Origin.x, y + ShipChunk.Origin.y,0),
                //    RefFloorTilemap.GetTile(new Vector3Int(x + ShipChunk.Origin.x, y + ShipChunk.Origin.y, 0)));
                //WallTilemap.SetTile(new Vector3Int(x + ShipChunk.Origin.x, y + ShipChunk.Origin.y, 0),
                //    RefWallTilemap.GetTile(new Vector3Int(x + ShipChunk.Origin.x, y + ShipChunk.Origin.y, 0)));
                //RefFloorTilemap.GetTile(new Vector3Int(x + ShipChunk.Origin.x, y + ShipChunk.Origin.y, 0));

                //SOMETHINGS WROMG WITH THE PARENT SHIP POS. I NEED TO OFFSET FROM THAT

                Vector2 Offset = new Vector2();//(int)(ParentShipOrigin.x-1), (int)(ParentShipOrigin.y+0.5f -9));//1, -ShipChunk.ShipArray.GetLength(1) - 2);   //why in the hell is this number the way it is
                Offset += ParentShipOrigin;
                //var TileType = TileManager.WeakWoodFloor; // default... not good but eh fine for now
                //var WallType = TileManager.WoodWall; // default... not good but eh fine for now

                Offset += ShipChunk.Origin;// this is just the offset from the default array i need to also offset X,Y of 0, 0 by the parent array

                //if (ShipChunk.ShipArray[x, y] == 1)
                //    TileType = TileManager.LookupTable[FloorTilemap.GetTile(new Vector3Int(x + ShipChunk.Origin.x + Offset.x, y + ShipChunk.Origin.y + Offset.y, 0))];

                Vector2Int OffsetInt = new Vector2Int((int)Offset.x, (int)Offset.y-2);
                //GridTemplate.SetTile(new Vector3Int(x + ShipChunk.Origin.x + Offset.x, y + ShipChunk.Origin.y + Offset.y, 0), ShipChunk.ShipArray[x, y] == 1 ?
                //    TileManager.Template.allTiles[GetTileType.Getindex(ShipChunk.ShipArray, new Vector2Int(x, y))] : null);

                //adjust by subtracting array height and width -1
                //WallTilemap.SetTile(new Vector3Int(x + ShipChunk.Origin.x, y + ShipChunk.Origin.y, 0), ShipChunk.ShipArray[x, y] == 1 ? TestingTile : null);
                if (ShipChunk.ShipArray[x, y] != null)
                {
                    //TileType = TileManager.LookupTable[FloorTilemap.GetTile(new Vector3Int(x + ShipChunk.Origin.x + Offset.x, y + ShipChunk.Origin.y + Offset.y, 0))];

                    //TileType = TileManager.LookupTable[RefFloorTilemap.GetTile(new Vector3Int(x + Offset.x, y + Offset.y, 0))]; // This works!

                    if(ShipChunk.WallArray[x, y] != null)
                    {

                        //WallType = TileManager.LookupTable[RefWallTilemap.GetTile(new Vector3Int(x + Offset.x, y + Offset.y, 0))];
                        WallTilemap.SetTile(new Vector3Int(x + OffsetInt.x, y + OffsetInt.y, 0),
                            ShipChunk.WallArray[x, y].allTiles[GetTileType.Getindex(ShipChunk.WallArray, new Vector2Int(x, y))]);    // messing up here as it is referencing the whole ship array and not wall array -- FIX THIS
                    }

                    FloorTilemap.SetTile(new Vector3Int(x + OffsetInt.x, y + OffsetInt.y, 0),
                        ShipChunk.ShipArray[x, y].allTiles[GetTileType.Getindex(ShipChunk.ShipArray, new Vector2Int(x, y))] );
                    GridTemplate.SetTile(new Vector3Int(x + OffsetInt.x, y + OffsetInt.y, 0),
                        TileManager.Template.allTiles[GetTileType.Getindex(ShipChunk.ShipArray, new Vector2Int(x, y))]);

                    //RefFloorTilemap.SetTile(new Vector3Int(x+ Offset.x, y + Offset.y, 0),null);
                    //RefWallTilemap.SetTile(new Vector3Int(x + Offset.x, y + Offset.y, 0),null);
                }


            }
        }
        FloorTilemap.CompressBounds();
        WallTilemap.CompressBounds();
        //Debug.ClearDeveloperConsole(); // ~~~~~~~~~~~~~~~~~~        remove this if debugging        ~~~~~~~~~~~~~~~~~~

        return true;
    }
}
