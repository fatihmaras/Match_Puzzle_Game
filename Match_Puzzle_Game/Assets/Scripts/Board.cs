using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject bgTilePrefab;

    public Gem[] gems;
    public Gem[,] allGems;

    public float gemSpeed;

    [HideInInspector]
    public  MatchFinder matchFinder;

    public enum BoardState{ wait , move}
    public BoardState currentState=BoardState.move;

    public Gem bomb;
    public float bombChance=2f;


    private void Awake() 
    {
        matchFinder=FindObjectOfType<MatchFinder>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        allGems=new Gem[width,height];
        Setup();
    }

    void Update() 
    {
       //  matchFinder.FindAllMatches();  --> We dont need to check at the every frame , we need to check when we move objects
        if(Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }

    }

   
   private void Setup() 
   {
        for(int x = 0; x<width; x++)
        {
            for(int y = 0; y<height; y++)
            {
                Vector2 pos = new Vector2(x,y);
                GameObject bgTile = Instantiate(bgTilePrefab,pos,Quaternion.identity);
                bgTile.transform.parent=transform;
                bgTile.name="BG Tile - " + x + " , " +y ;

                int gemToUse =Random.Range(0,gems.Length);


                int iterations=0;  // to prevent to infinity while loop
                while(MatchesAt(new Vector2Int(x,y), gems[gemToUse]) && iterations<100)   // prevent a match at starting the game 
                {
                    gemToUse =Random.Range(0,gems.Length);
                    iterations++;

                    /*if(iterations>0)  // just a debug control
                    {
                        Debug.Log(iterations);
                    } */
                }


                SpawnGem(new Vector2Int(x,y) , gems[gemToUse]);
            }

        }
   }

   private void SpawnGem(Vector2Int pos, Gem gemToSpawn)
   {
        if(Random.Range(0f,100f)<bombChance)
        {
            gemToSpawn=bomb;
        }

        Gem gem= Instantiate(gemToSpawn, new Vector3(pos.x, pos.y +height ,0f) , Quaternion.identity);
        gem.transform.parent=transform;
        gem.name= "Gem - " +pos.x + " , " + pos.y;
        allGems[pos.x, pos.y] =gem;

        gem.SetupGem(pos,this);
   }

   bool MatchesAt(Vector2Int posToCheck , Gem gemToCheck)    // control of starting position of gem ( are there any matches)
   {
        if(posToCheck.x>1 )     // Check Horizontal Match
        {
            if(allGems[posToCheck.x -1, posToCheck.y ] .type ==gemToCheck.type && allGems[posToCheck.x -2, posToCheck.y ] .type ==gemToCheck.type )
            {
                return true;
            }
        }

        if(posToCheck.y>1 )  // Check Vertical Match
        {
            if(allGems[posToCheck.x , posToCheck.y-1 ] .type ==gemToCheck.type && allGems[posToCheck.x , posToCheck.y-2 ] .type ==gemToCheck.type )
            {
                return true;
            }
        }

        return false;
   }

   private void DestroyMatchedGemAt(Vector2Int pos)
   {
        if(allGems[pos.x, pos.y] != null)
        {
            if(allGems[pos.x, pos.y].isMatched)
            {
                Instantiate(allGems[pos.x, pos.y].destroyEffect,new Vector2(pos.x, pos.y),Quaternion.identity);

                Destroy(allGems[pos.x,pos.y].gameObject);
                allGems[pos.x, pos.y]=null;

            }
        }
        
   }

   public void DestroyMatches()
   {
        for(int i=0; i<matchFinder.currentMatches.Count; i++)
        {
            if(matchFinder.currentMatches[i]!=null)
            {
                DestroyMatchedGemAt(matchFinder.currentMatches[i].posIndex);
            }
        }

        StartCoroutine(DecreaseRowCo());
   }

   private IEnumerator DecreaseRowCo()  // move to empty spaces
   {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;

        for(int x = 0; x<width; x++)   // control to empty spaces
        {
            for(int y = 0; y<height; y++)
            {
                if(allGems[x,y]==null)
                {
                    nullCounter++;
                }
                else if (nullCounter>0)
                {
                    allGems[x,y].posIndex.y -= nullCounter;
                    allGems[x, y- nullCounter]=allGems [x,y];
                    allGems[x,y]=null;
                }
            }

            nullCounter=0;
        }

        StartCoroutine(FillBoardCo());
   }

   private IEnumerator FillBoardCo()  // if there is a match after shuffle , provide to destroy them;
   {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(0.5f);
        matchFinder.FindAllMatches();

        if(matchFinder.currentMatches.Count>0)
        {
            yield return new WaitForSeconds(.05f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            currentState=BoardState.move;

        }

   }

   private void RefillBoard()
   {
        for(int x = 0; x<width; x++)   // control to empty spaces
        {
            for(int y = 0; y<height; y++)
            {
                if(allGems[x,y]==null)
                {
                    int gemToUse = Random.Range(0,gems.Length);
                    SpawnGem(new Vector2Int(x,y), gems[gemToUse]);
                }
                
            }
        }

        CheckMisplacedGems();
   }

    private void CheckMisplacedGems()   // This method for fixing bug
    {
        List<Gem> foundedGems=new List<Gem>();
        foundedGems.AddRange(FindObjectsOfType<Gem>());

        for(int x = 0; x<width; x++)   
        {
            for(int y = 0; y<height; y++)
            {
                if(foundedGems.Contains(allGems[x,y]))
                {
                    foundedGems.Remove(allGems[x,y]);
                }                
            }
        }

        foreach(Gem gem in foundedGems)
        {
            Destroy(gem.gameObject);
        }
    }
   
   public void ShuffleBoard()
   {
        if(currentState !=BoardState.wait)
        {
            currentState=BoardState.wait;

            List<Gem> gemsFromBoard=new List<Gem>();

            for(int x = 0; x<width; x++)   
            {
                for(int y = 0; y<height; y++)
                {
                    gemsFromBoard.Add(allGems[x,y]);
                    allGems[x,y]=null;
                }
            
            }

            for(int x = 0; x<width; x++)   
            {
                for(int y = 0; y<height; y++)
                {
                    int gemToUse=Random.Range(0,gemsFromBoard.Count);

                    int iterations=0;
                    while (MatchesAt(new Vector2Int(x,y),gemsFromBoard[gemToUse]) && iterations<100 && gemsFromBoard.Count>1)   
                    {
                        gemToUse=Random.Range(0,gemsFromBoard.Count);
                        iterations++;
                    }

                    gemsFromBoard[gemToUse].SetupGem(new Vector2Int(x,y),this);
                    allGems[x,y]= gemsFromBoard[gemToUse];
                    gemsFromBoard.RemoveAt(gemToUse);
                }
            }

            StartCoroutine(FillBoardCo()); // if there is a match after shuffle , provide to destroy them
        }

   
   }

}
