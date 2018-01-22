using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using PlaneMan = Singleton<PlaneManager>;
using Camera = Singleton<CameraController>;

public class TutorialScript : MonoBehaviour
{
    public Transform PauseMenu;
    public bool isActive;

    [Header("Bools")]
    public bool ReturnedIsPressed;
    public bool FirstTipCompleted;
    public bool SecondTipCompleted;
    public bool ThirdTipCompleted;

    [Header("Buttons")]
    public Button HeightHelp;
    public Button DirectionHelp;
    public Button ReturnHelp;
    public Button TapHelp;

    [Header("UI Props")]
    public Button Compass;
    public Slider AltitudeSlider;
    public GameObject ContinueScreen;

    [Header("Gates")]
    public GameObject HeightGate;
    public GameObject DirectionGate;

    private float TimeTillTip;
    private int StopRecurNum;
    private int count = 0;

    void Start ()
    {
        Time.timeScale = 0;
        ReturnedIsPressed = false;
        FirstTipCompleted = false;
        SecondTipCompleted = false;
        ThirdTipCompleted = false;
        StopRecurNum = 0;
        TimeTillTip = 1.5f;
        //TimeTillEnd = 3;
    }
	
	void Update ()
    {
        if (PlaneMan.Instance.m_PlanesMadeItCount >= 2 && PlaneMan.Instance.TutorialHit < 2)
        { SceneManager.LoadScene("VolcanicTutorial"); }

        if (TimeTillTip > 0)
        { TimeTillTip -= 1 * Time.deltaTime; }
        else if (TimeTillTip < 0)
        {   Time.timeScale = 0;
            HeightHelp.gameObject.SetActive(true);
            TimeTillTip = 0;
        }
        if (HeightGate.gameObject.activeSelf == false && StopRecurNum == 0)
        {
            Time.timeScale = 0;
            DirectionHelp.gameObject.SetActive(true);
            DirectionGate.gameObject.SetActive(true);
            StopRecurNum += 1;
        }
        if (DirectionGate.gameObject.activeSelf == false && StopRecurNum == 1)
        {
            Time.timeScale = 0;
            ReturnHelp.gameObject.SetActive(true);
            StopRecurNum += 1;
        }
        if (ReturnedIsPressed == true && PlaneMan.Instance.m_PlanesMadeItCount >= 2 &&  PlaneMan.Instance.TutorialHit >= 2)
        {
            Time.timeScale = 0;
            ContinueScreen.gameObject.SetActive(true);
            Camera.Instance.ToggleRotationLock(true);
        }
        if (PlaneMan.Instance.m_CrashedAircraft >= 1)
        { SceneManager.LoadScene("VolcanicTutorial"); }
        if (PlaneMan.Instance.CurrentSelected != null && count == 0)
        {   TapResume();
            count += 1;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isActive == false)
            {
                PlaneMan.Instance.PauseIsActive = true;
                Camera.Instance.ToggleRotationLock(true);
                if (TapHelp.gameObject.activeSelf == false
                    && HeightHelp.gameObject.activeSelf == false
                    && DirectionHelp.gameObject.activeSelf == false)
                { Time.timeScale = 0; }
                PauseMenu.gameObject.SetActive(true);
                isActive = true;
            }
            else if (isActive == true)
            {
                PlaneMan.Instance.PauseIsActive = false;
                Camera.Instance.ToggleRotationLock(false);
                if (TapHelp.gameObject.activeSelf == false
                    && HeightHelp.gameObject.activeSelf == false
                    && DirectionHelp.gameObject.activeSelf == false)
                { Time.timeScale = 1; }
                PauseMenu.gameObject.SetActive(false);
                isActive = false;
            }
        }
    }

    public void TapResume()
    {
        if (PlaneMan.Instance.CurrentSelected != null)
        {
            Time.timeScale = 1;
            TapHelp.gameObject.SetActive(false);
            FirstTipCompleted = true;
        }
    }

    public void HeightResume()
    {
        if (FirstTipCompleted == true && PlaneMan.Instance.CurrentSelected != null
            && SecondTipCompleted == false && HeightHelp.gameObject.activeSelf == true)
        {
            Time.timeScale = 1;
            HeightHelp.gameObject.SetActive(false);
            SecondTipCompleted = true;
        }
    }

    public void DirectionResume()
    {
        if (SecondTipCompleted == true && PlaneMan.Instance.CurrentSelected != null
            && ThirdTipCompleted == false && DirectionHelp.gameObject.activeSelf == true)
        {
            Time.timeScale = 1;
            DirectionHelp.gameObject.SetActive(false);
            ThirdTipCompleted = true;
        }
    }

    public void ReturnResume()
    {
        if (ThirdTipCompleted == true && PlaneMan.Instance.CurrentSelected != null
            && ReturnHelp.gameObject.activeSelf == true)
        {
            Time.timeScale = 1;
            ReturnHelp.gameObject.SetActive(false);
        }
    }

    public void CheckIfPressed()
    {
        ReturnedIsPressed = true;
    }

    public void LoadMap()
    {
        SceneManager.LoadScene("VolcanicIslands");
    }

    public void RestartMap()
    {
        SceneManager.LoadScene("VolcanicTutorial");
    }

    public void PauseButton()
    {
        if (isActive == false )
        {
            PlaneMan.Instance.PauseIsActive = true;
            Camera.Instance.ToggleRotationLock(true);
            if (TapHelp.gameObject.activeSelf == false 
                && HeightHelp.gameObject.activeSelf == false
                && DirectionHelp.gameObject.activeSelf == false)
            {
                Time.timeScale = 0;
            }
            PauseMenu.gameObject.SetActive(true);
            isActive = true;
        }
        else if (isActive == true)
        {
            PlaneMan.Instance.PauseIsActive = false;
            Camera.Instance.ToggleRotationLock(false);
            if (TapHelp.gameObject.activeSelf == false
                && HeightHelp.gameObject.activeSelf == false
                && DirectionHelp.gameObject.activeSelf == false)
            {
                Time.timeScale = 1;
            }
            PauseMenu.gameObject.SetActive(false);
            isActive = false;
        }
    }
}