using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

/// <summary>
/// Used for serializing out node data.
/// </summary>
public class Node
{
    public string Name;

    public float posX, posY, posZ;
    public float commercial, light, priority;
    public float speedModifier;

    public void Init(Vector3 _position, string _name, Vector4 planeTravelData)
    {
        Name = _name;
        posX = _position.x;
        posY = _position.y;
        posZ = _position.z;

        commercial = planeTravelData.x;
        light = planeTravelData.y;
        priority = planeTravelData.z;
        speedModifier = planeTravelData.z;
    }
}