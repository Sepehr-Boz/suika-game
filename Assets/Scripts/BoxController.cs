using UnityEngine;
using TMPro;

public class BoxController : MonoBehaviour
{
    [SerializeField] private GameObject lid;

    [SerializeField] private GameObject knob;
    [SerializeField] private GameObject[] scoreCounters;

    [SerializeField] private GameObject overflowChild;
    [SerializeField] private GameObject overflowText;

    public void UpdateScoreTracker(int pointsAdded)
    {
        // 1 point = 0.4 degree
        float currentRotation = 0.45f * pointsAdded;
        knob.transform.Rotate(0, 0, -currentRotation);
    }

    public void UpdateScoreMarkers(int currentScore)
    {
        int start_index = 8 * (currentScore / 800);
        // check if the start index is the same
        // if not then loop through the counters and update them
        if (scoreCounters[0].GetComponent<TextMeshProUGUI>().text == start_index.ToString())
        {
            return;
        }

        for (int i = 0; i < scoreCounters.Length; i++)
        {
            scoreCounters[i].GetComponent<TextMeshProUGUI>().text = (start_index + i).ToString();
        }
    }

    public void DropLid()
    {
        // disable the overflow child, and overflow text in canvas
        overflowChild.SetActive(false);
        overflowText.SetActive(false);
        // enable and drop the lid from its current position onto the top of the box
        lid.SetActive(true);

        // play the lid drop sound
        StartCoroutine(GameManager.instance.PlayQuickSound(lid.GetComponent<AudioSource>(), 5f));
    }
}
