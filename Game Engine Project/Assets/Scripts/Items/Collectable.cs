using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
   public int score;


   public Collectable(int scorevalue)
   {
    
    this.score = scorevalue;
   }

    public void UpdateScore()
    {
        ScoreManager.scoreManager.UpdateScore(score);
    }

}
