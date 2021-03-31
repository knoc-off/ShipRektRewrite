using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class Collision2DExtensions
{
    public static float GetImpactForce(this Collision2D collision)
    {
        float impulse = 0F;

        foreach (ContactPoint2D point in collision.contacts)
        {
            impulse += point.normalImpulse;
        }

        return impulse / Time.fixedDeltaTime;
    }
}

public class pointStorage
{
    public float ImpactForce;
    public Vector2Int ArrayPoint;
    public Vector2Int LocalPointRounded;
    public pointStorage(float ImpactForce, Vector2Int ArrayPoint, Vector2Int LocalPointRounded)
    {
        this.LocalPointRounded = LocalPointRounded;
        this.ArrayPoint = ArrayPoint;
        this.ImpactForce = ImpactForce;
    }
}

public class collisionManager : MonoBehaviour
{
    private ShipDataClass ThisShip;
    private Tilemap ThisTilemap;
    private Tilemap FloorTilemap;
    private Tilemap WallTilemap;
    Tiles[,] FloorArray;
    Tiles[,] WallArray;
    private bool IsFloor = true;
    //private Vector2 GizmoDrawSquarePos = new Vector2();
    //private Vector2 GizmoDrawCirclePos = new Vector2();
    public float damageMult;
    public List<pointStorage> TilesOfContact = new List<pointStorage>();


    Color RandColor;
    BoundsInt Bounds;
    // Start is called before the first frame update
    void Start()
    {
        RandColor = new Color(Random.value, Random.value, Random.value);

        IsFloor = gameObject.tag.Equals("Floor") ? true : false;


        ThisShip = transform.parent.parent.GetComponent<ShipDataClass>();
        ThisShip.SetData();

        ThisTilemap = IsFloor ? ThisShip.Floor.GetComponent<Tilemap>() : ThisShip.Wall.GetComponent<Tilemap>();
        FloorTilemap = ThisShip.Floor.GetComponent<Tilemap>();
        WallTilemap = ThisShip.Wall.GetComponent<Tilemap>();

        FloorArray = ThisShip.FloorArray;
        WallArray = ThisShip.WallArray;

        print("CollisionManager " + ThisShip.name);
        Bounds = FloorTilemap.cellBounds;
    }
    //private void OnDrawGizmos()
    //{
    //    if (ThisShip != null)
    //    {
    //        Matrix4x4 rotationMatrix = Matrix4x4.TRS(ThisShip.Grid.transform.position, ThisShip.Grid.transform.rotation, ThisShip.Grid.transform.lossyScale);
    //        Gizmos.matrix = rotationMatrix;
    //    }

    //    Gizmos.color = RandColor;
    //    Gizmos.DrawWireCube(V2ToV3(GizmoDrawSquarePos), new Vector3(0.9f, 0.9f, 0));
    //    Gizmos.DrawWireSphere(V2ToV3(GizmoDrawCirclePos), 0.25f);

    //}

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // when new collsiion get all points of collision store that then in the on collision stay iterate through that and apply damage accordingly
        float ImpactForce = 0.00025f * collision.GetImpactForce(); 
        if (ImpactForce > 0.005f && Time.realtimeSinceStartup - delayTime > 0.1f) // removes uneccicary calculations
        {
            delayTime = Time.realtimeSinceStartup;
            TilesOfContact.Clear();
            TilesOfContact = CalculateTilePos(collision);
            foreach (var points in TilesOfContact)
            {

                //  still need a few optimisations but should work well enough for a bit
                try
                {
                    SetHealthOfTile(points.ArrayPoint, points.LocalPointRounded, points.ImpactForce, collision.rigidbody.mass);

                }
                catch
                {
                    print("failed to set health");
                }



                //GizmoDrawSquarePos = new Vector2(points.LocalPointRounded.x + .5f, points.LocalPointRounded.y - .5f);
                //GizmoDrawCirclePos = points.LocalPointRounded;
            }
            //Debug.Break();
            //nearestTiletemp += new Vector2Int(ThisTilemap.origin.x, ThisTilemap.origin.y);


        }
    }

    private float delayTime = 0f;
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    // loop through all tiles subtract by some smallish value multiplied by velocity or something.
    //    foreach (var k in TilesOfContact)
    //    {
    //        if (k.ImpactForce > 0.050f)
    //            try
    //            {
    //                if (!SetHealthOfTile(k.ArrayPoint, k.LocalPointRounded, k.ImpactForce, collision.rigidbody.mass) && Time.realtimeSinceStartup - delayTime > 1) // if no tile and if last calc was < 1 sec
    //                {
    //                    CalculateTilePos(collision);
    //                    delayTime = Time.realtimeSinceStartup;
    //                }
                    
    //                }
    //            catch
    //            {
    //                print("failed to set health");
    //                TilesOfContact = CalculateTilePos(collision);
    //            }
    //    }



    //}

    private List<pointStorage> CalculateTilePos(Collision2D collision)
    {
        List<pointStorage> pointStorages = new List<pointStorage>();
        for (var i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D Contact = collision.GetContact(i);
            Vector2 GlobalPointOfContact = Contact.point;
            Vector2 LocalPointOfContact = ThisTilemap.WorldToLocal(GlobalPointOfContact);

            Vector2Int ArrayPoint = new Vector2Int((int)Mathf.Round(LocalPointOfContact.x - .5f), (int)Mathf.Round(LocalPointOfContact.y + .5f));
            Vector2Int LocalPointRounded = ArrayPoint;
            ArrayPoint -= new Vector2Int(Bounds.min.x, Bounds.min.y);
            ArrayPoint += new Vector2Int(0, 0);        // OFFSET

            // Error With the array stored in ShipData

            try // THIS FINALLY WORKS. Was a problem will cellBounds being set incorrectly as wallbounds instead of floor bounds.
            {

                print("\t\t\tTile Type at " + ArrayPoint + " = " + FloorArray[ArrayPoint.x, ArrayPoint.y].Name); // might need offset
                                                                                                                 //Tiles Temp = TilesArray[ArrayPoint.x, ArrayPoint.y];
                                                                                                                 //TilesArray[ArrayPoint.x, ArrayPoint.y] = TilesArray[ArrayPoint.x, ArrayPoint.y] != TileManager.WeakWoodFloor? TileManager.WeakWoodFloor : TileManager.SolidWoodFloor;
                                                                                                                 //print("\n");
                                                                                                                 //arrayPrint(TilesArray);
                                                                                                                 //TilesArray[ArrayPoint.x, ArrayPoint.y] = Temp;


            }
            catch
            {
                TriggerTileMap.ClearLogConsole();
                arrayPrint(FloorArray);
                // gotta write some sort of get next closest tile algorithem
                print("Out Of Range Or Null \t\t Point at : " + ArrayPoint + "\tBounds : " + Bounds.min);
                Vector2 decPlaces = new Vector2();
                decPlaces.x = LocalPointOfContact.x - (int)LocalPointOfContact.x;
                decPlaces.y = LocalPointOfContact.y - (int)LocalPointOfContact.y;
                decPlaces.x = Mathf.Round(decPlaces.x / .5f) * .5f; //round to nearest .5 hopefully...
                decPlaces.y = Mathf.Round(decPlaces.y / .5f) * .5f;

                decPlaces.x = decPlaces.x < 0 ? decPlaces.x + 1 : decPlaces.x;
                decPlaces.y = decPlaces.y < 0 ? decPlaces.y + 1 : decPlaces.y;

                if (decPlaces.x != 0.5f)
                    if (decPlaces.x > 0.5f)
                    {
                        ArrayPoint.x++;
                        LocalPointRounded.x++;
                    }
                    else
                    {
                        ArrayPoint.x--;
                        LocalPointRounded.x--;
                    }
                if (decPlaces.y != 0.5f)
                    if (decPlaces.y > 0.5f)
                    {
                        ArrayPoint.y++;
                        LocalPointRounded.y++;
                    }
                    else
                    {
                        ArrayPoint.y--;
                        LocalPointRounded.y--;
                    }


                print("Local Point of Contact : " + LocalPointOfContact);
                print("Decimals " + decPlaces);


            }
            pointStorages.Add(new pointStorage(0.00025f * collision.GetImpactForce(), ArrayPoint, LocalPointRounded));
        }
        return pointStorages;
    }


    private bool SetHealthOfTile(Vector2Int ArrayPoint, Vector2Int LocalPointRounded, float ImpactForce, float mass)
    {
        bool returnbool = false;
        if (IsFloor)
        {
            if (FloorArray[ArrayPoint.x, ArrayPoint.y] != null)
            {
                damageMult = FloorArray[ArrayPoint.x, ArrayPoint.y].DamageMult;
                print("first set dmg mult");

                if (WallArray[ArrayPoint.x, ArrayPoint.y] != null)
                {
                    returnbool = true;
                    if (!ThisShip.tileHealth.tileWallHealth.ContainsKey(ArrayPoint))
                        ThisShip.tileHealth.tileWallHealth.Add(ArrayPoint, new BlockStatus(WallArray[ArrayPoint.x, ArrayPoint.y]));
                    print("Has Wall Above tile");
                    damageMult = WallArray[ArrayPoint.x, ArrayPoint.y].DamageMult;
                    print("second set dmg mult");
                    ThisShip.tileHealth.tileWallHealth[ArrayPoint].Health -= ImpactForce * mass * damageMult; //health calc
                    if (ThisShip.tileHealth.tileWallHealth[ArrayPoint].Health <= 0)
                    {
                        print("Wall itle is destroyed");
                        WallTilemap.SetTile(new Vector3Int(LocalPointRounded.x, LocalPointRounded.y - 1, 0), null);
                        ThisShip.tileHealth.tileWallHealth.Remove(ArrayPoint);
                        WallArray[ArrayPoint.x, ArrayPoint.y] = null;
                        ThisShip.GetComponent<ShipSeperator>().SeperateShips(); //construct simpler versiont to only recalc a 3X3 tile around destroyed tile
                    }
                }
                else
                {
                    returnbool = true;
                    if (!ThisShip.tileHealth.tileFloorHealth.ContainsKey(ArrayPoint))
                        ThisShip.tileHealth.tileFloorHealth.Add(ArrayPoint, new BlockStatus(FloorArray[ArrayPoint.x, ArrayPoint.y]));
                    print("Does Not have Wall Above tile");
                    ThisShip.tileHealth.tileFloorHealth[ArrayPoint].Health -= ImpactForce * mass * damageMult;
                    if (ThisShip.tileHealth.tileFloorHealth[ArrayPoint].Health <= 0)
                    {
                        FloorTilemap.SetTile(new Vector3Int(LocalPointRounded.x, LocalPointRounded.y - 1, 0), null);
                        ThisShip.tileHealth.tileFloorHealth.Remove(ArrayPoint);
                        FloorArray[ArrayPoint.x, ArrayPoint.y] = null;
                        ThisShip.GetComponent<ShipSeperator>().SeperateShips();
                    }
                }


                print("\t\t\tTile Health : " + ThisShip.tileHealth.tileFloorHealth[ArrayPoint].Health + "\t Velocity : " + ImpactForce + "\t Mass  : " + mass);

            }
        }
        else
        {
            if (WallArray[ArrayPoint.x, ArrayPoint.y] != null)
            {
                damageMult = WallArray[ArrayPoint.x, ArrayPoint.y].DamageMult;
                if (!ThisShip.tileHealth.tileWallHealth.ContainsKey(ArrayPoint))
                    ThisShip.tileHealth.tileWallHealth.Add(ArrayPoint, new BlockStatus(FloorArray[ArrayPoint.x, ArrayPoint.y]));

                returnbool = true;

                ThisShip.tileHealth.tileWallHealth[ArrayPoint].Health -= ImpactForce * mass * damageMult; //health calc


                if (ThisShip.tileHealth.tileWallHealth[ArrayPoint].Health <= 0)
                {
                    ThisTilemap.SetTile(new Vector3Int(LocalPointRounded.x, LocalPointRounded.y - 1, 0), null);
                    ThisShip.tileHealth.tileWallHealth.Remove(ArrayPoint);
                    ThisShip.GetComponent<ShipSeperator>().SeperateShips();
                }
                print("\t\t\tTile Health : " + ThisShip.tileHealth.tileWallHealth[ArrayPoint].Health + "\t Velocity : " + ImpactForce + "\t Mass  : " + mass);

            }
        }
        return returnbool;
    }

    private Vector3 V2ToV3(Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y, 0);
    }
    private Vector2 V3ToV2(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }
    private Vector2Int V3ToV2Int(Vector3 vec3)
    {
        return new Vector2Int((int)vec3.x, (int)vec3.y);
    }
    private void arrayPrint(Tiles[,] arry)
    {

        for (int x = arry.GetLength(1) - 1; x > 0; x--)
        {
            string Line = "";
            for (int y = 0; y < arry.GetLength(0); y++)
            {
                Line += arry[y, x] == TileManager.SolidWoodFloor || arry[y, x] == TileManager.WoodWall || arry[y, x] == TileManager.WeakWoodFloor ? "▓" : "░"; //   ? "▒" // medium shade
                Line += "      ";
            }
            print(Line);
        }
        print("═══════════════════════════════════════════════════════════════");
    }

}
