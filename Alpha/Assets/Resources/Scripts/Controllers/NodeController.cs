using UnityEngine;
//using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class NodeController : MonoBehaviour
{
    public PathController controller;

    #region Values
    [SerializeField]
    [HideInInspector]
    private float distance;

    [SerializeField]
    [HideInInspector]
    BoxCollider _boxCollider;

    [SerializeField]
    [HideInInspector]
    NodeController previousController;

    [SerializeField]
    [HideInInspector]
    public Node _saveData = new Node();
    public bool meshCreated = false;

    /// <summary>
    /// The speed the plane will change to 
    /// at this node.
    /// </summary>
    public float speedModifier;

    /// <summary>
    /// The time a commercial jet on
    /// this route will take to reach 
    /// this node.
    /// </summary>
    [SerializeField]
    public float commercialArrivalTime;

    /// <summary>
    /// The time it will take a Light Aircraft 
    /// on this route to reach this node.
    /// </summary>
    [SerializeField]
    public float lightAircraftArrivalTime;

    /// <summary>
    /// The time it will take a Priority Aircraft 
    /// on this route to reach this node.
    /// </summary>
    [SerializeField]
    public float priorityArrivalTime;

    /// <summary>
    /// Dictionary storing arrival times of planes
    /// </summary>
    public Dictionary<string, float> ArrivalTimes;
    #endregion

    void Start()
    {
        if (Application.isPlaying)
        {
            controller = this.transform.parent.gameObject.GetComponent<PathController>();

            RaycastHit objectHit;
            Vector3 DownPos = this.transform.TransformDirection(Vector3.down);
            Debug.DrawRay(this.transform.position, DownPos * 18, Color.red);

            if (Physics.Raycast(transform.position, DownPos, out objectHit, 18))
            {
                if (objectHit.transform.tag == "Terrain")
                {
                    Instantiate(controller.PathTexture, objectHit.point, transform.rotation);
                }
            }
            CreateBox();
            ArrivalTimes = new Dictionary<string, float>();
            ArrivalTimes[StringHandler.Planes.Commercial] = commercialArrivalTime;
            ArrivalTimes[StringHandler.Planes.LightAircraft] = lightAircraftArrivalTime;
            ArrivalTimes[StringHandler.Planes.Priority] = priorityArrivalTime;
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateLookAt();
            //transform.position += transform.parent.position;
        }
    }

    public void Init(NodeController _previous)
    {
        _boxCollider = GetComponent<BoxCollider>();
        previousController = _previous;
        distance = Vector3.Distance(transform.position, previousController.transform.position);
        _boxCollider.size = new Vector3(1, 40, distance);
        _boxCollider.center = new Vector3(0, 0, distance / 2);
    }

    private void UpdateLookAt()
    {
        if (previousController)
        {
            transform.LookAt(previousController.transform.position);
            distance = Vector3.Distance(transform.position, previousController.transform.position);
            _boxCollider.size = new Vector3(1, 40, distance);
            _boxCollider.center = new Vector3(0, 0, (distance / 2));
        }
    }

    void CreateBox()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        float length = 0.1f;
        float width = 0.1f;
        float height = distance;
        
        Vector3 offset = Vector3.forward * (distance / 2);

        #region Vertices
        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f) + offset;
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f) + offset;
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f) + offset;
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f) + offset;

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f) + offset;
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f) + offset;
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f) + offset;
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f) + offset;

        Vector3[] vertices = new Vector3[] {
	        // Bottom
	        p0, p1, p2, p3,
 
	        // Left
	        p7, p4, p0, p3,
 
	        // Front
	        p4, p5, p1, p0,
 
	        // Back
	        p6, p7, p3, p2,
 
	        // Right
	        p5, p6, p2, p1,
 
	        // Top
	        p7, p6, p5, p4
         };
        #endregion

        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normales = new Vector3[]
        {
	// Bottom
	down, down, down, down,
 
	// Left
	left, left, left, left,
 
	// Front
	front, front, front, front,
 
	// Back
	back, back, back, back,
 
	// Right
	right, right, right, right,
 
	// Top
	up, up, up, up
        };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
	// Bottom
	_11, _01, _00, _10,
 
	// Left
	_11, _01, _00, _10,
 
	// Front
	_11, _01, _00, _10,
 
	// Back
	_11, _01, _00, _10,
 
	// Right
	_11, _01, _00, _10,
 
	// Top
	_11, _01, _00, _10,
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	// Bottom
	3, 1, 0,
    3, 2, 1,			
 
	// Left
	3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	// Front
	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	// Back
	3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
    3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	// Right
	3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	// Top
	3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
    3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

        };
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        ;

        meshCreated = true;
    }

    public float GetArrivalTime(string _tag)
    {
        if (ArrivalTimes.ContainsKey(_tag))
        {
            return ArrivalTimes[_tag];
        }

        return 1.0f;
    }
}