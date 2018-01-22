using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using System.Collections;
using Camera = Singleton<CameraController>;

public class GameManager : MonoBehaviour
{
    public EffiScript ScoreData;

    public Transform GameOverScreen;
    public Scene CurrentSceneName;
    private string SceneName;

    private float ShiftTime;
    public int ShiftHours;

    public float countDown;
    public Text ShiftTextBox;

    void Start()
    {
        CurrentSceneName = SceneManager.GetActiveScene();
        SceneName = CurrentSceneName.name;
        Time.timeScale = 1;

        ShiftTime = 60.00f;
        countDown = ShiftTime;
    }

    void Update()
    {
        if (countDown > 0)
        {
            countDown -= 1 * Time.deltaTime;
        }
        else if (countDown <= 0 && GameOverScreen.gameObject.activeSelf == false)
        {
            ShiftHours -= 1;
            if (ShiftHours < 0)
            {
                countDown = 0;
                Time.timeScale = 0;
                Camera.Instance.ToggleRotationLock(true);
                ShiftHours = 0;
                GameOverScreen.gameObject.SetActive(true);
            }
            else
            {
                countDown = ShiftTime;
            }
        }
        ShiftTextBox.text = ShiftHours.ToString() + ":" + Mathf.Round(countDown).ToString();
    }

    public void LoadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneName);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("SplashMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TurnGameOverOff()
    {
        GameOverScreen.gameObject.SetActive(false);
    }
}