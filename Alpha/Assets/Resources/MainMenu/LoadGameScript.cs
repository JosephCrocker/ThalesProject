using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LoadGameScript : MonoBehaviour
{
    // ======================= //
    [Header("Player Prefs (Dont Touch)")]
    public float savedScore;
    public float HighScore;
    // ======================= //

    // Menus
    [Header("Menu Vars")]
    public GameObject menus;
    public GameObject sceneSelection;
    public GameObject controls_P1;
    public GameObject controls_P2;
    public GameObject controls_P3;
    public GameObject credits;

    public float PassingScoreValue;
    public Button IslandsButton;
    private bool tutorial = true;
    private bool volanicIsland = false;
    private bool airport = false;

    #if UNITY_STANDALONE_WIN
    public MovieTexture startAnimation;
    #endif

    void Start ()
    {

        #if UNITY_STANDALONE_WIN
        //Renderer r = GetComponent<Renderer>();
        #endif

        savedScore = PlayerPrefs.GetFloat("Score");
        HighScore = PlayerPrefs.GetFloat("HighScore");

        if (HighScore == 0 || savedScore > HighScore)
        {
            HighScore = savedScore;
            PlayerPrefs.SetFloat("HighScore", HighScore);
        }
#if UNITY_STANDALONE_WIN
        if (HighScore > PassingScoreValue)
        {
            IslandsButton.interactable = true;
        }
#endif

#if UNITY_ANDROID
        IslandsButton.interactable = true;
#endif
    }

    public void LoadMainMenu()
    {
        menus.SetActive(true);
    }

    public void Load3DScene()
    {
#if UNITY_STANDALONE_WIN
        startAnimation.Play();
#endif

        sceneSelection.SetActive(false);
        menus.SetActive(false);
        controls_P1.SetActive(false);
        controls_P2.SetActive(false);
        controls_P3.SetActive(false);
        credits.SetActive(false);

#if UNITY_STANDALONE_WIN
        if (startAnimation.isPlaying)
        {
            if (tutorial == true)
            {
                StartCoroutine(LoadFarmland());
            }
            if (volanicIsland == true)
            {
                StartCoroutine(LoadVolcanicIsland());
            }
            if (airport == true)
            {
                StartCoroutine(LoadAirport());
            }
        }
#endif
#if UNITY_ANDROID
        if (tutorial == true)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Farmland_Tutorial");
        }
        if (volanicIsland == true)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("VolcanicIslands");
        }
        if (airport == true)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Airport_0.1.1");
        }
#endif

    }

    public void LoadSceneSelection()
    {
        sceneSelection.SetActive(true);
        menus.SetActive(true);
        controls_P1.SetActive(false);
        controls_P2.SetActive(false);
        controls_P3.SetActive(false);
        credits.SetActive(false);
    }


    public void HowToP1()
    {
        controls_P1.SetActive(true);
        menus.SetActive(true);
        sceneSelection.SetActive(false);
        controls_P2.SetActive(false);
        controls_P3.SetActive(false);
        credits.SetActive(false);
    }

    public void HowToP2()
    {
        controls_P2.SetActive(true);
        menus.SetActive(true);
        sceneSelection.SetActive(false);
        controls_P1.SetActive(false);
        controls_P3.SetActive(false);
        credits.SetActive(false);
    }

    public void HowToP3()
    {
        controls_P3.SetActive(true);
        menus.SetActive(true);
        sceneSelection.SetActive(false);
        controls_P1.SetActive(false);
        controls_P2.SetActive(false);
        credits.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Tutorial()
    {
        tutorial = true;
        volanicIsland = false;
        airport = false;
        Load3DScene();
    }
    public void VolanicIsland()
    {
        tutorial = false;
        volanicIsland = true;
        airport = false;
        Load3DScene();
    }
    public void Airport()
    {
        tutorial = false;
        volanicIsland = false;
        airport = true;
        Load3DScene();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadFeedbackForm()
    {
        //Application.OpenURL("https://goo.gl/forms/Ou7efkeDckvJmePI3");

        menus.SetActive(true);
        sceneSelection.SetActive(false);
        controls_P1.SetActive(false);
        controls_P2.SetActive(false);
        controls_P3.SetActive(false);
        credits.SetActive(true);

    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator LoadFarmland()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Farmland_Tutorial");
    }

    IEnumerator LoadVolcanicIsland()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("VolcanicIslands");
    }

    IEnumerator LoadAirport()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Airport_0.1.1");
    }
}