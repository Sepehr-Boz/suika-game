using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour
{

    // playerprefs store 3 highscores: score0, score1, score2

    void Start()
    {
        // check if any score playerprefs made, and if not then make and set to -1
        for (int i = 0; i < 3; i++)
        {
            if (PlayerPrefs.GetInt("score" + i.ToString(), -1) == 0)
            {
                PlayerPrefs.SetInt("score" + i.ToString(), 0);
            }
        }
    }

    public void AddNewScore(int newScore)
    {
        // get the current highscores into an array
        // add the new score and get the highest 3
        // set the highest 3 back to the playerprefs

        List<int> highscores = new List<int>() { PlayerPrefs.GetInt("score0", 0), PlayerPrefs.GetInt("score1", 0), PlayerPrefs.GetInt("score2", 0), newScore };
        highscores.Sort();
        highscores.Reverse();

        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("score" + i.ToString(), highscores[i]);
        }
    }

    public int[] GetCurrentHighscores()
    {
        return new int[] { PlayerPrefs.GetInt("score0", 0), PlayerPrefs.GetInt("score1", 0), PlayerPrefs.GetInt("score2", 0) };
    }
}
