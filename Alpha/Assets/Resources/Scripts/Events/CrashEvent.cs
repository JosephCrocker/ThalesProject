using UnityEngine;
using System.Collections;
using EventHandler = Singleton<EventManager>;
using PlaneMan = Singleton<PlaneManager>;
using Jarvis = Singleton<CoPilot>;

/// <summary>
/// Crash Event, a type of CoPilotEvent.
/// </summary>
public class CrashEvent : CoPilotEvent
{
    // Stores a reference to both planes in the event.
    public PlaneController firstPlane;
    public PlaneController secondPlane;

    public GameObject currentThreat;

    public float severity;
    public float timeTillCollision;

    // Polls events every this amount of seconds.
    //private float timer = 3.0f;
    //private bool startTimer = false;

    void Start()
    {
        EventHandler.Instance.AddListener("ResetIcons", CloseDisplay);
    }

    void Update()
    {
        //if (startTimer)
        //{ timer -= Time.deltaTime;
        //  if (timer <= 0)
        //  { Jarvis.Instance.AssessCollision(firstPlane, secondPlane, this);
        //    timer = 2.0f;
        //  }
        //}
    }

    // Sets 
    public void Initialize(PlaneController _firstPlane, PlaneController _secondPlane, float _timeTillCollision)
    {
        firstPlane = _firstPlane;
        secondPlane = _secondPlane;
        timeTillCollision = _timeTillCollision;
    }

    public void InitializeNewThreat(PlaneController _plane, GameObject _threat)
    {
        firstPlane = _plane;
        currentThreat = _threat;
    }

    public void SelectPlaneOne()
    {
        PlaneMan.Instance.CurrentSelected = firstPlane;
    }

    public void SelectPlaneTwo()
    {
        PlaneMan.Instance.CurrentSelected = secondPlane;
    }

    private void CloseDisplay(params object[] args)
    {
        CrashEvent crashEvent = args[0] as CrashEvent;
        if (crashEvent == this)
        {
            transform.Find("Data").gameObject.SetActive(false);
        }
    }

    public void ShowData()
    {
        if (Jarvis.Instance.CurrentSelectedEvent)
        {
            Jarvis.Instance.ResetIcons();
        }
        transform.Find("Data").gameObject.SetActive(true);
        Jarvis.Instance.CurrentSelectedEvent = this;
    }
}