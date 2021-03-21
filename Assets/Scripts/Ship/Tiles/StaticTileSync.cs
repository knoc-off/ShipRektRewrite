using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StaticTileSync: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AdjustTiles();
    }

    public (Tiles[,] Floor, Tiles[,] Wall, BoundsInt Bounds) AdjustTiles() // specify bounds of where to look
    {
        ShipDataClass ThisShip = GetComponent<ShipDataClass>();

        Tilemap FloorLayoutTilemap = ThisShip.Floor.GetComponent<Tilemap>();
        Tilemap WallLayoutTilemap = ThisShip.Wall.GetComponent<Tilemap>();  // and here
        Tilemap TemplateLayoutmap = ThisShip.Grid.GetComponent<Tilemap>();  // and here

        TemplateLayoutmap.ClearAllTiles();

        FloorLayoutTilemap.CompressBounds();
        BoundsInt Bounds = FloorLayoutTilemap.cellBounds;
        //print("X: " + Bounds.size.x + " , Y: " + Bounds.size.y);

        Tiles[,] FloorShipArray = new Tiles[Bounds.size.x, Bounds.size.y + 1];
        Tiles[,] WallShipArray = new Tiles[Bounds.size.x, Bounds.size.y + 1];     //here 

        GetTileIndex GetTileType = new GetTileIndex();

        for (int x = 0; x < Bounds.size.x + 1; x++)
        {
            for (int y = 0; y < Bounds.size.y + 2; y++) // get array that sets pos of x-1 ,y-1 to correct tile
            {
                if (x < Bounds.size.x) // shitty fix but it should work...
                {
                    if (y < Bounds.size.y + 1)
                    {
                        //LayoutTilemap.origin;
                        FloorShipArray[x, y] = FloorLayoutTilemap.HasTile(new Vector3Int(x, y - 1, 0) + FloorLayoutTilemap.origin) ? // if
                            TileManager.LookupTable[FloorLayoutTilemap.GetTile(new Vector3Int(x, y - 1, 0) + FloorLayoutTilemap.origin)] : null;

                        WallShipArray[x, y] = WallLayoutTilemap.HasTile(new Vector3Int(x, y - 1, 0) + FloorLayoutTilemap.origin) ? //if 
                            TileManager.LookupTable[WallLayoutTilemap.GetTile(new Vector3Int(x, y - 1, 0) + FloorLayoutTilemap.origin)] : null;

                        //FloorShipArray[x, y] = FloorLayoutTilemap.GetTile(new Vector3Int(x, y-1,0) + FloorLayoutTilemap.origin) != null ? 1 : 0;
                        //WallShipArray[x, y] = WallLayoutTilemap.GetTile(new Vector3Int(x, y-1,0) + FloorLayoutTilemap.origin) != null ? 1 : 0; // maby cahnge to wall origin ?
                    }

                }
                if (x >= 1)
                {
                    if (y >= 1)
                    {
                        if (FloorShipArray[x - 1, y - 1] != null)
                        {
                            FloorLayoutTilemap.SetTile(new Vector3Int(x - 1, y - 2, 0) + FloorLayoutTilemap.origin,
                               FloorShipArray[x - 1, y - 1].allTiles[GetTileType.Getindex(FloorShipArray, new Vector2Int(x - 1, y - 1))]); // add health offset here
                            TemplateLayoutmap.SetTile(new Vector3Int(x - 1, y - 2, 0) + FloorLayoutTilemap.origin,
                                TileManager.Template.allTiles[GetTileType.Getindex(FloorShipArray, new Vector2Int(x - 1, y - 1))]);
                            if (WallShipArray[x - 1, y - 1] != null)
                            {
                                //WallType = TileManager.LookupTable[RefWallTilemap.GetTile(new Vector3Int(x + Offset.x, y + Offset.y, 0))];
                                WallLayoutTilemap.SetTile(new Vector3Int(x - 1, y - 2, 0) + FloorLayoutTilemap.origin,
                                    WallShipArray[x - 1, y - 1].allTiles[GetTileType.Getindex(WallShipArray, new Vector2Int(x - 1, y - 1))]);    // messing up here as it is referencing the whole ship array and not wall array -- FIX THIS
                            }
                        }
                    }
                }
            }
        }
        ThisShip.FloorArray = FloorShipArray;
        ThisShip.WallArray = WallShipArray;
        return (FloorShipArray, WallShipArray, Bounds);
    }
}
