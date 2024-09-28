using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private GameObject faceUI;

    public int lvl;

    // update face when enabled
    void OnEnable()
    {
        SetFace(lvl);
    }

    void Update()
    {
        Vector3 thisVP = Camera.main.WorldToViewportPoint(transform.position);

        if (thisVP.x < 0 || thisVP.x > 1 || thisVP.y < 0 || thisVP.y > 1)
        {
            Vector3 mouseVP = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            // check if mouse position also out of bounds, the game should not end if mouse is out of bounds
            if (mouseVP.x < 0 || mouseVP.x > 1) return;

            //print(this + "is out of bounds");
            ObjectPooling.instance.GetComponent<ObjectPooling>().StoreObject(this.gameObject);
            GameManager.instance.GetComponent<GameManager>().EndGame();
        }
    }

    // occurs on end of game, the text emojis keep rotating

    public void SetFace(int index)
    {
        faceUI.GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetTextEmoji(index);
    }

    public void Fusion()
    {
        // call fusion sound effect
        transform.Find("Fusion").gameObject.GetComponent<AudioSource>().Play();
        // call fusion particle effects
        transform.Find("Fusion").gameObject.GetComponent<ParticleSystem>().Play();
    }

    public IEnumerator GoCrazy()
    {
        // turn the fusion particle effect to loop, and play continuously
        ParticleSystem ps = transform.Find("Fusion").gameObject.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = true;
        ps.Play();


        int i = 0;
        while (true)
        {
            SetFace(i % 9);
            // also randomise the colour of the text
            faceUI.GetComponent<TextMeshProUGUI>().color = new Color(Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
            yield return new WaitForSeconds(0.1f);
            i++;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject otherObj = other.gameObject;

        if (otherObj.tag != "Item")
        {
            return;
        }

        // check if other Item is of the same level
        if (lvl == otherObj.GetComponent<ItemController>().lvl)
        {
            // disable the other
            otherObj.SetActive(false);

            if (this.gameObject.activeSelf)
            {
                // call spawn new item in game manager
                GameManager.instance.SpawnNewItem(lvl + 1, transform.position);

                // deactivate itself
                ObjectPooling.instance.StoreObject(this.gameObject);
            }
        }
    }
}
