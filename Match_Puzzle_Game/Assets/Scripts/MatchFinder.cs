using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<Gem>currentMatches=new List<Gem>();

    private void Awake() 
    {
        board=FindObjectOfType<Board>();
    }


    public void FindAllMatches()
    {
        currentMatches.Clear();  
        
        for(int x=0 ; x<board.width; x++)
        {
            for(int y=0; y<board.height; y++)
            {
                Gem currentGem= board.allGems[x,y];

                if(currentGem != null)   // Slot is not empty
                {
                    if(x>0 && x<board.width-1) // Selected gem is not on the horizontal edge  + Start to check horizontal
                    {
                        Gem leftGem= board.allGems[x-1,y];
                        Gem rightGem= board.allGems[x+1,y];
                        if(leftGem != null && rightGem != null)  //neighbor slot is not empty
                        {
                            if(leftGem.type==currentGem.type && rightGem.type==currentGem.type)   // Check gem type left-current-right
                            {
                                currentGem.isMatched=true;
                                leftGem.isMatched=true;
                                rightGem.isMatched=true;
                                
                                currentMatches.Add(currentGem);
                                currentMatches.Add(leftGem);
                                currentMatches.Add(rightGem);
                                
                            }
                        }
                    }
                    
                    if(y>0 && y<board.height-1) // Selected gem is not on the vertical edge  + Start to check vertical
                    {
                        Gem aboveGem= board.allGems[x,y+1];
                        Gem belowGem= board.allGems[x,y-1];
                        if(aboveGem != null && belowGem != null)  //neighbor slot is not empty
                        {
                            if(aboveGem.type==currentGem.type && belowGem.type==currentGem.type)   // Check gemtype above-current-below
                            {                             
                                currentGem.isMatched=true;
                                aboveGem.isMatched=true;
                                belowGem.isMatched=true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(aboveGem);
                                currentMatches.Add(belowGem);
                            }
                        }
                    }
                }
            }
        }

        if(currentMatches.Count > 0)
        {
            currentMatches=currentMatches.Distinct().ToList();    // to prevenet to duplicate(consider twice) matched gem ( vertical+horizontal match -- not 6 , it is 5)
        }
    }
}
