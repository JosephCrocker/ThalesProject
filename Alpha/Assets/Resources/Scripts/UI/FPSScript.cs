using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Code From http://forum.unity3d.com/threads/how-can-i-display-fps-on-android-device.386250/
/// </summary>
public class FPSScript : MonoBehaviour
{
    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;

    public Text FpsCounter;

    void Start ()
    {
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (;;)
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it
            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
        }
    }

    void Update ()
    {
        FpsCounter.text = fps;
    }
}
