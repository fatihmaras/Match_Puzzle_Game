using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    //[HideInInspector]
    public Vector2Int posIndex;
    // [HideInInspector]
    public Board board;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupGem(Vector2Int pos , Board theBoard)
    {
        posIndex=pos;
        board=theBoard;

    }
}
