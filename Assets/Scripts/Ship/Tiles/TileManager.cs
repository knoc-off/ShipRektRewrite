using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockStatus
{
    public Tiles Material;
    public float Health;
    public int GetIndexOffset() // this is added to the orientation index of the tile
    {
        return (int)(Health / -8f + 3) * 16;
    }
    public BlockStatus(float Health,Tiles Type)
    {
        this.Health = Health;
        Material = Type;
    }
    public BlockStatus(Tiles Type)
    {
        Health = 24f;
        Material = Type;
    }
}


public class GetTileIndex
{
    // define what makes each tile ie up+left
    private bool up;
    private bool down;
    private bool left;
    private bool right;


    public int Getindex(int[,] array, Vector2Int Position)
    {
        GetSurroundings test = new GetSurroundings(Position, array);
        
        int Total = 0;
        if (test.up != Position && array[test.up.x,test.up.y] == 1)
            Total += 8;
        if (test.down != Position && array[test.down.x, test.down.y] == 1)
            Total += 4;
        if (test.left != Position && array[test.left.x, test.left.y] == 1)
            Total += 2;
        if (test.right != Position && array[test.right.x, test.right.y] == 1)
            Total += 1;
        return Total;
    }
    public int Getindex(Tiles[,] array, Vector2Int Position)
    {
        GetSurroundings test = new GetSurroundings(Position, array);
        
        int Total = 0;
        if (test.up != Position && array[test.up.x,test.up.y] != null)
            Total += 8;
        if (test.down != Position && array[test.down.x, test.down.y] != null)
            Total += 4;
        if (test.left != Position && array[test.left.x, test.left.y] != null)
            Total += 2;
        if (test.right != Position && array[test.right.x, test.right.y] != null)
            Total += 1;
        return Total;
    }

}

public class Tiles
{
    //Dictionary<>
    public string Name;
    public float DamageMult;
    public TileBase[] allTiles; // instantiate this at the constructor
    public Tiles(TileBase[] InputTiles)
    {
        Name = "";
        allTiles = InputTiles;
        DamageMult = 1;
    }
    public Tiles(TileBase[] InputTiles, string Name)
    {
        this.Name = Name;
        allTiles = InputTiles;
        DamageMult = 1;
    }
    public Tiles(TileBase[] InputTiles, string Name, float DamageMult)
    {
        this.Name = Name;
        allTiles = InputTiles;
        this.DamageMult = DamageMult;
    }


}



public class TileManager : MonoBehaviour
{
    public float testingVar = 1;
    public static Dictionary<TileBase, Tiles> LookupTable = new Dictionary<TileBase, Tiles>();

    public static Tiles WeakWoodFloor;
    [SerializeField]
    private TileBase[] weakWoodFloorArray = new TileBase[16];

    public static Tiles Template;
    [SerializeField]
    private TileBase[] TemplateArray = new TileBase[16];

    public static Tiles SolidWoodFloor;
    [SerializeField]
    private TileBase[] SolidWoodFloorArray = new TileBase[16];
    
    public static Tiles WoodWall;
    [SerializeField]
    private TileBase[] WoodWallArray = new TileBase[16];

    public void Start()
    {
        WeakWoodFloor = new Tiles(weakWoodFloorArray,"WeakWoodFloor",2 * testingVar);
        Template = new Tiles(TemplateArray, "Template",0 * testingVar);  // indestructable
        SolidWoodFloor = new Tiles(SolidWoodFloorArray, "SolidWoodFloor",1.2f * testingVar);
        WoodWall = new Tiles(WoodWallArray, "WoodWall",1 * testingVar);
        updateLookupList();
    }

    private void updateLookupList()
    {
        LookupTable.Clear();
        addTilesToLookup(WeakWoodFloor.allTiles, WeakWoodFloor);
        addTilesToLookup(Template.allTiles, Template);
        addTilesToLookup(SolidWoodFloor.allTiles, SolidWoodFloor);
        addTilesToLookup(WoodWall.allTiles, WoodWall);
    }
    private void addTilesToLookup(TileBase[] tiles, Tiles Source)
    {
        foreach (var k in tiles)
            LookupTable.Add(k, Source);
    }

    //should have a way to input a TileBase and get where its from/ what its from. through a dictonary  




}
