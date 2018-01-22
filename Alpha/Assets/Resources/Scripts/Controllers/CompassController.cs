using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine.UI;
using EventHandler = Singleton<EventManager>;
using Camera = Singleton<CameraController>;
using Jarvis = Singleton<CoPilot>;
using PlaneMan = Singleton<PlaneManager>;

public class CompassController : MonoBehaviour
{
    public GameObject CompassArrow;
    // ================================= //
    public bool CamIsMoving;
    //public bool PlaneIsMoving;
    public Transform cameraTransform;
    // Compass Vars
    public float compassRotationOffset;
    public Image compassOverlay;
    // Heading
    private Vector3 compassHeading;
    private Vector3 planeHeading;
    private Vector3 cameraHeading;
    private Vector3 arrowHeading;
    // Rotations
    private Quaternion cameraDeltaRotation;
    private Quaternion compassDeltaRotation;
    private Quaternion arrowDeltaRotation;
    private Quaternion planeDeltaRotation;
    public float RotateSpeed;
    // Other Vars
    private ObservedValue<Vector3> angleFromNormal;
    private PlaneController currentSelected;
    // ================================= //

    private struct UpdateDirectionData 
    {
        public PlaneController plane;
        public Quaternion rotation;
        public Vector3 heading;
    }

    void Start()
    {
        cameraTransform = Camera.Instance.transform;
        compassRotationOffset = (360f - cameraTransform.rotation.eulerAngles.y);
        compassHeading = planeHeading = Vector3.zero;
        arrowHeading = planeHeading = Vector3.zero;
        angleFromNormal = new ObservedValue<Vector3>(Vector3.zero, FindOverlayAngle);
        EventHandler.Instance.AddListener(StringHandler.Events.InitializeSelection, InitializeSelection);
    }

    public void DragCompass()
    {
        if (PlaneMan.Instance.CurrentSelected)
        {
#if UNITY_ANDROID
            Vector3 GetTouchPos = Input.GetTouch(0).position;
            angleFromNormal.Value = (GetTouchPos - transform.position).normalized;
            // PC Debug
            //angleFromNormal.Value = (Input.mousePosition - transform.position).normalized;
#endif

#if UNITY_STANDALONE_WIN
            angleFromNormal.Value = (Input.mousePosition - transform.position).normalized;
#endif
            float UpDot = Vector3.Dot(angleFromNormal.Value, Vector3.up);
            float RightDot = Vector3.Dot(angleFromNormal.Value, Vector3.right); 
            float Tan = Mathf.Atan2(RightDot, UpDot);
            float Angle = Tan * Mathf.Rad2Deg;
            Quaternion Quert = Quaternion.Euler(0, Angle, 0);

            PlaneMan.Instance.CurrentSelected._rotationGhost.SetActive(true);
            PlaneMan.Instance.CurrentSelected._rotationGhost.transform.rotation = PlaneMan.Instance.CurrentSelected.transform.rotation * Quert;
        }
    }

    public void ReleaseCompass()
    {
        if (PlaneMan.Instance.CurrentSelected)
        {
            PlaneMan.Instance.CurrentSelected._rotationGhost.SetActive(false);
            currentSelected = PlaneMan.Instance.CurrentSelected;
            planeHeading = currentSelected.transform.eulerAngles;
            if (compassOverlay.fillClockwise)
            {
                SetPlaneHeading(Vector3.Angle(Vector3.up, angleFromNormal.Value));
                SetCompass(compassHeading.z + Vector3.Angle(Vector3.up, angleFromNormal.Value));
                return;
            }
            SetPlaneHeading(-(Vector3.Angle(Vector3.up, angleFromNormal.Value)));
            SetCompass(compassHeading.z - Vector3.Angle(Vector3.up, angleFromNormal.Value));
            return;
        }
    }

    public void SetCameraRotation(float _newRotation)
    {
        cameraHeading.x = cameraTransform.eulerAngles.x;
        cameraHeading.y = _newRotation;
        cameraDeltaRotation = Quaternion.Euler(cameraHeading);
        StartCoroutine(StringHandler.CoRoutines.UpdateCamera);
    }

    public void SetCompass(float _newRotation)
    {
        if (currentSelected)
        { currentSelected.LockRotation(true);
        }
        compassHeading.z = _newRotation;
        compassDeltaRotation = Quaternion.Euler(compassHeading);
        StartCoroutine(StringHandler.CoRoutines.RotateCompass);
    }

    public void SetPlaneArrow(float _newRotation)
    {
        arrowHeading.z = _newRotation;
        arrowDeltaRotation = Quaternion.Euler(arrowHeading);
        StartCoroutine(StringHandler.CoRoutines.RotateArrow);
    }

    public void ResetCompass()
    {
        compassHeading = Vector3.zero;
        compassDeltaRotation = Quaternion.Euler(compassHeading);
        StartCoroutine(StringHandler.CoRoutines.RotateCompass);
    }

    public void SetPlaneHeading(float _newRotation)
    {
        planeHeading.y += _newRotation;
        planeDeltaRotation = Quaternion.Euler(planeHeading);

        UpdateDirectionData newData;
        newData.heading = planeHeading;
        newData.plane = currentSelected;
        newData.rotation = planeDeltaRotation;

        StartCoroutine(StringHandler.CoRoutines.UpdatePlaneHeading, newData);
    }

    private void FindOverlayAngle(Vector3 oldValue, Vector3 newValue)
    {
        if (oldValue != newValue)
        {
            if (newValue.x >= 0 && !compassOverlay.fillClockwise)
            { compassOverlay.fillClockwise = true;
            }
            else if (newValue.x < 0 && compassOverlay.fillClockwise)
            { compassOverlay.fillClockwise = false;
            }
            float newAngle = Vector3.Angle(Vector3.up, newValue);
            compassOverlay.fillAmount = newAngle / 360;
        }
    }

    IEnumerator UpdateCamera()
    {
        CamIsMoving = true;
        while (Vector3.Distance(cameraTransform.localRotation.eulerAngles, cameraDeltaRotation.eulerAngles) > 0.2)
        {
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, cameraDeltaRotation, RotateSpeed * Time.deltaTime);
            yield return null;
        }
        cameraTransform.localRotation = cameraDeltaRotation;
        CamIsMoving = false;
    }

    IEnumerator UpdatePlaneHeading(UpdateDirectionData newData)
    {
        PlaneController plane = newData.plane;
        Quaternion deltaRotate = newData.rotation;
        Vector3 direction = newData.heading;

        //PlaneIsMoving = true;
        while (Vector3.Distance(plane.transform.eulerAngles, direction) > 4.5)
        {
            plane.transform.rotation = Quaternion.Slerp(plane.transform.rotation, deltaRotate, RotateSpeed * Time.deltaTime);
            yield return null;
        }
        //PlaneIsMoving = false;
        plane.transform.rotation = deltaRotate;
        plane.LockRotation(false);
        Jarvis.Instance.ReAssessEvents(plane);
        Jarvis.Instance.ReAssessCollisions(plane);
    }

    IEnumerator RotateCompass()
    {
        while (Vector3.Distance(transform.eulerAngles, compassDeltaRotation.eulerAngles) > 0.2)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, compassDeltaRotation, RotateSpeed * Time.deltaTime);
            compassOverlay.fillAmount = Mathf.Lerp(compassOverlay.fillAmount, 0, RotateSpeed * Time.deltaTime);
            yield return null;
        }
        compassOverlay.fillAmount = 0;
        transform.rotation = compassDeltaRotation;
    }

    IEnumerator RotateArrow()
    {
        while (Vector3.Distance(CompassArrow.transform.eulerAngles, arrowDeltaRotation.eulerAngles) > 0.2)
        {
            CompassArrow.transform.rotation = Quaternion.Slerp(CompassArrow.transform.rotation, arrowDeltaRotation, RotateSpeed * Time.deltaTime);
            yield return null;
        }
        CompassArrow.transform.rotation = arrowDeltaRotation;
    }

    public void StopCameraMovement()
    {
        StopCoroutine(StringHandler.CoRoutines.UpdateCamera);
    }

    public void StopPlaneRotate()
    {
        StopCoroutine(StringHandler.CoRoutines.UpdatePlaneHeading);
    }

    public void InitializeSelection(params object[] args)
    {
        PlaneController planeController = args[0] as PlaneController;
        if (planeController && planeController.isPriority == false)
        {
            // Rotates Camera to plane when selected
            //SetCameraRotation(planeController.transform.eulerAngles.y);
            //SetCompass(planeController.transform.eulerAngles.y + compassRotationOffset);
            SetPlaneArrow(planeController.transform.eulerAngles.y + compassRotationOffset);
        }
    }
}