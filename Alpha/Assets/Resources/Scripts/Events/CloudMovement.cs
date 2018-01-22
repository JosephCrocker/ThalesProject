using UnityEngine;
using System.Collections;
using ThreatManager = Singleton<ThreatSpawner>;

public class CloudMovement : MonoBehaviour
{
    Rigidbody rb;
    public float MovementSpeed;
    [Header("Weather Bools")]
    public bool WeatherSystemOne = false;
    public bool WeatherSystemTwo = false;
    public bool WeatherSystemThree = false;

    void Start () { rb = this.GetComponent<Rigidbody>(); }
	
	void FixedUpdate ()
    {
        MovementSpeed = Time.fixedDeltaTime;
	    if (WeatherSystemOne == true)
        {
            rb.MovePosition(Vector3.MoveTowards(rb.position, ThreatManager.Instance.Location2.transform.position, MovementSpeed));
        }
        if (WeatherSystemTwo == true)
        {
            rb.MovePosition(Vector3.MoveTowards(rb.position, ThreatManager.Instance.Location3.transform.position, MovementSpeed));
        }
        if (WeatherSystemThree == true)
        {
            rb.MovePosition(Vector3.MoveTowards(rb.position, ThreatManager.Instance.Location1.transform.position, MovementSpeed));
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "WS1")
        {
            WeatherSystemThree = false;
            WeatherSystemOne = true;
        }
        if (collider.gameObject.name == "WS2")
        {
            WeatherSystemOne = false;
            WeatherSystemTwo = true;
        }
        if (collider.gameObject.name == "WS3")
        {
            WeatherSystemTwo = false;
            WeatherSystemThree = true;
        }
    }
}