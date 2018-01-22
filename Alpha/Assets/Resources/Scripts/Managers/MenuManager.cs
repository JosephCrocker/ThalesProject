using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using PlaneMan = Singleton<PlaneManager>;
public class MenuManager : MonoBehaviour
{
    public Transform PauseMenu;
    public Transform GameUI;
    public Transform MainMenu;
    public bool DestroySelf;
    // =========================== //
    // PlayerPref UI - New Data
    [Header("PlayerPrefs")]
    public Text CrashScoreVal;
    public Text SpawnScoreVal;
    public Text NewScoreVal;
    // =========================== //
    //PlayerPref System
    public float PrefCrashes;
    public float PrefSpawns;
    public float PrefScore;
    // Pref Strings
    private string CrashVal;
    private string SpawnVal;
    private string ScoreVal;
    // =========================== //
    private bool MainMenuIsActive;
    private bool PauseIsActive;
    [Header("Timer Settings")]
    public bool TimerActivate;
    private float TimeHolder;
    public float m_Time;
    public Text TimerText;

    void Start()
    {
        PauseIsActive = false;
        DestroySelf = true;
        // =========================== //
        CrashVal = "CrashValue";
        SpawnVal = "SpawnValue";
        ScoreVal = "ScoreValue";
        //PlayerPref System
        PrefCrashes = PlayerPrefs.GetFloat(CrashVal);
        PrefSpawns = PlayerPrefs.GetFloat(SpawnVal);
        PrefScore = PlayerPrefs.GetFloat(ScoreVal);
        // Sets Text Values
        CrashScoreVal.text = PlayerPrefs.GetFloat(CrashVal).ToString();
        SpawnScoreVal.text = PlayerPrefs.GetFloat(SpawnVal).ToString();
        NewScoreVal.text = PlayerPrefs.GetFloat(ScoreVal).ToString();
        // =========================== //
        Time.timeScale = 0.0f;
        MainMenuIsActive = true;
    }

	void Update ()
    {
        if (TimerActivate == true) {
            if (TimeHolder > 0) {
                TimeHolder -= 1 * Time.deltaTime;
            }
            else if (TimeHolder <= 0) {
                TimeHolder = 0;
                isGameOver();
            }
        }

        if (MainMenuIsActive == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && PauseIsActive == false)
            {
                PauseIsActive = true;
                PauseMenu.gameObject.SetActive(true);
                Time.timeScale = 0.0f;
                GameUI.gameObject.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Escape) && PauseIsActive == true)
            {
                PauseIsActive = false;
                PauseMenu.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                GameUI.gameObject.SetActive(true);
            }
        }
        TimerText.text = TimeHolder.ToString();
	}

    public void LoadGame()
    {
        // Resets Values
        PlaneMan.Instance.m_CrashedAircraft = 0;
        PlaneMan.Instance.m_Score = 0;
        // Sets Remaining Var's
        TimeHolder = m_Time;
        TimerActivate = true;
        Time.timeScale = 1.0f;
        MainMenu.gameObject.SetActive(false);
        MainMenuIsActive = false;
        GameUI.gameObject.SetActive(true);
        DestroySelf = false;
    }    

    void UpdateScores()
    {
        if (PrefCrashes >= 0) // Crashes
        {
            PrefCrashes = PlaneMan.Instance.m_CrashedAircraft;   
            PlayerPrefs.SetFloat(CrashVal, PrefCrashes);
            PlayerPrefs.Save();
        }
        if (PrefScore >= 0) // Score
        {
            PrefScore = PlaneMan.Instance.m_Score;
            PlayerPrefs.SetFloat(ScoreVal, PrefScore);
            PlayerPrefs.Save();
        }

        if (PrefSpawns >= 0) // Spawns
        {
            PrefSpawns = PlaneMan.Instance.PlanesSpawned;
            PlayerPrefs.SetFloat(SpawnVal, PrefSpawns);
            PlayerPrefs.Save();
        }
        // Resets Text Variables
        CrashScoreVal.text = PlayerPrefs.GetFloat(CrashVal).ToString();
        SpawnScoreVal.text = PlayerPrefs.GetFloat(SpawnVal).ToString();
        NewScoreVal.text = PlayerPrefs.GetFloat(ScoreVal).ToString();
        PlayerPrefs.Save();
    }

    public void isGameOver()
    {
        if (MainMenuIsActive == false)
        {
            UpdateScores();
            Time.timeScale = 0.0f;
            MainMenu.gameObject.SetActive(true);
            MainMenuIsActive = true;
            GameUI.gameObject.SetActive(false);
            TimerActivate = false;
            DestroySelf = true;
        }
    }
    
    public void MainMenuLoader()
    {
        SceneManager.LoadScene("MainMenu");
    }
}