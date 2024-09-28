using System.Collections.Generic;
using UnityEngine;

public class OverflowChecker : MonoBehaviour
{
    [SerializeField] private List<GameObject> objInside;

    void Start()
    {
        objInside = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // on pass add the item object to the collection of objects inside
        GameObject otherObj = other.gameObject;
        if (otherObj.tag != "Item") return;

        objInside.Add(otherObj);
        //print(otherObj + "has been added");

        // wait 3 seconds and check if the collection still has the object within
        StartCoroutine(GameManager.instance.WaitThenFunc(3.0f, () =>
        {
            if (objInside.Contains(otherObj)) GameManager.instance.EndGame();
        }));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // on exit remove the item object from the collection of objects inside
        GameObject otherObj = other.gameObject;
        if (otherObj.tag != "Item") return;

        objInside.Remove(otherObj);
        //print(otherObj + "has been removed");
    }
}
