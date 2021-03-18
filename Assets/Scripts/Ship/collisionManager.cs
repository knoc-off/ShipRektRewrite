using UnityEngine;
using UnityEngine.Tilemaps;
public class collisionManager : MonoBehaviour
{
    private ShipDataClass ThisShip;
    private Tilemap ThisTilemap;
    private Tilemap FloorTilemap;
    Tiles[,] TilesArray;
    private bool IsFloor = true;
    private Vector2 GizmoDrawSquarePos = new Vector2();
    private Vector2 GizmoDrawCirclePos = new Vector2();
    public float MinDamage = 0.05f;
    public float damageMult;

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
        TilesArray = IsFloor ? ThisShip.FloorArray : ThisShip.WallArray;

        print("CollisionManager " + ThisShip.name);
        Bounds = FloorTilemap.cellBounds;
    }
    private void OnDrawGizmos()
    {
        if (ThisShip != null)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(ThisShip.Grid.transform.position, ThisShip.Grid.transform.rotation, ThisShip.Grid.transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
        }

        Gizmos.color = RandColor;
        Gizmos.DrawWireCube(V2ToV3(GizmoDrawSquarePos), new Vector3(0.9f, 0.9f, 0));
        Gizmos.DrawWireSphere(V2ToV3(GizmoDrawCirclePos), 0.25f);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude * collision.rigidbody.mass > MinDamage) // removes unneccicart calculations
        {

            for (var i = 0; i < collision.contactCount; i++)
            {
                ContactPoint2D Contact = collision.GetContact(i);
                Vector2 GlobalPointOfContact = Contact.point;
                Vector2 LocalPointOfContact = ThisTilemap.WorldToLocal(GlobalPointOfContact);

                Vector2Int ArrayPoint = new Vector2Int((int)Mathf.Round(LocalPointOfContact.x - .5f), (int)Mathf.Round(LocalPointOfContact.y + .5f));
                Vector2Int LocalPointRounded = new Vector2Int((int)Mathf.Round(LocalPointOfContact.x - .5f), (int)Mathf.Round(LocalPointOfContact.y + .5f));
                ArrayPoint -= new Vector2Int(Bounds.min.x, Bounds.min.y);
                ArrayPoint += new Vector2Int(0, 0);        // OFFSET

                // Error With the array stored in ShipData

                try // THIS FINALLY WORKS. Was a problem will cellBounds being set incorrectly as wallbounds instead of floor bounds.
                {

                    print("\t\t\tTile Type at " + ArrayPoint + " = " + TilesArray[ArrayPoint.x, ArrayPoint.y].Name); // might need offset
                                                                                                                     //Tiles Temp = TilesArray[ArrayPoint.x, ArrayPoint.y];
                                                                                                                     //TilesArray[ArrayPoint.x, ArrayPoint.y] = TilesArray[ArrayPoint.x, ArrayPoint.y] != TileManager.WeakWoodFloor? TileManager.WeakWoodFloor : TileManager.SolidWoodFloor;
                                                                                                                     //print("\n");
                                                                                                                     //arrayPrint(TilesArray);
                                                                                                                     //TilesArray[ArrayPoint.x, ArrayPoint.y] = Temp;


                }
                catch
                {
                    TriggerTileMap.ClearLogConsole();
                    arrayPrint(TilesArray);
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

                //  still need a few optimisations but should work well enough for a bit
                try
                {
                    SetHealthOfTile(ArrayPoint, LocalPointRounded, collision.relativeVelocity.magnitude, collision.rigidbody.mass);

                }
                catch
                {
                    print("failed to set health");
                }




                GizmoDrawSquarePos = new Vector2(LocalPointRounded.x + .5f, LocalPointRounded.y - .5f);
                GizmoDrawCirclePos = LocalPointOfContact;
                //Debug.Break();
                //nearestTiletemp += new Vector2Int(ThisTilemap.origin.x, ThisTilemap.origin.y);
            }

        }
    }

    private void SetHealthOfTile(Vector2Int ArrayPoint, Vector2Int LocalPointRounded, float velocity, float mass)
    {

        if (TilesArray[ArrayPoint.x, ArrayPoint.y] != null)
        {
            damageMult = TilesArray[ArrayPoint.x, ArrayPoint.y].DamageMult;
            if (!ThisShip.tileHealth.BlockData.ContainsKey(ArrayPoint))
                ThisShip.tileHealth.BlockData.Add(ArrayPoint, new BlockStatus(TilesArray[ArrayPoint.x, ArrayPoint.y]));
            
            ThisShip.tileHealth.BlockData[ArrayPoint].Health -= velocity * mass * damageMult; //health calc
            if (ThisShip.tileHealth.BlockData[ArrayPoint].Health <= 0)
            {
                ThisTilemap.SetTile(new Vector3Int(LocalPointRounded.x, LocalPointRounded.y - 1, 0), null);
                ThisShip.tileHealth.BlockData.Remove(ArrayPoint);
                ThisShip.GetComponent<ShipSeperator>().SeperateShips();
            }
            print("\t\t\tTile Health : " + ThisShip.tileHealth.BlockData[ArrayPoint].Health + "\t Velocity : " + velocity);
            
        }
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
