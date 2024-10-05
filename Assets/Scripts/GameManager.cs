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
    private bool keepPlaying;
    private Vector2 lastMousePos;
    private Vector2 lastPos;


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
    private Vector2 GetCurrentPos()
    {
        return new Vector2(Input.GetAxis("Horizontal"), 0f);
    }

    public Vector2 GetLastMousePos() => lastMousePos;
    public Vector2 GetLastPos() => lastPos;

    public Vector2 AddVector2s(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x + b.x, a.y + b.y);
    }

    private bool HasMouseMoved()
    {
        return !(lastMousePos == GetCurrentMousePos());
    }

    private float BoundXPos(float x)
    {
        return x < 0 ? Math.Max(x, -3.5f) : Math.Min(x, 2.5f);
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        keepPlaying = true;

        score = 0;
        isGameOver = false;

        StartCoroutine(WaitUntilCond(ObjectPooling.instance.GetComponent<ObjectPooling>().isReady, () => SpawnNewItem()));
    }

    void Update()
    {
        // wait until game is in focus
        if (!keepPlaying)
        {
            return;
        }

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
            Vector2 currentMousePos = GetCurrentMousePos();
            // if not moving then move based on keys instead
            float xPos = 0;
            if (HasMouseMoved())
            {
                xPos = GetCurrentMousePos().x;
                lastMousePos = currentMousePos;
            }
            else
            {
                xPos = GetCurrentPos().x + currentItem.transform.position.x;
            }
            currentItem.transform.position = new Vector2(BoundXPos(xPos), 9);

            //float xPos = GetCurrentMousePos().x;

            //currentItem.transform.position = new Vector2(xPos <= 0.0f ? Math.Max(xPos, -3.5f) : Math.Min(xPos, 2.5f), 9);
            //currentItem.transform.position = AddVector2s(currentItem.transform.position, GetCurrentPos());
            //currentItem.transform.Rotate(0.0f, 0.0f, Input.GetAxis("Horizontal") * -10.0f);

            // // TODO: TEST CASE DELETE ON BUILD
            // if (Input.GetKeyDown(KeyCode.Tab))
            // {
            //     UpdateScore(10);
            // }



            // // TODO: TEST CASE DELETE ON BUILD
            // if (Input.GetKeyUp(KeyCode.Backspace) && !isGameOver)
            // {
            //     currentItem.transform.position = new Vector2(5, 9);
            //     DropCurrentItem();
            //     return;
            // }
            // check if mouse button clicked or spacebar clicked or enter clicked
            if ((Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) && !isGameOver)
            {
                DropCurrentItem();
                StartCoroutine(WaitThenFunc(1.0f, SpawnNewItem));
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            StartCoroutine(WaitThenFunc(0.1f, () => keepPlaying = true));
        }
        else
        {
            keepPlaying = false;
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

        float rand = UnityEngine.Random.Range(0.00f, 1.00f);
        int newLvl;
        if (rand < 0.475f) newLvl = 0; // 47.5% chance
        else if (rand < 0.95f) newLvl = 1; // 47.5% chance
        else newLvl = 2; // 5% chance

        float xPos = 0.0f;
        if (HasMouseMoved())
        {
            xPos = GetCurrentMousePos().x;
            lastMousePos = GetCurrentMousePos();
        }
        else
        {
            xPos = lastPos.x;
        }
        Vector2 spawnPos = new Vector2(BoundXPos(xPos), 9);

        currentItem = ObjectPooling.instance.GetComponent<ObjectPooling>().GetObject(newLvl, spawnPos);
        // set the rotation to 0
        currentItem.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currentItem.GetComponent<ItemController>().lvl = newLvl;
        currentItem.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;

        // // set face of current item
        // currentItem.GetComponent<ItemController>().SetFace(newLvl);

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
        objControl.FusionEffects();
        // add score
        UpdateScore(lvl * 10);

        // // update face
        // objControl.SetFace(lvl);
    }

    private void DropCurrentItem()
    {
        if (!HasMouseMoved())
        {
            lastPos = currentItem.transform.position;
        }


        // play item drop sound
        StartCoroutine(PlayQuickSound(currentItem.GetComponent<AudioSource>()));

        currentItem.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        currentItem = null;

        StartCoroutine(WaitThenFunc(0.5f, () => UpdateScore(5)));
    }


    public void UpdateScore(int pointsAdded)
    {
        score += pointsAdded;
        scoreUI.GetComponent<TextMeshProUGUI>().text = score.ToString();

        box.GetComponent<BoxController>().UpdateScoreTracker(pointsAdded);
        box.GetComponent<BoxController>().UpdateScoreMarkers(score);

        // check if any achievements reached
        AchievementManager.instance.CheckScoreAchievements(score);
        AchievementManager.instance.CheckSmoothieAchievements(ObjectPooling.instance.GetObjsActive());
        AchievementManager.instance.CheckAllAchievements();
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
