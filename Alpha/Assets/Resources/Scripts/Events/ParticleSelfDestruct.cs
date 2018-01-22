using UnityEngine;
using System.Collections;

public class ParticleSelfDestruct : MonoBehaviour
{
    public float Timer = 3;
	
	void Update ()
    {
        if (Timer > 0)
        {
            Timer -= 1 * Time.deltaTime;
        }
        else
        {
            Timer = 0;
            Destroy(this.gameObject);
        }
	}
}
