using UnityEngine;
using System.Collections;
using PlaneMan = Singleton<PlaneManager>;
using Compass = Singleton<CompassController>;
using EventHandler = Singleton<EventManager>;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Speed at which to zoom the orthographic camera.
    /// </summary>
    public float m_orthoZoomSpeed;
    
    #region ROTATE
    /// <summary>
    /// Speed to rotate the camera.
    /// </summary>
    public float m_rotationSpeed; // = 2.5f;

    /// <summary>
    /// If the cameras orthographic size is smaller than 
    /// this value, it won't be able to rotate.
    /// </summary>
    public float m_cameraRotateBuffer; // = 25f;

    /// <summary>
    /// Vector to assign new rotation values to
    /// before assigning itself to the cameras rotation
    /// </summary>
    private Vector3 m_newRotation = Vector3.zero;
    private bool m_rotationLock = false;


#if UNITY_STANDALONE_WIN
    private Vector3 m_mouseReference;
    private Vector3 m_mouseOffset;
#endif
    #endregion

    #region CamVars
    /// <summary>
    /// Normalized Vector facing "true north".
    /// </summary>
    public Vector3 m_north;
    
    /// <summary>
    /// Reference to the Main Camera
    /// </summary>
    private Camera MainCamera;


    /// <summary>
    /// The direction of the touch is multiplied by this value.
    /// </summary>
    public float m_fingerSpeedSensitivity;

    /// <summary>
    /// Size of the orthographic camera when the scene is loaded.
    /// </summary>
    private float m_orthographicSize;

    /// <summary>
    /// Maximum Orthographic Size of the Camera.
    /// </summary>
    public float m_maxOrthographicSize = 35f;

    /// <summary>
    /// Minimum allowed size of camera.
    /// </summary>
    public float m_minOrthographicSize = 5f;
    #endregion

    /// <summary>
    /// Current size of the camera.
    /// </summary>
    ObservedValue<float> CurrentOrthoSize;

    /// <summary>
    /// Initial Y Rotation
    /// </summary>
    [HideInInspector]
    public float cameraOriginRotation;

#if UNITY_STANDALONE_WIN
    private float m_zoomSpeed; // = 5f;
    #endif

    public bool RotationLock
    {
        get { return m_rotationLock; }
        set { m_rotationLock = value; }
    }

    void Start()
    {
        MainCamera = Camera.main;
        cameraOriginRotation = transform.eulerAngles.y;
        m_orthographicSize = MainCamera.orthographicSize;
        m_north = transform.forward;
        CurrentOrthoSize = new ObservedValue<float>(MainCamera.orthographicSize, ClampCamera);
        //EventHandler.Instance.AddListener(StringHandler.Events.InitializeSelection, InitializeSelection);
        EventHandler.Instance.AddListener(StringHandler.Events.ReleaseSelection, ReleaseSelection);
    }

    void Update()
    {
        CurrentOrthoSize.Value = MainCamera.orthographicSize;
        HandleInput();
    }

#if UNITY_ANDROID
    //void Zoom()
    //{
    //    Touch touchZero = Input.GetTouch(0);
    //    Touch touchOne = Input.GetTouch(1);
    //
    //    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    //    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
    //
    //    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    //    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
    //    float deltaMagDif = prevTouchDeltaMag - touchDeltaMag;
    //
    //    if (MainCamera.orthographic)
    //    {
    //        MainCamera.orthographicSize += deltaMagDif * m_orthoZoomSpeed;
    //        MainCamera.orthographicSize = Mathf.Max(MainCamera.orthographicSize, 5f);
    //    }
    //    else
    //    {
    //        //MainCamera.fieldOfView += deltaMagDif * m_perspectiveZoomSpeed;
    //        MainCamera.fieldOfView = Mathf.Clamp(MainCamera.fieldOfView, 5f, 179.9f);
    //    }
    //}

    void Rotate()
    {
        if (!RotationLock && Compass.Instance.CamIsMoving == false)
        {
            Vector2 delta = Input.GetTouch(0).deltaPosition;
            m_newRotation.y = -(delta.x + delta.y) * m_rotationSpeed;
            transform.RotateAround(transform.position, Vector3.down, m_newRotation.y);
            Compass.Instance.SetCompass(transform.rotation.eulerAngles.y);
        }
    }

    void Translate()
    {
        if (!RotationLock)
        {
            Vector2 touchDelta = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).deltaPosition);
            transform.Translate(-touchDelta.x * m_fingerSpeedSensitivity, -touchDelta.y * m_fingerSpeedSensitivity, 0f, Space.Self);
        }
    }
#endif

    void HandleInput()
    {
#if UNITY_ANDROID
        switch (Input.touchCount)
        {
            case 1:
                if (MainCamera.orthographicSize > m_cameraRotateBuffer && !PlaneMan.Instance.m_selectionLock)
                {
                    Rotate();
                    break;
                }
                Translate();
                break;
            //case 2:
            //    Zoom();
            //    break;
            default:
                break;
        }
#endif

#if UNITY_STANDALONE_WIN
        // Camera Size - PC
        if (!PlaneMan.Instance.m_selectionLock )
        {
           if (Input.GetAxis("Mouse ScrollWheel") > 0f && MainCamera.orthographicSize != m_minOrthographicSize)
           { MainCamera.orthographicSize -= m_orthoZoomSpeed;
           }
           else if (Input.GetAxis("Mouse ScrollWheel") < 0f && MainCamera.orthographicSize != m_maxOrthographicSize)
           { MainCamera.orthographicSize += m_orthoZoomSpeed;
           }
        }

        if (!RotationLock)
        {
            if (MainCamera.orthographicSize > m_cameraRotateBuffer)
            {
                if (Input.GetMouseButtonDown(1) && Compass.Instance.CamIsMoving == false)
                { m_mouseReference = Input.mousePosition;
                }

                if (Input.GetMouseButton(1))
                {
                    m_mouseOffset = (Input.mousePosition - m_mouseReference);
                    m_newRotation.y = -(m_mouseOffset.x + m_mouseOffset.y) * m_rotationSpeed;
                    m_newRotation.x = 0;
                    transform.RotateAround(transform.position, Vector3.down, m_newRotation.y);
                    m_mouseReference = Input.mousePosition;
                    Compass.Instance.SetCompass(transform.rotation.eulerAngles.y);
                }
            }
            else
            {
                // Keyboard & Mouse - Windows Builds
                if (Input.GetMouseButton(1))
                {
                    if (Input.GetAxis("Mouse X") < 0)
                    { transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * m_zoomSpeed, 0.0f, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * m_zoomSpeed);
                    }
                    else if (Input.GetAxis("Mouse X") > 0)
                    { transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * m_zoomSpeed, 0.0f, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * m_zoomSpeed);
                    }
                }
            }
        }
#endif
    }

    public void ToggleRotationLock(bool _state)
    {
        RotationLock = _state;
    }

    public void Reset()
    {
        MainCamera.orthographicSize = m_orthographicSize;
        Compass.Instance.StopCameraMovement();
        Compass.Instance.SetCameraRotation(cameraOriginRotation);
        Compass.Instance.SetCompass(Compass.Instance.compassRotationOffset + cameraOriginRotation);
    }

    private void ClampCamera(float oldVal, float newVal)
    {
        if (oldVal != newVal)
        {
            // Camera Orth Size Check
            if (newVal > m_maxOrthographicSize)
            {
                MainCamera.orthographicSize = m_maxOrthographicSize;
            }
            if (newVal < m_minOrthographicSize)
            {
                MainCamera.orthographicSize = m_minOrthographicSize;
            }
        }
    }

    private void InitializeSelection(params object[] args)
    {
        ToggleRotationLock(true);
    }

    private void ReleaseSelection(params object[] args)
    {
        ToggleRotationLock(false);
    }
}