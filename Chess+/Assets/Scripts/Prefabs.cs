using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    //The public piece prefabs
    public GameObject[] prefabArray;
    public Material[] materialArray;

    public Sprite[] spriteArray;
    public static GameObject pawnPrefab;
    public static GameObject rookPrefab;
    public static GameObject bishopPrefab;
    public static GameObject knightPrefab;
    public static GameObject queenPrefab;
    public static GameObject kingPrefab;
    public static GameObject tilePrefab;

    //The public materials for each team
    public static Material white;
    public static Material black;
    public static Material highLight;

    public static Material highlight2;
    

    //The prefab for cards
    public static GameObject cardPrefab;

    //The prefabs for the images for each card

    public static Sprite rookifyPrefab;

    public static Sprite summonPawnPrefab; 



    public static GameObject getPrefab(Piece type)
    {
        if (type is Pawn)
        {
            return pawnPrefab;
        }
        if (type is Rook)
        {
            return rookPrefab;
        }
        if (type is Knight)
        {
            return knightPrefab;
        }
        if (type is Bishop)
        {
            return bishopPrefab;
        }
        if (type is Queen)
        {
            return queenPrefab;
        }
        if (type is King)
        {
            return kingPrefab;
        }
        throw new System.Exception("Invalid impossible pieceType???");
    }

    // Start is called before the first frame update
    void Start()
    {
        setPrefabs();
    }
    private void setPrefabs()
    {
        pawnPrefab = prefabArray[0];
        rookPrefab = prefabArray[1];
        knightPrefab = prefabArray[2];
        bishopPrefab = prefabArray[3];
        queenPrefab = prefabArray[4];
        kingPrefab = prefabArray[5];
        tilePrefab = prefabArray[6];
        white = materialArray[0];
        black = materialArray[1];
        highLight = materialArray[2];
        cardPrefab = prefabArray[7];
        highlight2 = materialArray[3];
        rookifyPrefab = spriteArray[0];
        summonPawnPrefab = spriteArray[1];
    }
}
