using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Path
{
    [SerializeField]
    [HideInInspector]
    [XmlElement("Name")]
    public string PathName;

    [SerializeField]
    [HideInInspector]
    [XmlElement("EntryGate")]
    public string entryGate;

    [SerializeField]
    [HideInInspector]
    [XmlElement("ExitGate")]
    public string exitGate;

    [SerializeField]
    [HideInInspector]
    [XmlElement("Commercial")]
    public float commercialAircraftTime;

    [SerializeField]
    [HideInInspector]
    [XmlElement("Light")]
    public float lightAircraftTime;

    [SerializeField]
    [HideInInspector]
    [XmlElement("Priority")]
    public float priorityAircraftTime;

    [XmlArray("Nodes")]
    [XmlArrayItem("Node")]
    [SerializeField]
    [HideInInspector]
    public List<Node>_nodes = new List<Node>();
        
    public void AddNode(NodeController _nodeController)
    {
        Vector4 routeData = new Vector4(_nodeController.commercialArrivalTime, _nodeController.lightAircraftArrivalTime, _nodeController.priorityArrivalTime, _nodeController.speedModifier);
        _nodeController._saveData.Init(_nodeController.transform.position, _nodeController.name, routeData);
        _nodes.Add(_nodeController._saveData);
    }

    public void Clear()
    {
        _nodes.Clear();
    }

    public void Save()
    {
        var serializer = new XmlSerializer(typeof(Path));
        var stream = new FileStream(Application.dataPath + "/Resources/Paths/" + PathName + ".xml", FileMode.Create);

        serializer.Serialize(stream, this);

        stream.Close();
        Debug.Log(PathName + " Saved");
    }
}