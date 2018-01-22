using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlaneMan = Singleton<PlaneManager>;
using InputMan = Singleton<InputController>;
//using Compass = Singleton<CompassController>;
using EventHandler = Singleton<EventManager>;
using MaterialMan = Singleton<MaterialManager>;
using Jarvis = Singleton<CoPilot>;
//using System;

public class PlaneController : MonoBehaviour
{
    // ============================= //
    public float RaycastLengthCenter; // 25
    public float RaycastLengthRight; // 10
    public float RaycastLengthLeft; // 10
    public float RaycastAngleLength;
    // ============================= //
    public GameObject currentPlaneThreat;
    public GameObject Warning;
    public LayerMask RaycastExclude;
    // ============================= //
    private bool hitCheck;
    private int count;
    public bool CastBool = false;
    public bool SevereWeatherHit = false;
    public bool VolcanicCheck = false;
    public bool VipCheck = false;
    public bool PlaneTrigger = false;
    private GameObject newGameEvent;
    private CrashEvent newCrashEvent;
    private GameObject VipCollision;
    // ============================= //

    #region PUBLIC VARIABLES
    public ScoreScript scoreManager;
    public float _speed;
    public float m_timeTillCollision;

    // Specific ID of each plane
    public string planeName;
    // Time the plane has been flying for
    public float flyingTime = 0.0f;
    // Shows if the plane is a priority
    public bool isPriority;
    // Modifies and returns the planes current height.
    public float CurrentHeight
    {   get { return PlaneToWorldHeight(m_currentHeight); }
        set { m_currentHeight = value; }
    }
    // Modifies and returns the planes current speed.
    public float Speed
    {   get { return m_speed; }
        set { m_speed = value; }
    }

    [Header("Height Values")]
    public float MinHeight; // 340 Rec
    public float MaxHeight; // 380 Rec

    [Header("Slider Values")]
    // Gets and Sets planes minimum speed
    public float minSpeed;
    //Gets and sets planes max speed
    public float maxSpeed;
    // Gets and sets planes maximum altitude
    public int MaxPlaneAltitude
    {   get { return m_maxPlaneAltitude; }
        set { m_maxPlaneAltitude = value; }
    }
    // Gets and sets planes minimum altitude
    public int MinPlaneAltitude
    {   get { return m_minPlaneAltitude; }
        set { m_minPlaneAltitude = value; }
    }

    [Header("Other Vars")]
    // Original Speed Value
    public float m_speedHolder;
    // List of all events that are going
    // to effect this plane
    public List<CoPilotEvent> CoPilotEvents;
    // Lock Rotation Bool
    public bool lockRotation = false;
    #endregion
     
    #region PRIVATE VARIABLES
    private float SpeedRange;
    private float HeightRange;
    private bool m_progressiveSpeed = false;
    private float m_speed;
    private float m_rotationSpeed = 10f;
    private float m_currentHeight = 220f;
    private int m_maxPlaneAltitude = 440;
    private int m_minPlaneAltitude = 220;
    private int m_maxWorldHeight = 25;
    private int m_minWorldHeight = 8;
    private Vector3 m_seekTowardPoint;
    private Vector3 m_EndTowardPoint;
    private LineRenderer m_lineRenderer;
    //private NodeController currentTraversingNode;
    #endregion

    #region References
    public GameObject WarningRadius;
    public PathController Path;
    public GameObject ghostObject;
    public GameObject _rotationGhost;
    private Collider m_currentlyTriggeredCollider;
    private GameObject m_selection;
    private MeshRenderer m_meshRenderer;
    //private ShaderController m_shaderController;
    private Rigidbody Rb;
    [HideInInspector]
    public MeshRenderer ghostMesh;
    #endregion

    // ============================= //
    private bool StartTimer1 = false;
    private bool StartTimer2 = false;
    private bool StartTimer3 = false;
    private float Timer;
    // ============================= //
    private struct UpdateData 
    {
        public PlaneController plane;
        public Quaternion direction;
    }

    // Adds CoPilot Events
    public void AddCoPilotEvent(CoPilotEvent _event)
    {
        CoPilotEvents.Add(_event);
    }
    // Material Set
    public void DefaultMaterial(params object[] args)
    {
        m_meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(planeName);
    }
    // Plane Deselection
    private void ReleaseSelection(params object[] args)
    {
        PlaneController controller = args[0] as PlaneController;
        if (controller != this) return;
        m_selection.SetActive(false);
    }
    // Plane Disablement - Collison
    private void DisablePlane()
    {
        if (this == PlaneMan.Instance.CurrentSelected)
        {   PlaneMan.Instance.ResetPlanes();
        }
        RemoveAllCoPilotEvents();
        EventHandler.Instance.RemoveListener(StringHandler.Events.InitializeSelection, InitializeSelection);
        EventHandler.Instance.RemoveListener(StringHandler.Events.ReleaseSelection, ReleaseSelection);
        PlaneMan.Instance.EnabledPlanes.Remove(this);
        transform.parent.gameObject.SetActive(false);
    }
    // World Heights
    public float WorldToPlaneHeight(float _worldHeight)
    {
        float newHeightPercentage = _worldHeight / m_maxWorldHeight;
        float newPlaneHeight = ((MaxPlaneAltitude - MinPlaneAltitude) * newHeightPercentage) + MinPlaneAltitude;
        return newPlaneHeight;
    }
    public float PlaneToWorldHeight(float _planeHeight)
    {
        float newHeightPercentage = (_planeHeight - m_minPlaneAltitude) / (MaxPlaneAltitude - MinPlaneAltitude);
        float newWorldHeight = ((m_maxWorldHeight - m_minWorldHeight) * newHeightPercentage) + m_minWorldHeight;
        return newWorldHeight;
    }
    
    // RB Update
    private void FixedUpdate()
    {
        // Manual Check
        Rb.MovePosition(transform.position + (transform.forward * Speed * Time.deltaTime));
    }

    // Intializer Functions
    public void InitSpeed()
    {
        Speed = PlaneMan.Instance.GetBaseSpeed(this);
    }
    public void InitHeight()
    {
        HeightRange = UnityEngine.Random.Range(MinHeight, MaxHeight);
        CurrentHeight = HeightRange;
    }
    public void Initialize(PathController _pathToAssign)
    {
        LocateReferences();
        InitializeVariables();
        Path = _pathToAssign;
        InitHeight();
        transform.position = new Vector3(Path._nodes[0].transform.position.x, CurrentHeight, Path._nodes[0].transform.position.z);
        transform.forward = (new Vector3(Path._nodes[1].transform.position.x, CurrentHeight, Path._nodes[1].transform.position.z) - transform.position).normalized;
    }
    public float InitializeCollision(float _timeTillCollision)
    {
        string name = planeName + "Outline";
        m_meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(name);
        // TODO: Implement a better way of assessing severity.
        if (_timeTillCollision < m_timeTillCollision)
        {
            m_timeTillCollision = _timeTillCollision;
            return 1.0f;
        }
        return 0.0f;
    }
    public void InitializeVariables()
    {
        planeName = transform.parent.name;
        InitSpeed();
        CoPilotEvents = new List<CoPilotEvent>();
        m_speedHolder = Speed;
        m_timeTillCollision = 100f;
    }
    // =============================================== //
    // Finding & Setting Variables
    private void LocateReferences()
    {
        Rb = GetComponent<Rigidbody>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_lineRenderer = GetComponent<LineRenderer>();
        m_selection = transform.Find("Selection").gameObject;
        m_selection.SetActive(false);
        ghostMesh = ghostObject.GetComponent<MeshRenderer>();
        ghostMesh.enabled = false;
    }
    // =============================================== //
    // Collisions & Triggers
    private void OnCollisionEnter(Collision collision)
    {
        // Exit Gate Collision
        if (collision.gameObject.CompareTag("ExitGate"))
        {   PlaneMan.Instance.AudioPlay(PlaneMan.Instance.BeaconExit);
            PlaneMan.Instance.m_PlanesMadeItCount += 1;
            DisablePlane();
        }
        // Out Of Bounds Check
        if (collision.transform.tag == "OutOfBounds")
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.Explosion);
            PlaneMan.Instance.m_Score -= 1;
            PlaneMan.Instance.m_CrashedAircraft += 1;
            DisablePlane();
            Instantiate(PlaneMan.Instance.OnDeathParticles, this.transform.position, this.transform.rotation);
        }
        // Plane Collison
        if (collision.transform.tag == "Plane")
        {   PlaneMan.Instance.AudioPlay(PlaneMan.Instance.Explosion);
            if (currentPlaneThreat != null)
            { CrashEvent(this, currentPlaneThreat, hitCheck); }
            DisablePlane();
            PlaneMan.Instance.m_CrashedAircraft += 1;
            Instantiate(PlaneMan.Instance.OnDeathParticles, this.transform.position, this.transform.rotation);
        }
        if (collision.transform.tag == "VIP")
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.Explosion);
            CrashEvent(this, VipCollision, CastBool);
            DisablePlane();
            PlaneMan.Instance.m_CrashedAircraft += 1;
            Instantiate(PlaneMan.Instance.OnDeathParticles, this.transform.position, this.transform.rotation);
        }
        if (collision.transform.tag == "Terrain")
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.Explosion);
            CrashEvent(this, VipCollision, CastBool);
            DisablePlane();
            PlaneMan.Instance.m_CrashedAircraft += 1;
            Instantiate(PlaneMan.Instance.OnDeathParticles, this.transform.position, this.transform.rotation);
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        // Node Collision
        if (collider.gameObject.name.IndexOf("Node") != -1 && PlaneMan.Instance.sceneName != "VolcanicTutorial") //&& Compass.Instance.PlaneIsMoving == false)
        {
            if (collider.transform.parent.name == Path.name)
            {
                if (!lockRotation)
                {   m_currentlyTriggeredCollider = collider;
                    m_seekTowardPoint = collider.gameObject.transform.position; //Path.endPosition.transform.position;
                    //currentTraversingNode = collider.gameObject.GetComponent<NodeController>();
                    var lookPos = (m_seekTowardPoint - transform.position).normalized;
                    lookPos.y = 0;
                    StartCoroutine(StringHandler.CoRoutines.LerpHeading, Quaternion.LookRotation(lookPos));
                }
            }
        }
        // Weather Collision
        if (collider.gameObject.CompareTag("MinorWeather"))
        {
            Speed = 4;
            PlaneMan.Instance._sliderController.speedSlider.value = Speed;
        }
        if (collider.gameObject.CompareTag("MajorWeather"))
        {
            Speed = 2;
            PlaneMan.Instance._sliderController.speedSlider.value = Speed;
        }
        if (collider.gameObject.CompareTag("DeadlyWeather") && isPriority == false)
        {
            if (newCrashEvent != null)
            {
                Jarvis.Instance.Events.Remove(newCrashEvent);
                Jarvis.Instance.alertNumber.text = Jarvis.Instance.Events.Count.ToString();
                Jarvis.Instance.EventGameObjects.Remove(newGameEvent);
                Destroy(newGameEvent);
            }
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.Explosion);
            DisablePlane();
            PlaneMan.Instance.m_CrashedAircraft += 1;
            Instantiate(PlaneMan.Instance.OnDeathParticles, this.transform.position, this.transform.rotation);
        }
        // Beacon Collision
        if (collider.transform.tag == "Beacon1")
        {
            StartTimer1 = true;
            Timer = 3;
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.BeaconEntry);
            PlaneMan.Instance.Beacon1.enabled = true;
            PlaneMan.Instance.Beacon1.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (collider.transform.tag == "Beacon2")
        {
            StartTimer2 = true;
            Timer = 3;
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.BeaconEntry);
            PlaneMan.Instance.Beacon2.enabled = true;
            PlaneMan.Instance.Beacon2.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (collider.transform.tag == "Beacon3")
        {
            StartTimer3 = true;
            Timer = 3;
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.BeaconEntry);
            PlaneMan.Instance.Beacon3.enabled = true;
            PlaneMan.Instance.Beacon3.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (collider.transform.tag == "Volcano" && isPriority == false)
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.Explosion);
            CrashEvent(this, PlaneMan.Instance.Volcano, CastBool);
            DisablePlane();
            Instantiate(PlaneMan.Instance.OnDeathParticles, this.transform.position, this.transform.rotation);
            PlaneMan.Instance.m_CrashedAircraft += 1;
        }
        // Tutorial Items
        if (collider.transform.tag == "HeightGate")
        {   collider.gameObject.SetActive(false);
            PlaneMan.Instance.TutorialHit += 1;
        }
        if (collider.transform.tag == "DirectionGate")
        {   collider.gameObject.SetActive(false);
            PlaneMan.Instance.TutorialHit += 1;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        //if (collider == m_currentlyTriggeredCollider)
        //{ currentTraversingNode = null; }
        // Checking Weather Tag
        if (collider.gameObject.CompareTag("Weather"))
        {
            if (Speed != m_speedHolder)
            { m_progressiveSpeed = true;
            }
        }
    }
    // =============================================== //
    // Removes CoPilot Event 
    public void RemoveCoPilotEvent(CoPilotEvent _event)
    {
        if (CoPilotEvents.Contains(_event))
        { CoPilotEvents.Remove(_event); }
    }
    // Creates A Line Render
    private void RenderLine()
    {
        m_lineRenderer.SetVertexCount(100);
        Vector3[] vertPositions = new Vector3[100];
        float segmentLength = transform.position.y / 100;
        for (int i = 0;i < 100;i++)
        { vertPositions[i] = new Vector3(this.transform.position.x, segmentLength * i, this.transform.position.z); }
        m_lineRenderer.SetPositions(vertPositions);
    }

    // Begins A Progressive Speed Increase Out Of A Hazard
    private void RegulateSpeed()
    {
        if (m_progressiveSpeed == true)
        {
            if (Speed < m_speedHolder)
            { Speed += 0.5f * Time.deltaTime;
            }
            else if (Speed >= m_speedHolder)
            { m_progressiveSpeed = false;
                Speed = m_speedHolder;
            }
        }
    }
    // Removes All CoPilot Events
    private void RemoveAllCoPilotEvents()
    {
        for (int i = 0; i < CoPilotEvents.Count; ++i)
        {
            CrashEvent crashEvent = CoPilotEvents[i] as CrashEvent;
            if (crashEvent != null)
                Jarvis.Instance.ResetCrashEvents(crashEvent);
        }
    }
    // Selection Check
    private void InitializeSelection(params object[] args)
    {
        PlaneController controller = args[0] as PlaneController;
        if (controller != this) return;
        m_selection.SetActive(true);
    }
    // Manual Check
    public void LockRotation(bool _state)
    { lockRotation = _state;
    }
    // Start Function
    private void Start()
    {
        if (isPriority == false)
        {
            EventHandler.Instance.AddListener(StringHandler.Events.InitializeSelection, InitializeSelection);
            EventHandler.Instance.AddListener(StringHandler.Events.ReleaseSelection, ReleaseSelection);
        }
        EventHandler.Instance.AddListener(StringHandler.Events.UpdateMaterial, UpdateMaterial);
    }
    // Updates
    private void Update()
    {
        if (isPriority == false)
        { PlaneRay(CastBool);}
        TimerWork();
        flyingTime += Time.deltaTime;
        RegulateSpeed();
        RenderLine();
    }
    private void UpdateMaterial(params object[] args)
    {
        PlaneController current = args[0] as PlaneController;
        if (current == this)
        { return;
        }
    }
    public void UpdatePlane()
    {
        if (isPriority == false)
        {
            InputMan.Instance.ListenForInput(this);
            TransformExtensions.SetXPos(ghostObject.transform, transform.position.x);
            TransformExtensions.SetZPos(ghostObject.transform, transform.position.z);
        }
    }

    IEnumerator LerpHeading(Quaternion newHeading)
    {
        while ((Vector3.Distance(transform.eulerAngles, newHeading.eulerAngles)) > 0.2)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, newHeading, Time.deltaTime * m_rotationSpeed);
            yield return null;
        }
        transform.rotation = newHeading;
    }

    IEnumerator NewHeading(UpdateData newData)
    {
        Quaternion newOffCourse = newData.direction;
        PlaneController planeDATA = newData.plane;
        while ((Vector3.Distance(planeDATA.transform.eulerAngles, newOffCourse.eulerAngles)) > 0.2)
        {
            planeDATA.transform.rotation = Quaternion.Slerp(planeDATA.transform.rotation, newOffCourse, Time.deltaTime * m_rotationSpeed);
            yield return null;
        }
        planeDATA.transform.rotation = newOffCourse;
    }
    // =============================================== //
    private void PlaneRay(bool Hit)
    {
        hitCheck = Hit;
        RaycastHit objectHit;
        Vector3 forwardPos = this.transform.TransformDirection(Vector3.forward);
        Vector3 rightPos = this.transform.TransformDirection(Vector3.right);
        Vector3 leftPos = this.transform.TransformDirection(Vector3.left);
        Vector3 angleOne = this.transform.TransformDirection(Vector3.forward + Vector3.right / 2).normalized;
        Vector3 angleTwo = this.transform.TransformDirection(Vector3.forward - Vector3.right / 2).normalized;

        Debug.DrawRay(this.transform.position, forwardPos * RaycastLengthCenter, Color.red);
        Debug.DrawRay(this.transform.position, rightPos * RaycastLengthRight, Color.red);
        Debug.DrawRay(this.transform.position, leftPos * RaycastLengthLeft, Color.red);
        Debug.DrawRay(this.transform.position, angleOne * RaycastAngleLength, Color.red);
        Debug.DrawRay(this.transform.position, angleTwo * RaycastAngleLength, Color.red);

        if (Physics.Raycast(this.transform.position, forwardPos, out objectHit, RaycastLengthCenter, ~RaycastExclude , QueryTriggerInteraction.Collide) || 
            Physics.Raycast(this.transform.position, rightPos, out objectHit, RaycastLengthRight, ~RaycastExclude, QueryTriggerInteraction.Collide) ||
            Physics.Raycast(this.transform.position, leftPos, out objectHit, RaycastLengthLeft, ~RaycastExclude, QueryTriggerInteraction.Collide) || 
            Physics.Raycast(this.transform.position, angleOne, out objectHit, RaycastAngleLength, ~RaycastExclude, QueryTriggerInteraction.Collide) ||
            Physics.Raycast(this.transform.position, angleTwo, out objectHit, RaycastAngleLength, ~RaycastExclude, QueryTriggerInteraction.Collide))
        {
            if (objectHit.transform.tag == "Weather" && Hit == false)
            { Hit = true;
            }
            if (objectHit.transform.tag == "MajorWeather" && Hit == false)
            { Hit = true;
            }
            if (objectHit.transform.tag == "DeadlyWeather" && Hit == false)
            { Hit = true;
              SevereWeatherHit = true;
            }
            if (objectHit.transform.tag == "Volcano")
            {
                Hit = true;
                VolcanicCheck = true;
            }
            if (objectHit.transform.tag == "VIP")
            {   if (objectHit.transform.GetComponent<VIPCon>().Manager != this)
                {   Hit = true;
                    VipCheck = true;
                    VipCollision = objectHit.transform.gameObject;
                }
            }
            if (objectHit.transform.tag == "Plane")
            {
                Hit = true;
                PlaneTrigger = true;
            }
        }
        if (Hit && count == 0 && VolcanicCheck == false && VipCheck == false && PlaneMan.Instance.sceneName == "VolcanicIslandsHard")
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.AlertNotify);
            if (SevereWeatherHit == false) { newGameEvent = Instantiate(Jarvis.Instance.weatherEventPrefab) as GameObject; }
            else if (SevereWeatherHit == true) { newGameEvent = Instantiate(Jarvis.Instance.severeWeatherPrefab) as GameObject; }
            CrashEvent(this, PlaneMan.Instance.MapThreats.MapCloud, Hit);
            count += 1;
            SevereWeatherHit = false;
        }
        else if (!Hit && VolcanicCheck == false && VipCheck == false && PlaneMan.Instance.sceneName == "VolcanicIslandsHard")
        {
            CrashEvent(this, PlaneMan.Instance.MapThreats.MapCloud, Hit);
            count = 0;
        }
        /*else*/ if (Hit && count == 0 && VolcanicCheck == true)
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.AlertNotify);
            newGameEvent = Instantiate(Jarvis.Instance.volcanicEventPrefab) as GameObject;
            CrashEvent(this, PlaneMan.Instance.Volcano, Hit);
            count += 1;
        }
        else if (!Hit && VolcanicCheck == true)
        {
            CrashEvent(this, PlaneMan.Instance.Volcano, Hit);
            count = 0;
            VolcanicCheck = false;
        }
        else if (Hit && count == 0 && VipCheck == true)
        {
            if (VipCollision.GetComponent<VIPCon>().isHeli == false)
            {   PlaneMan.Instance.AudioPlay(PlaneMan.Instance.AlertNotify);
                newGameEvent = Instantiate(Jarvis.Instance.vipEventPrefab) as GameObject;
            }
            else if (VipCollision.GetComponent<VIPCon>().isHeli == true)
            {   PlaneMan.Instance.AudioPlay(PlaneMan.Instance.AlertNotify);
                newGameEvent = Instantiate(Jarvis.Instance.heliEventPrefab) as GameObject;
            }
            CrashEvent(this, VipCollision , Hit);
            count += 1;
        }
        else if (!Hit && VipCheck == true)
        {
            CrashEvent(this, VipCollision, Hit);
            count = 0;
            VipCheck = false;
        }
        // ============================================ //
        if (Hit && count == 0 && PlaneTrigger == true)
        {
            currentPlaneThreat = objectHit.transform.gameObject;
            string name = planeName + "Outline";
            string nameTwo = currentPlaneThreat.GetComponent<PlaneController>().planeName + "Outline";
            m_meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(name);
            currentPlaneThreat.GetComponent<PlaneController>().m_meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(nameTwo);
            Warning.gameObject.SetActive(true);
            currentPlaneThreat.GetComponent<PlaneController>().Warning.gameObject.SetActive(true);

            if (CoPilotEvents.Count != 2 && currentPlaneThreat.GetComponent<PlaneController>().CoPilotEvents.Count != 2)
            {
                PlaneMan.Instance.AudioPlay(PlaneMan.Instance.AlertNotify);
                newGameEvent = Instantiate(Jarvis.Instance.crashEventPrefab) as GameObject;
                CrashEvent(this, currentPlaneThreat, Hit);
            }
            count += 1;
        }
        else if (!Hit && currentPlaneThreat != null)
        {
            Warning.gameObject.SetActive(false);
            currentPlaneThreat.GetComponent<PlaneController>().Warning.gameObject.SetActive(false);
            CrashEvent(this, currentPlaneThreat, Hit);
            count = 0;
            this.DefaultMaterial();
            currentPlaneThreat.GetComponent<PlaneController>().DefaultMaterial();
            PlaneTrigger = false;
        }
        // ============================================ //
    }

    private void TimerWork()
    {
        if (StartTimer1 == true)
        {
            if (Timer > 0)
            { Timer -= 1 * Time.deltaTime;
            }
            else
            { Timer = 0;
              PlaneMan.Instance.Beacon1.enabled = false;
              PlaneMan.Instance.Beacon1.GetComponent<SpriteRenderer>().enabled = false;
                StartTimer1 = false;
            }
        }
        else if (StartTimer2 == true)
        {
            if (Timer > 0)
            { Timer -= 1 * Time.deltaTime;
            }
            else
            { Timer = 0;
              PlaneMan.Instance.Beacon2.enabled = false;
              PlaneMan.Instance.Beacon2.GetComponent<SpriteRenderer>().enabled = false;
                StartTimer2 = false;
            }
        }
        else if (StartTimer3 == true)
        {
            if (Timer > 0)
            { Timer -= 1 * Time.deltaTime;
            }
            else
            { Timer = 0;
              PlaneMan.Instance.Beacon3.enabled = false;
              PlaneMan.Instance.Beacon3.GetComponent<SpriteRenderer>().enabled = false;
                StartTimer3 = false;
            }
        }
    }

    public void CrashEvent(PlaneController _a, GameObject _b, bool hit)
    {
        if (hit)
        {
            newCrashEvent = newGameEvent.GetComponent<CrashEvent>();
            newCrashEvent.InitializeNewThreat(_a, _b);
            newGameEvent.transform.SetParent(Jarvis.Instance.EventTransform);
            Jarvis.Instance.Events.Add(newCrashEvent);
            Jarvis.Instance.EventGameObjects.Add(newGameEvent);

            newGameEvent.transform.position = Jarvis.Instance.alertButton.transform.position;
            float newY = newGameEvent.transform.position.y - (Jarvis.Instance.EventGameObjects.Count * Jarvis.Instance.m_notificationOffset);
            TransformExtensions.SetYPos(newGameEvent.transform, newY);
            newGameEvent.transform.localScale = new Vector3(1f, 1f, 1f);

            Jarvis.Instance.alertNotification.SetActive(true);
            Jarvis.Instance.alertNumber.text = Jarvis.Instance.Events.Count.ToString();
        }
        else if (!hit || this.enabled == false)
        {
            if (newCrashEvent != null)
            {
                Jarvis.Instance.Events.Remove(newCrashEvent);
                Jarvis.Instance.alertNumber.text = Jarvis.Instance.Events.Count.ToString();
                Jarvis.Instance.EventGameObjects.Remove(newGameEvent);
                Jarvis.Instance.ReAssessEvents(_a);
                Destroy(newGameEvent);
            }
        }
    }

    public void ResetLook()
    {
        m_EndTowardPoint = Path.endPosition.transform.position;
        var lookPos = (m_EndTowardPoint - transform.position).normalized;
        lookPos.y = 0;

        UpdateData newData;
        newData.direction = Quaternion.LookRotation(lookPos);
        newData.plane = PlaneMan.Instance.CurrentSelected;

        //StartCoroutine(StringHandler.CoRoutines.LerpHeading, Quaternion.LookRotation(lookPos));
        StartCoroutine(StringHandler.CoRoutines.NewHeading, newData);
    }
}