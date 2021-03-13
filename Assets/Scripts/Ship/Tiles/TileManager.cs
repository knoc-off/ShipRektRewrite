using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

}

public class Tiles
{
    //Dictionary<>
    public TileBase[] allTiles = new TileBase[16]; // instantiate this at the constructor
    public TileBase middleMiddle;//15
    public TileBase middleRight; //14
    public TileBase middleLeft;  //13
    public TileBase vertical;    //12
    public TileBase bottomMiddle;//11
    public TileBase bottomRight; //10
    public TileBase bottomLeft;  //9
    public TileBase downEnd;     //8
    public TileBase topMiddle;   //7
    public TileBase topRight;    //6
    public TileBase topLeft;     //5
    public TileBase topEnd;      //4
    public TileBase horizontal;  //3
    public TileBase rightEnd;    //2
    public TileBase leftEnd;     //1
    public TileBase single;      //0
    public Tiles(TileBase topLeft, TileBase topMiddle, TileBase topRight, TileBase middleLeft, TileBase middleMiddle, TileBase middleRight, TileBase bottomLeft,
        TileBase bottomMiddle, TileBase bottomRight, TileBase horizontal, TileBase vertical, TileBase topEnd, TileBase leftEnd, TileBase rightEnd, TileBase downEnd, TileBase single)
    {

        this.middleMiddle = middleMiddle;
        allTiles[15] = middleMiddle;
        this.middleRight = middleRight;
        allTiles[14] = middleRight;
        this.middleLeft = middleLeft;
        allTiles[13] = middleLeft;
        this.vertical = vertical;
        allTiles[12] = vertical;
        this.bottomMiddle = bottomMiddle;
        allTiles[11] = bottomMiddle;
        this.bottomRight = bottomRight;
        allTiles[10] = bottomRight;
        this.bottomLeft = bottomLeft;
        allTiles[9] = bottomLeft;
        this.downEnd = downEnd;
        allTiles[8] = downEnd;
        this.topMiddle = topMiddle;
        allTiles[7] = topMiddle;
        this.topRight = topRight;
        allTiles[6] = topRight;
        this.topLeft = topLeft;
        allTiles[5] = topLeft;
        this.topEnd = topEnd;
        allTiles[4] = topEnd;
        this.horizontal = horizontal;
        allTiles[3] = horizontal;
        this.rightEnd = rightEnd;
        allTiles[2] = rightEnd;
        this.leftEnd = leftEnd;
        allTiles[1] = leftEnd;
        this.single = single;
        allTiles[0] = single;
    }
    public Tiles(TileBase[] InputTiles)
    {
        allTiles = InputTiles;
        this.middleMiddle = InputTiles[15];
        this.middleRight = InputTiles[14];
        this.middleLeft = InputTiles[13];
        this.vertical = InputTiles[12];
        this.bottomMiddle = InputTiles[11];
        this.bottomRight = InputTiles[10];
        this.bottomLeft = InputTiles[9];
        this.downEnd = InputTiles[8];
        this.topMiddle = InputTiles[7];
        this.topRight = InputTiles[6];
        this.topLeft = InputTiles[5];
        this.topEnd = InputTiles[4];
        this.horizontal = InputTiles[3];
        this.rightEnd = InputTiles[2];
        this.leftEnd = InputTiles[1];
        this.single = InputTiles[0];
    }

}



public class TileManager : MonoBehaviour
{
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

    public static Tiles ReverseLookUpTile(TileBase Lookup)
    {
        return LookupTable[Lookup];
    }

    private void Start()
    {
        WeakWoodFloor = new Tiles(weakWoodFloorArray);
        Template = new Tiles(TemplateArray);
        SolidWoodFloor = new Tiles(SolidWoodFloorArray);
        WoodWall = new Tiles(WoodWallArray);
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
