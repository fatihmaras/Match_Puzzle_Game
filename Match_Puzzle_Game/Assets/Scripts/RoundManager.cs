using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public float roundTime=60f;
    private UIManager uIManager;
    private bool endingdRound=false;

    private Board board;

    public int currentScore;
    public float displayScore;
    public float scoreSpeed;

    public int scoreTarget1, scoreTarget2, scoreTarget3;


    // Start is called before the first frame update
    void Awake()
    {
        uIManager=FindObjectOfType<UIManager>();
        board=FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        if(roundTime>0)
        {
            roundTime -= Time.deltaTime;

            if(roundTime<=0)
            {
                roundTime=0;

                endingdRound=true;
                
            }
        }
        if(endingdRound && board.currentState==Board.BoardState.move)
        {
            winCheck();
            endingdRound=false;
        }
        uIManager.timeText.text=roundTime.ToString("0.0") + "s";

        displayScore=Mathf.Lerp(displayScore,currentScore, scoreSpeed * Time.deltaTime);  // for increasing score gradually(smooth)
        uIManager.scoreText.text =displayScore.ToString("0"); 
    }

    private void winCheck()
    {
        uIManager.roundOverScreen.SetActive(true);

        uIManager.winScore.text=currentScore.ToString();

        if(currentScore>= scoreTarget3)
        {
            uIManager.winText.text= "Congratulations! You earned 3 stars ";
            uIManager.winStars3.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1" , 1 );
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2" , 1 );
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star3" , 1 );

        }

        else if(currentScore>= scoreTarget2)
        {
            uIManager.winText.text= "Congratulations! You earned 2 stars ";
            uIManager.winStars2.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1",1 );
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2",1 );
            
        }

        else if(currentScore>= scoreTarget1)
        {
            uIManager.winText.text= "Congratulations! You earned 1 star ";
            uIManager.winStars1.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1 );
        }

        else
        {
            uIManager.winText.text =" Oh no! No stars for you! Try Again!";
        }





    }

}
