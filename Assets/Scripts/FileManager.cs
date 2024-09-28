using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour
{

    // playerprefs store 3 highscores: score0, score1, score2

    public void AddNewScore(int newScore)
    {
        // get the current highscores into an array
        // add the new score and get the highest 3
        // set the highest 3 back to the playerprefs

        List<int> highscores = new List<int>() { PlayerPrefs.GetInt("score0", -1), PlayerPrefs.GetInt("score1", -1), PlayerPrefs.GetInt("score2", -1), newScore };
        highscores.Sort();
        highscores.Reverse();

        //print(ListToString(highscores));

        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("score" + i.ToString(), highscores[i]);
        }
    }

    public int[] GetCurrentHighscores()
    {
        return new int[] { PlayerPrefs.GetInt("score0", -1), PlayerPrefs.GetInt("score1", -1), PlayerPrefs.GetInt("score2", -1) };
    }

    // used to debug //TODO: remove
    private string ListToString(List<int> nums)
    {
        string arr = "[";
        foreach (int num in nums)
        {
            arr += num.ToString() + ",";
        }
        arr += "]";
        return arr;
    }

}
