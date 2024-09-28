using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool isActive;

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameHelpMenu;

    void Start()
    {
        isActive = false;
    }

    public void ToggleMenu()
    {
        if (isActive) PlayGame();
        else EnableMenu();
    }


    // game scene methods
    private void EnableMenu()
    {
        isActive = true;
        menuUI.SetActive(true);
    }

    private void PlayGame()
    {
        // check if the game is over, and if so then reload the game scene
        if (GameManager.instance.GetComponent<GameManager>().isGameOver)
        {
            SceneManager.LoadScene("Game");
            return;
        }


        // delay setting isActive back to false as it releases the ball immediately, wait at least 1 second
        StartCoroutine(GameManager.instance.GetComponent<GameManager>().WaitThenFunc(0.5f, () => isActive = false));
        menuUI.SetActive(false);
        // check if help menu also active and if so then set false
        if (gameHelpMenu.activeSelf) gameHelpMenu.SetActive(false);
    }

    public void ShowHelpGameMenu()
    {
        // disable the main menu
        menuUI.SetActive(false);
        // enable the help menu
        gameHelpMenu.SetActive(true);
    }

    public void QuitGame()
    {
        //print("game quit");

        // go back to startmenu scene
        SceneManager.LoadScene("StartMenu");
    }

    public void ReturnToGameMenu()
    {
        // disable help menu
        gameHelpMenu.SetActive(false);
        // enable main menu
        menuUI.SetActive(true);
    }



    // start menu methods

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject scoreMenu;
    [SerializeField] private GameObject helpMenu;
    [SerializeField] private GameObject boxButtonMainMenu;
    private int numHovers = 0;

    [SerializeField] private FileManager fileManager;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowHighscores()
    {
        // disable main menu
        mainMenu.SetActive(false);
        // disable help menu
        helpMenu.SetActive(false);
        // show score menu
        scoreMenu.SetActive(true);

        // update the scores in the main menu

        // get the current scores from the file manager
        int[] highscores = fileManager.GetCurrentHighscores();
        // loop through 0-2 and set the scores
        for (int i = 0; i < 3; i++)
        {
            scoreMenu.transform.Find("ScoreText").Find("Score" + i.ToString()).gameObject.GetComponent<TextMeshProUGUI>().text = highscores[i].ToString();
        }
    }

    public void ShowHelpMainMenu()
    {
        // set only help menu active
        helpMenu.SetActive(true);
        mainMenu.SetActive(false);
        scoreMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        // enable the main menu
        mainMenu.SetActive(true);
        // disable help menu
        helpMenu.SetActive(false);
        // disable score menu
        scoreMenu.SetActive(false);
    }

    public void MouseOverBox()
    {
        boxButtonMainMenu.transform.rotation = Quaternion.Euler(0, 0, numHovers % 2 == 0 ? -20 : 20);

        numHovers++;
    }

    public void MouseExitBox()
    {
        boxButtonMainMenu.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
