using UnityEngine;
using System.Collections;

public class DrawLineInFrontOff : MonoBehaviour
{
    ///<summary>
    ///Draw Line in front of plane using GL lines
    ///</summary>

    public Material forwardLine;
    public Transform inFront;

    //private Vector3 planePosition;
    //private Vector3 forwardPosition;

    void Update()
    {
        //forwardPosition = new Vector3 (inFront.transform.position.x, inFront.transform.position.y, inFront.transform.position.z);
        //planePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }
}