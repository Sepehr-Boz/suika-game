using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// GameManager singleton to hold common data that will be shared
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private string[] spriteFaces;


    private GameObject currentItem;
    public bool isGameOver;


    [SerializeField] private int score;
    public GameObject scoreUI;
    public GameObject box;

    [SerializeField] private GameObject canvas;

    [SerializeField] private FileManager fileManager;

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


    public Sprite GetTexture(int index) => sprites[index];

    public string GetTextEmoji(int index) => spriteFaces[index];

    private Vector2 GetCurrentMousePos()
    {
        return FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);
    }

    void Start()
    {
        score = 0;
        isGameOver = false;

        StartCoroutine(WaitUntilCond(ObjectPooling.instance.GetComponent<ObjectPooling>().isReady, () => SpawnNewItem()));
    }

    void Update()
    {
        if (!ObjectPooling.instance.GetComponent<ObjectPooling>().isReady) return;


        // check if esc button pressed and if so then enable main menu
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            canvas.GetComponent<UIManager>().ToggleMenu();
            return;
        }



        else if (currentItem && !canvas.GetComponent<UIManager>().isActive)
        {
            // if current item exists then track the current item to the mouse position
            currentItem.transform.position = new Vector2(GetCurrentMousePos().x, 9);
            currentItem.transform.Rotate(0.0f, 0.0f, Input.GetAxis("Horizontal") * -10.0f);

            // check if button clicked
            if (Input.GetMouseButtonUp(0) && !isGameOver)
            {
                DropCurrentItem();
                StartCoroutine(WaitThenFunc(1.0f, SpawnNewItem));
            }
        }
    }


    // for spawning a new current item
    private void SpawnNewItem()
    {
        // check if the menu is inactive currently, if not then wait and recurse itself
        if (canvas.GetComponent<UIManager>().isActive)
        {
            StartCoroutine(WaitThenFunc(1.0f, SpawnNewItem));
            return;
        }

        int newLvl = UnityEngine.Random.Range(0, 2);
        currentItem = ObjectPooling.instance.GetComponent<ObjectPooling>().GetObject(newLvl, new Vector2(GetCurrentMousePos().x, 9));
        currentItem.GetComponent<ItemController>().lvl = newLvl;
        currentItem.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;

        // set face of current item
        currentItem.GetComponent<ItemController>().SetFace(newLvl);

    }

    // for spawning a fused item
    public void SpawnNewItem(int lvl, Vector2 pos)
    {
        // pull a new object from object pool and spawn at pos
        GameObject newObj = ObjectPooling.instance.GetObject(lvl, pos);
        // update the new items lvl
        ItemController objControl = newObj.GetComponent<ItemController>();
        objControl.lvl = lvl;
        // activate the sound, and effects on the new item, and update the face
        objControl.Fusion();
        // add score
        UpdateScore(lvl * 10);

        // update face
        objControl.SetFace(lvl);
    }

    private void DropCurrentItem()
    {
        // play item drop sound
        StartCoroutine(PlayQuickSound(currentItem.GetComponent<AudioSource>()));

        currentItem.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        currentItem = null;

        StartCoroutine(WaitThenFunc(0.5f, () => UpdateScore(5)));
    }


    public void UpdateScore(int pointsAdded)
    {
        // play sound effect for 1 second then pause it
        StartCoroutine(PlayQuickSound(scoreUI.GetComponent<AudioSource>(), pointsAdded * 0.05f));

        score += pointsAdded;
        scoreUI.GetComponent<TextMeshProUGUI>().text = score.ToString();

        box.GetComponent<BoxController>().UpdateScoreTracker(pointsAdded);
        box.GetComponent<BoxController>().UpdateScoreMarkers(score);
    }

    public IEnumerator PlayQuickSound(AudioSource source)
    {
        source.Play();
        yield return new WaitForSeconds(0.5f);
        source.Pause();
    }

    public IEnumerator PlayQuickSound(AudioSource source, float seconds)
    {
        source.Play();
        yield return new WaitForSeconds(seconds);
        source.Pause();
    }

    public void EndGame()
    {
        // disable spawning new objects
        isGameOver = true;
        //print("Game is finished" + score.ToString());

        fileManager.AddNewScore(score);

        // if an item still exists on game end then despawn it and set current item null
        if (currentItem != null)
        {
            ObjectPooling.instance.GetComponent<ObjectPooling>().StoreObject(currentItem);
            currentItem = null;
        }

        // call the drop lid from box controller
        box.GetComponent<BoxController>().DropLid();

        // get all active objects and call the GoCrazy coroutine on them
        GameObject[] activeObjs = ObjectPooling.instance.GetObjsActive();
        //print(activeObjs.Length);
        foreach (GameObject obj in activeObjs)
        {
            // start the coroutine after a random float (0 - 1) seconds to make them rotate at different intervals
            StartCoroutine(WaitThenFunc(UnityEngine.Random.Range(0.01f, 1.0f), () =>
            {
                StartCoroutine(obj.GetComponent<ItemController>().GoCrazy());
            }));
        }

        // open the menu
        canvas.GetComponent<UIManager>().ToggleMenu();
    }


    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // waits seconds seconds then does func
    public IEnumerator WaitThenFunc(float seconds, Action func)
    {
        yield return new WaitForSeconds(seconds);
        func();
    }

    // waits until condition then executes action
    public IEnumerator WaitUntilCond(bool cond, Action func)
    {
        while (true)
        {
            if (cond)
            {
                func();
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
