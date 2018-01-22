using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Camera = Singleton<CameraController>;
using PlaneMan = Singleton<PlaneManager>;

public class PauseScript : MonoBehaviour
{
    public Transform PauseMenu;
    public bool isActive;

	void Start ()
    {
        isActive = false;
	}
	
	void Update ()
    {
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isActive == false)
            {
                PlaneMan.Instance.PauseIsActive = true;
                Camera.Instance.ToggleRotationLock(true);
                Time.timeScale = 0;
                PauseMenu.gameObject.SetActive(true);
                isActive = true;
            }
            else if (isActive == true)
            {
                PlaneMan.Instance.PauseIsActive = false;
                Camera.Instance.ToggleRotationLock(false);
                Time.timeScale = 1;
                PauseMenu.gameObject.SetActive(false);
                isActive = false;
            }
        }
#endif
    }

    public void PauseButton()
    {
        if (isActive == false)
        {
            PlaneMan.Instance.PauseIsActive = true;
            Camera.Instance.ToggleRotationLock(true);
            Time.timeScale = 0;
            PauseMenu.gameObject.SetActive(true);
            isActive = true;
        }
        else if (isActive == true)
        {
            PlaneMan.Instance.PauseIsActive = false;
            Camera.Instance.ToggleRotationLock(false);
            Time.timeScale = 1;
            PauseMenu.gameObject.SetActive(false);
            isActive = false;
        }
    }
}
