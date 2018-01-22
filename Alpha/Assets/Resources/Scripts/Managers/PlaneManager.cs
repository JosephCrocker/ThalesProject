using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EventHandler = Singleton<EventManager>;
using Compass = Singleton<CompassController>;
using Jarvis = Singleton<CoPilot>;
using Slider = Singleton<SliderController>;
using Cam = Singleton<CameraController>;

public class PlaneManager : MonoBehaviour
{
    #region Objects
    public PlaneController CurrentSelected
    {   get { return m_currentSelected; }
        set { if (m_currentSelected)
            { ResetPlanes();}
            EventHandler.Instance.DoEvent(StringHandler.Events.InitializeSelection, value);
        }
    }

    public int PlanesSpawned
    {   get { return m_planesSpawned; }
        private set { m_planesSpawned = value; }
    }

    // Reference to the Game UI Transform and its children.
    public Transform _gameUITab;
    // Transform that contains all of the sliders.
    public GameObject _planeUpdaterTab;

    /// <summary>
    /// List of all Instatiated Planes.
    /// </summary>
    private List<PlaneController> PlaneControllers;

    /// <summary>
    /// List of all Enabled Planes.
    /// </summary>
    [HideInInspector]
    public List<PlaneController> EnabledPlanes;

    public GameObject[] PlanePrefabs;
    
    /// <summary>
    /// A Plane with box collider which is used
    /// for detecting touch input at the same height
    /// as the currently selected plane.
    /// </summary>
    public GameObject RaycastPlane;

    /// <summary>
    /// If true, user can't select other planes.
    /// </summary>
    public bool m_selectionLock = false;

    // The max number of planes to spawn.
    public int m_maxPlanes;

    /// <summary>
    /// The number of planes that have been spawned.
    /// </summary>
    private int m_planesSpawned;

    /// <summary>
    /// Time between Spawns
    /// </summary>
    public float m_timerSet;
    #endregion

    [HideInInspector]
    public float TutorialHit;
    public bool PauseIsActive;
    // =========================== //
    public ThreatSpawner MapThreats;
    private Scene currentScene;
    public string sceneName;
    private int TutorialSpawn;
    public GameObject ResetObject;
    public GameObject PathHolder;
    public LayerMask planeLayer;
    [Header("Prefabs")]
    public GameObject Volcano;
    public GameObject OnDeathParticles;
    private float countUp;
    // =========================== //
    #region Beacons
    [Header("Beacon Animations / Audio")]
    public Animator Beacon1;
    public Animator Beacon2;
    public Animator Beacon3;
    public AudioClip BeaconEntry;
    public AudioClip BeaconExit;
    public AudioClip AlertNotify;
    public AudioClip Explosion;
    private AudioSource source;
    #endregion

    #region Data & Score
    [Header("Plane Data")]
    public float commercialBaseSpeed;
    public float lightAircraftBaseSpeed;
    public float priorityBaseSpeed;
    [Header("Scores")]
    public float m_Score;
    public float m_CrashedAircraft;
    public float m_PlanesMadeItCount;
    // =========================== //
    private float m_timer;
    private Dictionary<string, float> planeSpeeds = new Dictionary<string, float>();
    // =========================== //
    // Controllers
    private PlaneController m_currentSelected;
    public PathController[] Paths;
    [HideInInspector]
    public SliderController _sliderController;
    // =========================== //
    #endregion

    void Start()
    {
        TutorialHit         = 0;
        m_PlanesMadeItCount = 0;
        currentScene        = SceneManager.GetActiveScene();
        sceneName           = currentScene.name;
        source              = GetComponent<AudioSource>();
        PlaneControllers    = new List<PlaneController>();
        EnabledPlanes       = new List<PlaneController>();
        m_Score             = 0;
        m_planesSpawned     = 0;
        m_timer             = m_timerSet;
        // Speeds
        planeSpeeds[StringHandler.Planes.Commercial]    = commercialBaseSpeed;
        planeSpeeds[StringHandler.Planes.LightAircraft] = lightAircraftBaseSpeed;
        planeSpeeds[StringHandler.Planes.Priority]      = priorityBaseSpeed;
        // Events
        EventHandler.Instance.AddListener(StringHandler.Events.InitializeSelection, InitializeSelection);
        EventHandler.Instance.AddListener(StringHandler.Events.ReleaseSelection, ReleaseSelection);
        CachePathIntersections();
        SpawnPlanes();
    }

    void Update()
    {
        if (CurrentSelected && CurrentSelected.isPriority == false)
        {
            Cam.Instance.ToggleRotationLock(true);
            if (Compass.Instance.CompassArrow.activeSelf == false)
            { Compass.Instance.CompassArrow.gameObject.SetActive(true); }
            if (ResetObject.activeSelf == false)
            { ResetObject.gameObject.SetActive(true); }
            CurrentSelected._rotationGhost.transform.position = CurrentSelected.transform.position;
            //CurrentSelected.Path.endPosition.GetChild(0).GetComponent<Animator>().enabled = true;
            CurrentSelected.Path.GetComponent<MeshRenderer>().enabled = true;
            CurrentSelected.UpdatePlane();
        }
        else
        {
            if (PauseIsActive == false) { Cam.Instance.ToggleRotationLock(false); }
            if (ResetObject.activeSelf == true)
            { ResetObject.gameObject.SetActive(false); }
            Compass.Instance.StopCameraMovement();
            Compass.Instance.CamIsMoving = false;
            Slider.Instance.ResetSliders();
            if (Compass.Instance.CompassArrow.activeSelf == true)
            { Compass.Instance.CompassArrow.gameObject.SetActive(false); }
        }
        CheckSelection();
        EnablePlanes();
    }

    // Mouse / Touch Selection Check
    void CheckSelection()
    {
        #region MOUSE/TOUCH
                if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    RaycastHit hitInfo;
                    if (!m_selectionLock)
                    {
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, float.PositiveInfinity, planeLayer, QueryTriggerInteraction.Collide))
                        {
                            if (hitInfo.collider.transform.CompareTag("Plane"))
                            {
                                CurrentSelected = hitInfo.transform.gameObject.GetComponent<PlaneController>();
                                TransformExtensions.SetYPos(CurrentSelected.ghostObject.transform, CurrentSelected.transform.position.y);
                            }
                            else
                            {
                                if (CurrentSelected)
                                {
                                    ResetPlanes();
                                }
                            }
                        }
                        else
                        {
                           if (CurrentSelected)
                           {
                               ResetPlanes();
                           }
                        }
                    }
                }
        #endregion
    }

    // Plane Management
    void SpawnPlanes()
    {
        for (int i = 0; i < m_maxPlanes; ++i)
        {
            countUp += 1;
            if (countUp == 4)
            { countUp = 1;
            }
            if (Paths.Length > 0)
            {
                GameObject newPlane;
                if (i != 0)
                {
                    //if (i % 10 == 0)
                    //{ newPlane = Instantiate(PlanePrefabs[2]) as GameObject;
                    //}
                    /*else*/ if (i % 2 == 0)
                    { newPlane = Instantiate(PlanePrefabs[1]) as GameObject;
                    }
                    else
                    { newPlane = Instantiate(PlanePrefabs[0]) as GameObject;
                    }
                }
                else
                { newPlane = Instantiate(PlanePrefabs[0]) as GameObject;
                }
                newPlane.name = newPlane.name.Substring(0, newPlane.name.Length - 7);
                newPlane.transform.parent = GameObject.Find("Planes").transform;
                PlaneController planeController = newPlane.transform.Find("Plane3D").GetComponent<PlaneController>();
                
                // Base Game
                if (sceneName != "VolcanicTutorial")
                {
                    if (countUp == 1)
                    { planeController.Initialize(Paths[0]);
                    }
                    if (countUp == 2)
                    { planeController.Initialize(Paths[1]);
                    }
                    if (countUp == 3)
                    { planeController.Initialize(Paths[2]);
                    }
                }
                // Tutorial
                else if (TutorialSpawn == 0)
                { planeController.Initialize(Paths[0]);
                  TutorialSpawn += 1;
                }
                else if (TutorialSpawn == 1)
                { planeController.Initialize(Paths[1]);
                }
                PlaneControllers.Add(planeController);
                newPlane.SetActive(false);
                planeController.scoreManager.InitTime(planeController);
            }
        }
    }
    void EnablePlanes()
    {
        if (m_planesSpawned < m_maxPlanes)
        { m_timer -= 1 * Time.deltaTime;
            if (m_timer <= 0f)
            {
                PlaneControllers[m_planesSpawned].transform.parent.gameObject.SetActive(true);
                if (EnabledPlanes.Count > 0)
                {
                    for (int j = 0; j < EnabledPlanes.Count; ++j)
                    { Jarvis.Instance.InitialCollisionCheck(PlaneControllers[m_planesSpawned], EnabledPlanes[j]);
                    }
                }
                EnabledPlanes.Add(PlaneControllers[m_planesSpawned]);
                m_timer = m_timerSet;
                m_planesSpawned++;
            }
        }
    }

    void TogglePaths(bool _state)
    {
        for (int i = 0; i < Paths.Length; ++i)
        { Paths[i].enabled = _state; }
    }

    private void InitializeSelection(params object[] args)
    {
        PlaneController planeController = args[0] as PlaneController;
        if (planeController)
        { m_currentSelected = planeController; }
    }

    private void ReleaseSelection(params object[] args)
    {
        PlaneController planeController = args[0] as PlaneController;
        if (planeController)
        { m_currentSelected = null;}
    }

    public void ResetPlanes()
    {
        if (CurrentSelected != null)
        {
            //CurrentSelected.Path.endPosition.GetChild(0).GetComponent<Animator>().enabled = false;
            CurrentSelected.Path.GetComponent<MeshRenderer>().enabled = false;
            EventHandler.Instance.DoEvent(StringHandler.Events.ReleaseSelection, CurrentSelected);
        }
    }

    public void ToggleSelectionLock(bool _state)
    { m_selectionLock = _state;}

    private void CachePathIntersections()
    {
        for (int i = 0; i < Paths.Length; ++i)
        {
            for (int j = 0; j < Paths.Length; ++j)
            {
                if (i != j && j > i)
                { CachePathIntersections(Paths[i], Paths[j]);
                }
            }
        }
    }

    private void CachePathIntersections(PathController _pathA, PathController _pathB)
    {
        int aCount = _pathA._nodes.Count;
        int bCount = _pathB._nodes.Count;
        for (int i = 0; i < aCount; ++i)
        {
            for (int j = 0; j < bCount; ++j)
            {
                if(_pathA.IntersectingPaths == null)
                {   _pathA.IntersectingPaths = new Dictionary<PathController, List<NodeController>>();
                }
                if (_pathB.IntersectingPaths == null)
                {   _pathB.IntersectingPaths = new Dictionary<PathController, List<NodeController>>();
                }
                if (_pathA._nodes[i].transform.position != _pathB._nodes[j].transform.position)
                {   continue;
                }
                else
                {
                    if (!_pathA.IntersectingPaths.ContainsKey(_pathB))
                    { _pathA.IntersectingPaths[_pathB] = new List<NodeController>();
                    }
                    _pathA.IntersectingPaths[_pathB].Add(_pathA._nodes[i]);                
                    if (!_pathB.IntersectingPaths.ContainsKey(_pathA))
                    { _pathB.IntersectingPaths[_pathA] = new List<NodeController>();
                    }
                    _pathB.IntersectingPaths[_pathA].Add(_pathB._nodes[j]);
                }
            }
        }
    }

    public float GetBaseSpeed(PlaneController plane)
    {
        if (planeSpeeds.ContainsKey(plane.planeName))
        {   return planeSpeeds[plane.planeName];
        }
        return 1.0f;
    }

    public void AudioPlay(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void ResetLook()
    {
        Compass.Instance.StopPlaneRotate();
        CurrentSelected.ResetLook();
    }
}