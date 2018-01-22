using UnityEngine;
using UnityEditor;
using System.Collections;

public class CustomObjects : MonoBehaviour {

    [MenuItem("Custom/Add Path")]
	// Use this for initialization
	static void AddPath ()
    {
        GameObject path = Resources.Load("Prefabs/Path") as GameObject;
        Instantiate(path);
	}
	
}
