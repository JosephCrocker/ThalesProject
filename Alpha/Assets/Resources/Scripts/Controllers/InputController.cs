using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlaneMan = Singleton<PlaneManager>;
using Compass = Singleton<CompassController>;

/// <summary>
/// Handles all of the selected planes Inputs.
/// </summary>
public class InputController : MonoBehaviour
{
    [HideInInspector]
    public float GhostPosHolder;
    [HideInInspector]
    public bool StartHeightChange;
    // =========================== //
    // Public
    public float _rotateValue;
    public float _heightChange; // Rec 3
    // Private
    private Vector3 newPos;
    private Transform toRotation;
    // =========================== //

// =================== //
// Android Vars
#if UNITY_ANDROID && UNITY_IOS
    /// <summary>
    /// If true, update the heading.
    /// </summary>
    private bool m_updateHeading;

    /// <summary>
    /// How long the touch has been held for.
    /// </summary>
    private float m_touchDuration;

    /// <summary>
    /// Time before the user can start moving the planes heading.
    /// </summary>
    private float m_headingBufferTime;

    /// <summary>
    /// Start Time of touch.
    /// </summary>
    private float m_fStartTouchTime;

    /// <summary>
    /// End time of touch.
    /// </summary>
    private float m_fEndTouchTime;

    /// <summary>
    /// Position when the user first touches the screen.
    /// </summary>  
    private Vector3 m_vStartTouchPosition;
#endif
// =================== //

    void Start()
    {
#if UNITY_ANDROID && UNITY_IOS
        m_touchDuration = 0f;
        m_headingBufferTime = 0.5f;
        m_updateHeading = false;
#endif
    }

    // Manager For Android & PC 
    public void ListenForInput(PlaneController _currentSelected)
    {
#if UNITY_ANDROID && UNITY_IOS
        switch (Input.touchCount)
        {
            case 1:
                SingleTouch(_currentSelected);
                break;
            default:
                break;
        }
#endif

#if UNITY_STANDALONE_WIN
        //if (Input.GetMouseButtonDown(1))
        //{ UpdateHeading(_currentSelected);}

        TransformExtensions.SetXPos(_currentSelected.ghostObject.transform, _currentSelected.transform.position.x);
        TransformExtensions.SetZPos(_currentSelected.ghostObject.transform, _currentSelected.transform.position.z);
#endif
    }

    // =================== //
    // Android Functions    
#if UNITY_ANDROID && UNITY_IOS
    // Functions
    void SingleTouch(PlaneController _currentSelected)
    {
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            // Touch Start
            case TouchPhase.Began:
                m_vStartTouchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                m_fStartTouchTime = Time.time;
                break;

            // Touch Stationary
            case TouchPhase.Stationary:
                if (!m_updateHeading)
                {
                    m_touchDuration += Time.deltaTime;

                    if (m_touchDuration > m_headingBufferTime)
                    {
                        Handheld.Vibrate();
                        m_updateHeading = true;
                    }
                }
                break;

            // Touch Moved
            case TouchPhase.Moved:
                if (m_updateHeading)
                {
                    UpdateHeading(_currentSelected);
                    break;
                }
                UpdateSpeed(_currentSelected);
                break;

            // Touch Over
            case TouchPhase.Ended:
                m_touchDuration = 0f;
                m_updateHeading = false;
                break;
            default:
                break;
        }
    }
    void UpdateHeading(PlaneController _currentSelected)
    {
        Touch touch = Input.GetTouch(0);
        Vector3 newTouch = new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane);
        Ray ray = Camera.main.ScreenPointToRay(newTouch);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 499f))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                var mousePosition = hit.point;
                mousePosition.y = _currentSelected.transform.position.y;
                Vector3 direction = (mousePosition - _currentSelected.transform.position).normalized;
                _currentSelected.transform.forward = direction;
            }
        }
    }
    void UpdateSpeed(PlaneController _currentSelected)
    {
        m_fEndTouchTime = Time.time;
        Touch touch = Input.GetTouch(0);
        Vector3 newTouch = new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane);
        Ray ray = Camera.main.ScreenPointToRay(newTouch);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 499f))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                var mousePosition = hit.point;
                mousePosition.y = _currentSelected.transform.position.y;
                m_vStartTouchPosition.y = _currentSelected.transform.position.y;

                Vector3 direction = (mousePosition - _currentSelected.transform.position).normalized;
                float directionOfDrag = Vector3.Dot(_currentSelected.transform.forward, direction);
                float currentTime = m_fEndTouchTime - m_fStartTouchTime;
                float distance = (mousePosition - m_vStartTouchPosition).magnitude;
                var speed = distance / currentTime; //Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
                speed *= .0001f;

                if (directionOfDrag > 0)
                {
                    // Speed Up
                    if (_currentSelected.Speed < _currentSelected.maxSpeed)
                    { _currentSelected.Speed += speed;
                    }
                    else
                    {
                        _currentSelected.Speed = _currentSelected.maxSpeed;
                    }
                }
                else if (directionOfDrag < 0)
                {
                    // Slow Down
                    if (_currentSelected.Speed > _currentSelected.minSpeed)
                    { _currentSelected.Speed -= speed;
                    }
                    else
                    {
                        _currentSelected.Speed = _currentSelected.minSpeed;
                    }
                }
            }
        }
    }
#endif
    // =================== //

    // PC Functions
#if UNITY_STANDALONE_WIN
    // Heading
    void UpdateHeading(PlaneController _currentSelected)
    {
        Vector3 newTouch = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Ray ray = Camera.main.ScreenPointToRay(newTouch);
        RaycastHit hit;
    
        if (Physics.Raycast(ray, out hit, 499f))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                var mousePosition = hit.point;
                mousePosition.y = _currentSelected.transform.position.y;
                Vector3 direction = (mousePosition - _currentSelected.transform.position).normalized;
                _currentSelected.transform.forward = direction;
                //_currentSelected.SetManualOverride(true);
                Compass.Instance.SetCompass(_currentSelected.transform.rotation.eulerAngles.y);
            }
        }
    }
#endif
}