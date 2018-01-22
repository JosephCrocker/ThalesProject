using UnityEngine;
//using System.Collections;

public class CameraLookAt : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        target = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    void Update ()
    {
        transform.LookAt(target);
	}
}