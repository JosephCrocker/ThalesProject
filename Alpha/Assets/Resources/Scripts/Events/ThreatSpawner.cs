using UnityEngine;
using System.Collections;

public class ThreatSpawner : MonoBehaviour
{
    public GameObject Cloud;
    public GameObject UnstableCloud;
    public GameObject SevereCloud;

    public GameObject Location1;
    public GameObject Location2;
    public GameObject Location3;

    public GameObject MapCloud;
    public GameObject MapWeather;

    private float SpawnRange;
    private float CloudRange;

    private PlaneController m_currentSelected;

    void Start ()
    {
	    SpawnRange = Random.Range(1, 4);
        CloudRange = Random.Range(1, 4);

        // Cloud Range Check
        if (CloudRange == 1) 
        { MapWeather = Cloud;
        }
        else if (CloudRange == 2)
        { MapWeather = UnstableCloud;
        }
        else
        { MapWeather = SevereCloud;
        }
        
        // Spawn Range Check
        if (SpawnRange == 1)
        {
            MapCloud = Instantiate(MapWeather, Location1.transform.position, transform.rotation) as GameObject;
        }
        else if (SpawnRange == 2)
        {
            MapCloud = Instantiate(MapWeather, Location2.transform.position, transform.rotation) as GameObject;
        }
        else
        {
            MapCloud = Instantiate(MapWeather, Location3.transform.position, transform.rotation) as GameObject;
        }
        MapCloud.transform.parent = transform.Find("Clouds");
    }
}