using UnityEngine;
using UnityEngine.UI;
//using System.Collections;
using System.Collections.Generic;
using EventHandler = Singleton<EventManager>;
using PlaneMan = Singleton<PlaneManager>;

/// <summary>
/// The brains of all the collision prediction.
/// </summary>
public class CoPilot : MonoBehaviour
{
    // Height Of how far apart the notification icons are.
    public float PCNotificationOffset;
    public float AndroidNotificationOffset;
    [HideInInspector]
    public float m_notificationOffset;

    /// <summary>
    /// How high apart planes can be before
    /// a collision check is early exited.
    /// If the parent prefab of the plane
    /// isn't set to (0,0,0) this can
    /// corrupt the data.
    /// </summary>
    public float heightCollisionBuffer;

    /// <summary>
    /// The prefab of the crash event to
    /// be instantiated.
    /// </summary>
    [Header("UI Prefabs")]
    public GameObject crashEventPrefab;
    public GameObject weatherEventPrefab;
    public GameObject volcanicEventPrefab;
    public GameObject vipEventPrefab;
    public GameObject heliEventPrefab;
    public GameObject severeWeatherPrefab;

    /// <summary>
    /// Auto generated at run time.
    /// Displays a list of the current 
    /// events.
    /// </summary>
    [HideInInspector]
    public List<GameObject> EventGameObjects;
    public List<CoPilotEvent> Events;

    /// <summary>
    /// The transform to spawn the notifications
    /// into. 
    /// </summary>
    public Transform EventTransform;

    public GameObject alertButton;
    public GameObject alertNotification;
    public Text alertNumber;

    /// <summary>
    /// Used for opening and closing the
    /// currently selected event.
    /// </summary>
    [HideInInspector]
    public CoPilotEvent CurrentSelectedEvent;

    /// <summary>
    /// Reassess' an event after its been created
    /// to make sure it's still going to happen.
    /// </summary>
    /// <param name="_event"></param>
    public void AssessEvent(CoPilotEvent _event)
    {
        CrashEvent crashEvent = _event as CrashEvent;
        if (crashEvent)
        {
            PlaneController aPlane = crashEvent.firstPlane;
            PlaneController bPlane = crashEvent.secondPlane;

            // If they are too far apart don't bother
            // with the rest of the check.
            if (!IsWithinAltitude(aPlane, bPlane))
            {
                // Remove the event if the planes are too far
                // apart.
                aPlane.Warning.gameObject.SetActive(false);
                bPlane.Warning.gameObject.SetActive(false);
                ResetCrashEvents(crashEvent);
                return;
            }

            // Actually check if there is going to be a collision
            float collisionTime = CalculateCollision(aPlane, bPlane);
            if (collisionTime == -1.0f)
            {
                // If there isn't going to be a collision now,
                // remove the crash event.
                aPlane.Warning.gameObject.SetActive(false);
                bPlane.Warning.gameObject.SetActive(false);
                ResetCrashEvents(crashEvent);
            }
            return;
        }
    }

    // Assesses a collision between two planes.
    public void AssessCollision(PlaneController _aPlane, PlaneController _bPlane)
    {
        // If the planes aren't close to each other (height wise)
        // don't bother continuing a collision check.
        if (!IsWithinAltitude(_aPlane, _bPlane))
        { return;
        }

        Vector2 aDir = new Vector2(_aPlane.transform.forward.x, _aPlane.transform.forward.z);
        Vector2 aVel = aDir * _aPlane.Speed;
        float collisionTime = CalculateCollision(_aPlane, _bPlane);

        if (collisionTime == -1.0f || collisionTime > 13)
        { return;
        }

        if (_aPlane.transform.parent.gameObject.activeSelf && _bPlane.transform.parent.gameObject.activeSelf)
        {
            // Point of collision in the world, not being used currently - but could place an object
            // at finalPos to show where the planes will collide.
            Vector2 finalPos = new Vector2(_aPlane.transform.position.x, _aPlane.transform.position.z);
            finalPos += (aVel * collisionTime);
            // Create a crash event.
            if (_aPlane.CoPilotEvents.Count != 2 && _bPlane.CoPilotEvents.Count != 2)
            {
                InitializeCrashEvent(_aPlane, _bPlane, collisionTime);
            }
        }
    }
    private float CheckPlaneSeverity(PlaneController _plane, float _timeTillCollision)
    {
        // TODO: Actually make a proper fitness checker.
        if (_timeTillCollision < _plane.m_timeTillCollision)
        {
            return 1.0f;
        }
        return 0.0f;
    }

    private void InitializeCrashEvent(PlaneController _a, PlaneController _b, float _timeTillCollision)
    {
        float severity = CheckPlaneSeverity(_a, _timeTillCollision) * CheckPlaneSeverity(_b, _timeTillCollision);
        // If this is the most severe event for both planes - create it.
        if (severity == 1)
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.AlertNotify);
            GameObject newEventGO = Instantiate(crashEventPrefab) as GameObject;
            CrashEvent newCrash = newEventGO.GetComponent<CrashEvent>();

            // Initialze the new crash event.
            newCrash.Initialize(_a, _b, _timeTillCollision);

            // Make it a child of the events transform
            newEventGO.transform.SetParent(EventTransform);

            // Add to the list of the events
            Events.Add(newCrash);
            EventGameObjects.Add(newEventGO);

            // Move the new event game object to the alert notification
            // buttons position
            newEventGO.transform.position = alertButton.transform.position;

            // Then offset it downwards. Whatever
            float newY = newEventGO.transform.position.y - (EventGameObjects.Count * m_notificationOffset);
            TransformExtensions.SetYPos(newEventGO.transform, newY);

            // Make sure it doesn't scale to the parent.
            newEventGO.transform.localScale = new Vector3(1f, 1f, 1f);
            
            // Make the notification icon show with the number of events.
            alertNotification.SetActive(true);
            alertNumber.text = Events.Count.ToString();

            // Add this event to each planes list of current events.
            _a.AddCoPilotEvent(newCrash);
            _b.AddCoPilotEvent(newCrash);

            _a.Warning.gameObject.SetActive(true);
            _b.Warning.gameObject.SetActive(true);

            // Initialize the collisions.
            _a.InitializeCollision(_timeTillCollision);
            _b.InitializeCollision(_timeTillCollision);
        }
    }

    // Wrapper around calling the reset event.
    public void ResetIcons()
    {
        EventHandler.Instance.DoEvent("ResetIcons", CurrentSelectedEvent);
    }

    void Start()
    {
#if UNITY_ANDROID
        m_notificationOffset = AndroidNotificationOffset;
#endif
#if UNITY_STANDALONE_WIN
        m_notificationOffset = PCNotificationOffset;
#endif

        EventGameObjects = new List<GameObject>();
        Events = new List<CoPilotEvent>();
        alertNotification.SetActive(false);
    }

    // When a plane is spawned it calls this collision check function.
    public void InitialCollisionCheck(PlaneController _aPlane, PlaneController _bPlane)
    {
        PathController pathA = _aPlane.Path;
        PathController pathB = _bPlane.Path;

        // Check if there are intersecting nodes between
        // the path of the first plane and the path of the second plane.
        if (pathA.IntersectingPaths.ContainsKey(pathB))
        {
            List<NodeController> pathAIntersectionNodes = pathA.IntersectingPaths[pathB];
            List<NodeController> pathBIntersectionNodes = pathB.IntersectingPaths[pathA];
            for (int i = 0; i < pathAIntersectionNodes.Count; ++i)
            {
                float arrivalTime = pathBIntersectionNodes[i].GetArrivalTime(_bPlane.planeName);
                // If the second plane is currently further into its traversal time
                // than it would have been when it arrived at the intersecting node. It can't collide there.
                if (_bPlane.flyingTime > arrivalTime) { continue; }
        
                // Check to see how far away the second plane is until reaching the intersection point.
                float planeBTimeTillIntersection = arrivalTime - _bPlane.flyingTime;
        
                // If the currently spawned plane is going to reach that intersection node
                // at the same time as the second plane...
                int collisionOffset = (int)(planeBTimeTillIntersection - pathAIntersectionNodes[i].GetArrivalTime(_aPlane.planeName));
                if (collisionOffset == 0 && _aPlane.CoPilotEvents.Count != 2 && _bPlane.CoPilotEvents.Count != 2)
                {   // ...Initial the crash event.
                    InitializeCrashEvent(_aPlane, _bPlane, planeBTimeTillIntersection);
                    return;
                }
            }
        }
        // If the two planes don't have intersecting nodes on their paths
        // just do a normal collision check.
        AssessCollision(_aPlane, _bPlane);
    }

    // Checks the difference in height between both planes are close.
    private bool IsWithinAltitude(PlaneController _aPlane, PlaneController _bPlane)
    {
        if (Mathf.Abs(_aPlane.transform.position.y - _bPlane.transform.position.y) < heightCollisionBuffer)
        {
            return true;
        }
        return false;
    }

    // Removes the crash event and resets the list of alerts.
    public void ResetCrashEvents(CrashEvent _crashEvent)
    {
        PlaneController aPlane = _crashEvent.firstPlane;
        PlaneController bPlane = _crashEvent.secondPlane;

        aPlane.RemoveCoPilotEvent(_crashEvent);
        bPlane.RemoveCoPilotEvent(_crashEvent);

        if (aPlane.CoPilotEvents.Count == 0)
        { aPlane.DefaultMaterial();
        }

        if (bPlane.CoPilotEvents.Count == 0)
        { bPlane.DefaultMaterial();
        }

        Events.Remove(_crashEvent);
        EventGameObjects.Remove(_crashEvent.gameObject);
        alertNumber.text = Events.Count.ToString();
        Destroy(_crashEvent.gameObject);

        for (int i = 0; i < Events.Count; ++i)
        {
            EventGameObjects[i].transform.position = alertButton.transform.position;
            float newY = EventGameObjects[i].transform.position.y - ((i + 1) * m_notificationOffset);
            TransformExtensions.SetYPos(EventGameObjects[i].transform, newY);
        }
    }

    // Does fancy math
    // Source: http://home.fnal.gov/~neilsen/publications/demon/node13.html
    private float CalculateCollision(PlaneController _aPlane, PlaneController _bPlane)
    {
        Vector2 _a = new Vector2(_aPlane.transform.position.x, _aPlane.transform.position.z);
        Vector2 _b = new Vector2(_bPlane.transform.position.x, _bPlane.transform.position.z);

        Vector2 aDir = new Vector2(_aPlane.transform.forward.x, _aPlane.transform.forward.z);
        Vector2 bDir = new Vector2(_bPlane.transform.forward.x, _bPlane.transform.forward.z);

        Vector2 aVel = aDir * _aPlane.Speed;
        Vector2 bVel = bDir * _bPlane.Speed;

        double aXVelSquare = aVel.x * aVel.x;
        double aYVelSquare = aVel.y * aVel.y;

        double bXVelSquare = bVel.x * bVel.x;
        double bYVelSquare = bVel.y * bVel.y;

        double a = aYVelSquare + bYVelSquare + aXVelSquare + bXVelSquare - (2 * (aVel.y * bVel.y)) - (2 * (aVel.x * bVel.x));
        double b = 2 * ((aVel.y * _a.y) - (_b.y * aVel.y) - (_a.y * bVel.y) + (bVel.y * _b.y) + (aVel.x * _a.x) - (_b.x * aVel.x) - (_a.x * bVel.x) + (bVel.x * _b.x));
        double c = ((_a.x * _a.x) - 2 * (_b.x * _a.x) + (_b.x * _b.x) + (_a.y * _a.y) - 2 * (_a.y * _b.y) + (_b.y * _b.y) - 1);

        double sqrtpart = b * b - 4 * a * c;
        double x1, x2;
        if (sqrtpart > 0)
        {
            x1 = (-b + System.Math.Sqrt(sqrtpart)) / (2 * a);
            x2 = (-b - System.Math.Sqrt(sqrtpart)) / (2 * a);

            if (x1 > 0 && x2 < 0)
                return (float)x1;
            else if (x2 > 0 && x1 < 0)
                return (float)x2;
            else if (x1 > 0 && x2 > 0)
                return Mathf.Min((float)x1, (float)x2);
            else
                return -1.0f;
        }
        return -1.0f;
    }

    // Reassess' all of this planes current events.
    public void ReAssessEvents(PlaneController currentSelected)
    {
        int count = currentSelected.CoPilotEvents.Count;
        for (int i = 0; i < currentSelected.CoPilotEvents.Count; ++i)
        {
            if (count != currentSelected.CoPilotEvents.Count)
            {
                i--;
                Debug.Log("Out Of Range");
                AssessEvent(currentSelected.CoPilotEvents[i]);
                count = currentSelected.CoPilotEvents.Count;
                continue;
            }
            AssessEvent(currentSelected.CoPilotEvents[i]);
        }
    }

    // Checks all of this currents planes collisions.
    public void ReAssessCollisions(PlaneController currentSelected)
    {
        for (int i = 0; i < PlaneMan.Instance.EnabledPlanes.Count; ++i)
        {
            if (PlaneMan.Instance.EnabledPlanes[i] != currentSelected)
            {
                AssessCollision(currentSelected, PlaneMan.Instance.EnabledPlanes[i]);
            }
        }
    }
}