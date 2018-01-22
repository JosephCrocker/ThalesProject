using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DelayedMenuLoad : MonoBehaviour 
{
    #if UNITY_STANDALONE_WIN
    public MovieTexture startAnimation;
    #endif

    public Image image1;
    public Image image2;
    public Button start;

	void Start () 
    {
        //#if UNITY_STANDALONE_WIN
        //Renderer r = GetComponent<Renderer>();
        //#endif

        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(1.5f);
        image1.gameObject.SetActive(false);
        image2.gameObject.SetActive(true);
        start.gameObject.SetActive(true);
    }
	
    public void PlayAnimation()
    {
#if UNITY_STANDALONE_WIN
        startAnimation.Play();
#endif

        image1.gameObject.SetActive(false);
        image2.gameObject.SetActive(false);
        start.gameObject.SetActive(false);

#if UNITY_STANDALONE_WIN
        if (startAnimation.isPlaying )
        {
            StartCoroutine(NowLoad());
        }
#endif
#if UNITY_ANDROID
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
#endif
    }

    IEnumerator NowLoad()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenu");
    }
}
