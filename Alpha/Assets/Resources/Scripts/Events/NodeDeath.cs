using UnityEngine;
using System.Collections;
using PlaneMan = Singleton<PlaneManager>;

public class NodeDeath : MonoBehaviour
{
    void Start()
    {
        transform.parent = PlaneMan.Instance.PathHolder.transform;
        transform.Rotate(0, 180, 0);
    }

	void Update ()
    {
	    if (!Application.isPlaying)
        {
            Destroy(this.gameObject);
        }
	}
}
