using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using PlaneMan = Singleton<PlaneManager>;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PathController : MonoBehaviour
{
    public GameObject PathTexture;

    #region Values
    /// <summary>
    /// Start Nodes Position
    /// </summary>
    public Transform startPosition;

    /// <summary>
    /// End Nodes position
    /// </summary>
    public Transform endPosition;

    /// <summary>
    /// Distance between each node.
    /// </summary>
    public float _nodeDistanceApart;

    /// <summary>
    /// Current amount of nodes on this path.
    /// </summary>
    public int nodeIndex = 0;

    /// <summary>
    /// The nodes of this path.
    /// </summary>
    public List<NodeController> _nodes = new List<NodeController>();

    /// <summary>
    /// Returns a list of floats containing 
    /// times, in seconds, of when planes on
    /// this path will intersect with the path
    /// passed in as the key. 
    /// </summary>
    public Dictionary<PathController, List<NodeController>> IntersectingPaths;

    /// <summary>
    /// Total time a Commercial Aircraft will take without
    /// being modified to traverse the path
    /// </summary>
    [SerializeField]
    public float commercialAircraftTraversalTime;

    /// <summary>
    /// Total time it will take a Light Aircraft
    /// to traverse the path.
    /// </summary>
    [SerializeField]
    public float lightAircraftTraversalTime;
    
    [SerializeField]
    public float priorityAircraftTraversalTime;

    /// <summary>
    /// An instance of a Path class used
    /// for serializing this class to XML.
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private Path _path = new Path();
    
    private bool createMesh = false;
    private bool init = false;
    #endregion

    void Update()
    {
        if (Application.isPlaying)
        {
            if (!init)
            { CreateMesh();
            }
        }
        else
        {
        #if UNITY_EDITOR
            CheckEditorFunctions();
            _path.PathName = name;
        #endif
        }
    }

    void CreateMesh()
    {
        if (createMesh)
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];
            int i = 0;
            int j = 0;
            while (i < meshFilters.Length)
            {
                if (meshFilters[i].sharedMesh)
                {
                    combine[j].mesh = meshFilters[i].sharedMesh;
                    combine[j].transform = meshFilters[i].transform.localToWorldMatrix;
                    meshFilters[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
                    j++;
                }
                i++;
            }

            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            transform.gameObject.SetActive(true);
            init = true;
        }
        else
        {
            CheckNodeMeshInit();
        }
    }

    void CheckNodeMeshInit()
    {
        for (int i = 0; i < _nodes.Count; ++i)
        {
            if (!_nodes[i].meshCreated)
            {
                return;
            }
        }
        createMesh = true;
    }

#if UNITY_EDITOR
    public void AddNode()
    {
        if (!startPosition || !endPosition)
        {
            Debug.Log("Add Start/End Position");
            return;
        }
        Vector3 delta = (endPosition.position - _nodes[nodeIndex - 1].transform.position).normalized;
        GameObject node = Instantiate(Resources.Load("Prefabs/Node")) as GameObject;
        node.transform.position = _nodes[nodeIndex - 1].transform.position + (delta * _nodeDistanceApart);
        node.transform.parent = this.transform;
        node.name = "Node" + nodeIndex;
        node.GetComponent<NodeController>().Init(_nodes[nodeIndex - 1]);
        _nodes.Add(node.GetComponent<NodeController>());
        nodeIndex++;
    }

    public void RemoveNode()
    {
        if (_nodes.Count > 1)
        {
            nodeIndex--;
            _nodes.RemoveAt(nodeIndex);
            DestroyImmediate(transform.Find("Node" + nodeIndex).gameObject);
        }
    }

    void CheckEditorFunctions()
    {
        if (startPosition)
        {
            if (!transform.Find("StartNode"))
            {
                GameObject node = Instantiate(Resources.Load("Prefabs/Node")) as GameObject;
                node.transform.position = startPosition.position;
                node.transform.parent = this.transform;
                node.name = "StartNode";
                _nodes.Add(node.GetComponent<NodeController>());
                nodeIndex++;
            }
            _path.entryGate = startPosition.name;
        }
        if (endPosition)
        {
            _path.exitGate = endPosition.name;
        }
    }

    public void Save()
    {
        _path.Clear();
        for (int i = 0; i < _nodes.Count; ++i)
        {
            _path.AddNode(_nodes[i]);
        }
        _path.Save();
    }

    public void Load(string _filePath)
    {
        var serializer = new XmlSerializer(typeof(Path));
        using (var stream = new FileStream(_filePath, FileMode.Open))
        {
            _path = serializer.Deserialize(stream) as Path;
        }

        Clear();

        for (int i = 0; i < _path._nodes.Count; ++i)
        {
            AddNodeFromFile(_path._nodes[i]);
        }
        name = _path.PathName;
    }

    void Clear()
    {
        if (_nodes.Count > 0)
        {
            _nodes.Clear();
        }
        nodeIndex = 0;

        int count = transform.childCount - 1;

        for (int i = count; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void AddNodeFromFile(Node _savedNode)
    {
        startPosition = GameObject.Find("Scene").transform.Find("World").transform.Find("Spawns").transform.Find(_path.entryGate);
        endPosition = GameObject.Find("Scene").transform.Find("World").transform.Find("ExitGates").transform.Find(_path.exitGate);
        GameObject node = Instantiate(Resources.Load("Prefabs/Node")) as GameObject;
        node.transform.position = new Vector3(_savedNode.posX, _savedNode.posY, _savedNode.posZ);
        node.transform.parent = this.transform;
        node.name = _savedNode.Name;

        NodeController newNode = node.GetComponent<NodeController>();
        newNode.commercialArrivalTime = _savedNode.commercial;
        newNode.lightAircraftArrivalTime = _savedNode.light;
        newNode.priorityArrivalTime = _savedNode.priority;
        newNode.speedModifier = _savedNode.speedModifier;

        if (nodeIndex == 0)
        {
            newNode.Init(newNode);
        }
        else
        {
            newNode.Init(_nodes[nodeIndex - 1]);
        }
        _nodes.Add(newNode);
        nodeIndex++;
    }

    public void CalculateTraversalTime()
    {
        commercialAircraftTraversalTime = 0.0f;
        lightAircraftTraversalTime = 0.0f;
        priorityAircraftTraversalTime = 0.0f;

        int count = _nodes.Count;
        for (int i = 1; i < count; ++i)
        {
            float distance = Vector3.Distance(_nodes[i].transform.position, _nodes[i - 1].transform.position);

            if (_nodes[i - 1].speedModifier == 0)
            {
                _nodes[i - 1].speedModifier = 1.0f;
            }

            // Find traversal time for a commercial Jet.
            float timeBetweenNodes = distance / (PlaneMan.Instance.commercialBaseSpeed * _nodes[i - 1].speedModifier);
            commercialAircraftTraversalTime += timeBetweenNodes;
            _nodes[i].commercialArrivalTime = commercialAircraftTraversalTime;

            // Find traversal for a light aircraft.
            timeBetweenNodes = distance / (PlaneMan.Instance.lightAircraftBaseSpeed * _nodes[i - 1].speedModifier);
            lightAircraftTraversalTime += timeBetweenNodes;
            _nodes[i].lightAircraftArrivalTime = lightAircraftTraversalTime;

            // Find traversal for a light aircraft.
            timeBetweenNodes = distance / (PlaneMan.Instance.priorityBaseSpeed);
            priorityAircraftTraversalTime += timeBetweenNodes;
            _nodes[i].priorityArrivalTime = priorityAircraftTraversalTime;
        }
        _path.commercialAircraftTime = commercialAircraftTraversalTime;
        _path.lightAircraftTime = lightAircraftTraversalTime;
        _path.priorityAircraftTime = priorityAircraftTraversalTime;
    }
#endif
}