using System.Collections;
using TMPro;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private GameObject faceUI;

    public int lvl;
    private bool isFusing;

    // update face when enabled
    void OnEnable()
    {
        SetFace(lvl);
        isFusing = false;
    }

    void Update()
    {
        Vector3 thisVP = Camera.main.WorldToViewportPoint(transform.position);

        if (thisVP.x < 0 || thisVP.x > 1 || thisVP.y < 0 || thisVP.y > 1)
        {
            //Vector3 mouseVP = Camera.main.ScreenToViewportPoint(GameManager.instance.GetLastMousePos());
            //Vector3 currentPosVP = Camera.main.ScreenToViewportPoint(GameManager.instance.GetLastPos());
            //// check if mouse position also out of bounds, the game should not end if mouse is out of bounds
            //if (mouseVP.x < 0 || mouseVP.x > 1) return;
            //else if (currentPosVP.x < 0 || currentPosVP.x > 1) return;


            //print("ITEMCONTROLLER" + this.transform.position.ToString() + gameObject.activeSelf.ToString() + lvl.ToString() + "is out of bounds");
            ObjectPooling.instance.GetComponent<ObjectPooling>().StoreObject(this.gameObject);
            GameManager.instance.GetComponent<GameManager>().EndGame();
        }
    }

    // occurs on end of game, the text emojis keep rotating

    public void SetFace(int index)
    {
        faceUI.GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetTextEmoji(index);
    }

    public void FusionEffects()
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
        if (otherObj.tag != "Item") return;

        if (isFusing) return;

        // check if other Item is of the same level
        if (lvl == otherObj.GetComponent<ItemController>().lvl)
        {
            // check if the other object is already inactive, and if so then stop fusing
            if (!otherObj.activeSelf) return;
            // disable the other
            ObjectPooling.instance.StoreObject(otherObj);
            isFusing = true;

            if (this.gameObject.activeSelf)
            {
                // call spawn new item in game manager
                GameManager.instance.SpawnNewItem(lvl + 1, transform.position);
                // deactivate itself
                ObjectPooling.instance.StoreObject(this.gameObject);
            }
        }
    }

    // check for collision with overflow
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name != "Overflow") return;
        else StartCoroutine(CheckAfterXSeconds(3.0f, other.gameObject));

    }

    // have the object check after x seconds, as it stops when the object is made inactive
    private IEnumerator CheckAfterXSeconds(float seconds, GameObject overflow)
    {
        yield return new WaitForSeconds(seconds);
        if (GetComponent<PolygonCollider2D>().IsTouching(overflow.GetComponent<Collider2D>()) && gameObject.activeSelf)
        {
            //print("ITEMCONTROLLER CHECKAFTERXSECONDS CALLED");
            GameManager.instance.EndGame();
        }
    }
}
