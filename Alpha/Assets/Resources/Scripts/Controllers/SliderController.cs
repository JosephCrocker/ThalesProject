using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine.UI;
using EventHandler = Singleton<EventManager>;
using Jarvis = Singleton<CoPilot>;
using PlaneMan = Singleton<PlaneManager>;

public class SliderController : MonoBehaviour
{
    public ScrollRect scrollSlider;
    public Slider speedSlider;
    public Slider altitudeSlider;

    public float heightSpeed;
    public float accelerationSpeed;

    private ObservedValue<float> speedDelta;
    private ObservedValue<float> heightDelta;

    private float initialSpeed;
    private float initialHeight;

    PlaneController currentSelected;
    public float lerpBufferRange;

    private struct UpdateHeightData
    {
        public float newHeight;
        public PlaneController plane;
    }

    void Start()
    {
        scrollSlider = GameObject.Find("AltitudeSlider").GetComponent<ScrollRect>();
        scrollSlider.enabled = false;
        EventHandler.Instance.AddListener(StringHandler.Events.InitializeSelection, InitializeSelection);
        EventHandler.Instance.AddListener(StringHandler.Events.ReleaseSelection, ReleaseSelection);
        speedDelta = new ObservedValue<float>(0.0f, SpeedChange);
        heightDelta = new ObservedValue<float>(0.0f, HeightChange);
        altitudeSlider.interactable = false;
        speedSlider.interactable = false;
    }

    public void InitializeSelection(params object[] args)
    {
        currentSelected = args[0] as PlaneController;
        if (currentSelected.isPriority == false)
        {
            // Sliders Turning On 
            altitudeSlider.interactable = true;
            speedSlider.interactable = true;
            // Slider Min/Max Setting
            speedSlider.minValue = currentSelected.minSpeed;
            speedSlider.maxValue = currentSelected.maxSpeed;
            altitudeSlider.minValue = currentSelected.MinPlaneAltitude;
            altitudeSlider.maxValue = currentSelected.MaxPlaneAltitude;
            // Slider Value Set
            SetSliderValues(currentSelected.Speed, currentSelected.WorldToPlaneHeight(currentSelected.transform.position.y));
        }
    }

    private void ReleaseSelection(params object[] args)
    {
        altitudeSlider.interactable = false;
        speedSlider.interactable = false;
    }

    public void PositionGhost()
    {
        if (PlaneMan.Instance.CurrentSelected)
        {
            scrollSlider.enabled = true;
            altitudeSlider.interactable = true;
            currentSelected.ghostObject.transform.position = currentSelected.transform.position;
            currentSelected.ghostObject.transform.rotation = currentSelected.transform.rotation;
        }
    }

    private void SetSliderValues(float _speed, float _altitude)
    {
        speedSlider.value = _speed;
        altitudeSlider.value = _altitude;

        initialSpeed = _speed;
        initialHeight = _altitude;

        heightDelta.Value = initialHeight;
        speedDelta.Value = initialSpeed;

        currentSelected.ghostObject.transform.position = currentSelected.transform.position;
    }

    private void HeightChange(float old, float newValue)
    {
        if (old != newValue && newValue != initialHeight)
        {
            UpdateHeightData data;
            data.newHeight = newValue;
            data.plane = currentSelected;
            StartCoroutine(StringHandler.CoRoutines.UpdateHeight, data);
        }
    }

    // Slider Drag Function
    public void DragAltitude()
    {
        if (currentSelected)
        {
            if (currentSelected.PlaneToWorldHeight(altitudeSlider.value) != currentSelected.transform.position.y)
            {
                if (!currentSelected.ghostMesh.enabled)
                { currentSelected.ghostMesh.enabled = true;
                }
            }
            else
            {
                currentSelected.ghostMesh.enabled = false;
            }
            TransformExtensions.SetYPos(currentSelected.ghostObject.transform, currentSelected.PlaneToWorldHeight(altitudeSlider.value));
        }
    }

    private void SpeedChange(float old, float newValue)
    {
        if (old != newValue && newValue != initialSpeed)
        {
            StartCoroutine(StringHandler.CoRoutines.UpdateSpeed, newValue);
        }
    }

    // Slider Pointer Up Functions
    public void ReleaseHeight()
    {
        if (currentSelected)
        {
            //StopCoroutine(StringHandler.CoRoutines.UpdateHeight);
            heightDelta.Value = altitudeSlider.value;
            currentSelected.ghostMesh.enabled = false;
        }
    }
    public void ReleaseSpeed()
    {
        if (currentSelected)
        {
            StopCoroutine(StringHandler.CoRoutines.UpdateSpeed);
            speedDelta.Value = speedSlider.value;
        }
    }

    IEnumerator UpdateHeight(UpdateHeightData data)
    {
        float newHeight = data.newHeight;
        PlaneController plane = data.plane;

        while (Mathf.Abs(plane.transform.position.y - plane.PlaneToWorldHeight(newHeight)) > lerpBufferRange)
        {
                float updatedHeight = Mathf.MoveTowards(plane.transform.position.y, plane.PlaneToWorldHeight(newHeight), heightSpeed * Time.deltaTime);
                TransformExtensions.SetYPos(plane.transform, updatedHeight);
                plane.CurrentHeight = plane.WorldToPlaneHeight(updatedHeight);
                Jarvis.Instance.ReAssessEvents(plane);
                yield return null;
        }
        TransformExtensions.SetYPos(plane.transform, plane.PlaneToWorldHeight(newHeight));
        Jarvis.Instance.ReAssessCollisions(plane);
    }

    IEnumerator UpdateSpeed(float newSpeed)
    {
        while (Mathf.Abs(currentSelected.Speed - newSpeed) > lerpBufferRange)
        {
            float updatedSpeed = Mathf.MoveTowards(currentSelected.Speed, newSpeed, accelerationSpeed * Time.deltaTime);
            currentSelected.Speed = updatedSpeed;
            yield return null;
        }

        currentSelected.Speed = newSpeed;

        Jarvis.Instance.ReAssessEvents(currentSelected);
        Jarvis.Instance.ReAssessCollisions(currentSelected);
    }

    // Resets Slider Values
    public void ResetSliders()
    {
        speedSlider.value = 0;
        altitudeSlider.value = 300;
        if (currentSelected)
        {
            currentSelected.ghostMesh.enabled = false;
            altitudeSlider.interactable = false;
            scrollSlider.enabled = false;
        }
    }
}