using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraMoveMVP : MonoBehaviour
{
    [Header("Buttons & Fade")]
    public Button StartButton;
    public Button QuitButton;
    public Image fadeImag;

    [Header("Toggles & Bools")]
    public Toggle TutorialToggle;
    public Toggle Level1Toggle;
    public Toggle Level2Toggle;

    public bool TutorialBool = false;
    public bool Level1Bool = false;
    public bool Level2Bool = false;

    [Header("Speeds")]
    public float FadeSpeed; 

    public float MoveForwardSpeed;
    public float MoveUpSpeed;
    public float MoveForwardTime;
    public float MoveUpTime;
    private float MovingForwardValue;
    private float MovingUpValue;

	void Start ()
    {
        fadeImag.color = Color.clear;
        fadeImag.gameObject.SetActive(false);

        Time.timeScale = 0;
        MovingForwardValue = MoveForwardTime;
        MovingUpValue = MoveUpSpeed;
	}
	
	void Update ()
    {
        if (TutorialToggle.isOn == false) { TutorialBool = false; }
        if (Level1Toggle.isOn == false) { Level1Bool = false; }
        if (Level2Toggle.isOn == false) { Level2Bool = false; }

        if (TutorialBool == true || Level1Bool == true || Level2Bool == true)
        {
            StartButton.interactable = true;
        }
        else
        {
            StartButton.interactable = false;
        }

        MovingForwardValue -= Time.deltaTime;
        MovingUpValue -= Time.deltaTime;

        // Moving Forward
        if (MovingForwardValue > 0)
        {   transform.Translate(Vector3.forward * MoveForwardSpeed * Time.deltaTime);
        }
        else
        { MovingForwardValue = 0;  
        }
        // Moving Up
        if (MovingUpValue > 0)
        {   transform.Translate(Vector3.up * MoveUpSpeed * Time.deltaTime);
        }
        else
        { MovingUpValue = 0;
        }
        // Game Start
        if (MovingForwardValue == 0 && MovingUpValue == 0)
        {
            fadeImag.gameObject.SetActive(true);
            fadeImag.color = Color.Lerp(fadeImag.color, Color.black, FadeSpeed * Time.deltaTime);
            if (fadeImag.color.a >= 0.95f)
            {
                if (TutorialBool == true)
                {
                    SceneManager.LoadScene("VolcanicTutorial");
                }
                if (Level1Bool == true)
                {
                    SceneManager.LoadScene("VolcanicIslands");
                }
                if (Level2Bool == true)
                {
                    SceneManager.LoadScene("VolcanicIslandsHard");
                }
            }
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        StartButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        TutorialToggle.gameObject.SetActive(false);
        Level1Toggle.gameObject.SetActive(false);
        Level2Toggle.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateToggle()
    {
        if (TutorialToggle.isOn == true)
        {
            Level1Toggle.isOn = false;
            Level2Toggle.isOn = false;
            TutorialBool = true;
        }
        if (Level1Toggle.isOn == true)
        {
            Level2Toggle.isOn = false;
            TutorialToggle.isOn = false;
            Level1Bool = true;
        }
        if (Level2Toggle.isOn == true)
        {
            Level1Toggle.isOn = false;
            TutorialToggle.isOn = false;
            Level2Bool = true;
        }
    }
}
