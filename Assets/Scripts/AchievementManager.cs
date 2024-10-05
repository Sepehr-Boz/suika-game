using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{

    public static AchievementManager instance;

    [SerializeField] private AchievementList achievementList;

    [SerializeField] private GameObject gameScenePopup;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // go through all the achievements, and if not instantiated already then set to 0
        for (int i = 0; i < 12; i++)
        {
            if (PlayerPrefs.GetInt("Ach" + i.ToString(), -1) == -1)
            {
                PlayerPrefs.SetInt("Ach" + i.ToString(), 0);
            }
        }
    }

    private void UnlockAchievement(string achievementId, int index)
    {
        int achStatus = PlayerPrefs.GetInt(achievementId, 0);
        if (achStatus == 1)
        {
            return;
        }

        PlayerPrefs.SetInt(achievementId, 1);
        // get the title and description from the scriptable object 
        Achievement ach = achievementList.achievements[index];

        StartCoroutine(ShowPopupForX(3.5f, ach.Title, ach.Description));
    }

    private IEnumerator ShowPopupForX(float seconds, string title, string desc)
    {
        // update the text in the game scene popup
        gameScenePopup.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        gameScenePopup.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = desc;

        gameScenePopup.SetActive(true);
        yield return new WaitForSeconds(seconds);
        //Color panelColour = gameScenePopup.transform.Find("Panel").GetComponent<Image>().color;
        // for (int i = 150; i > 0; i--)
        // {
        //     gameScenePopup.transform.Find("Panel").GetComponent<Image>().color = new Color(panelColour.r, panelColour.g, panelColour.b, i);
        //     yield return new WaitForSeconds(0.01f);
        // }
        gameScenePopup.SetActive(false);
        //gameScenePopup.transform.Find("Panel").GetComponent<Image>().color = new Color(panelColour.r, panelColour.g, panelColour.b, 150);
    }

    public void CheckScoreAchievements(int currentScore)
    {
        // check the current score and unlock for each boundary
        if (currentScore > 1000) UnlockAchievement("Ach0", 0);
        if (currentScore > 2500) UnlockAchievement("Ach1", 1);
        if (currentScore > 5000) UnlockAchievement("Ach2", 2);
        if (currentScore > 10_000) UnlockAchievement("Ach3", 3);

        return;
    }

    public void CheckSmoothieAchievements(GameObject[] objsActive)
    {
        // set a default dict with all lvls and set the vals to 0
        Dictionary<int, int> lvlsActive = new Dictionary<int, int>();
        for (int i = 0; i < 9; i++)
        {
            lvlsActive.Add(i, 0);
        }
        // loop through all objsactive and increment the vals of the lvls
        foreach (GameObject obj in objsActive)
        {
            lvlsActive[obj.GetComponent<ItemController>().lvl]++;
        }

        CheckCollectorAchievements(lvlsActive);
        // check all the vals, if >= 2 in each then unlock HeavySmoothie
        bool heavyStatus = true;
        bool lightStatus = true;

        foreach (int val in lvlsActive.Values)
        {
            if (val < 2) heavyStatus = false;
            if (val < 1) lightStatus = false;
        }
        // if >= 1 then unlock LightSmoothie
        if (lightStatus) UnlockAchievement("Ach4", 4);
        if (heavyStatus) UnlockAchievement("Ach5", 5);
        // else dont unlock anything
        return;
    }

    private void CheckCollectorAchievements(Dictionary<int, int> vals)
    {
        // apple = 4 = ach6
        if (vals[4] >= 1) UnlockAchievement("Ach6", 6);
        // dragonfruit = 5 = ach7
        if (vals[5] >= 1) UnlockAchievement("Ach7", 7);
        // peach = 6 = ach8
        if (vals[6] >= 1) UnlockAchievement("Ach8", 8);
        // pomengranate = 7 = ach9
        if (vals[7] >= 1) UnlockAchievement("Ach9", 9);
        // watermelon = 8 = ach10
        if (vals[8] >= 1) UnlockAchievement("Ach10", 10);

        return;
    }

    public void CheckAllAchievements()
    {
        // loop through all achievements 0-10, and if all are set to 1 then unlock ach11 as well
        for (int i = 0; i < 11; i++)
        {
            int achStatus = PlayerPrefs.GetInt("Ach" + i.ToString());
            if (achStatus == 0) return;
        }
        // if reach the end of the loop then all are unlocked
        UnlockAchievement("Ach11", 11);
    }


    public bool[] GetCurrentAchievementStatus()
    {
        bool[] stats = new bool[12];

        for (int i = 0; i < 12; i++)
        {
            stats[i] = PlayerPrefs.GetInt("Ach" + i.ToString()) == 1 ? true : false;
        }
        return stats;
    }
}
