using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed;
	
	void Update ()
    {
        this.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
	}
}
