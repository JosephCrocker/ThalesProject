using UnityEngine;
//using System.Collections;

public class UIController : MonoBehaviour
{
    public float maxScaleSize;
    public float minScaleSize;

    public float animationSpeed;
    private bool dropDownActive = false;

    public void ToggleDropDown()
    {
        dropDownActive = !dropDownActive;
        
        if (dropDownActive)
        {
            DownAnimation();
        }
        else
        {
            UpAnimation();
        }
    }

    private void DownAnimation()
    {
        while(transform.localScale.y < maxScaleSize)
        {
            float currentY = transform.localScale.y + animationSpeed * Time.deltaTime;
            TransformExtensions.SetYScale(transform, currentY);
        }
        if (transform.localScale.y > maxScaleSize)
        {
            TransformExtensions.SetYScale(this.transform, maxScaleSize);
            return;
        }
    }

    private void UpAnimation()
    {
        while (transform.localScale.y > minScaleSize)
        {
            float currentY = transform.localScale.y - animationSpeed * Time.deltaTime;
            TransformExtensions.SetYScale(transform, currentY);
        }

        if (transform.localScale.y < minScaleSize)
        {
            TransformExtensions.SetYScale(transform, 0f);
            return;
        }
    }

    public void ToggleOff(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
    }

    public void ToggleOn(GameObject _gameObject)
    {
        _gameObject.SetActive(true);
    }
}