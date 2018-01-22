using UnityEngine;
using System.Collections;

public class VipManager : MonoBehaviour
{
    [Header("VIP Objects")]
    public GameObject VipZepPrefab;
    public GameObject VipHeliPrefab;
    public GameObject VipJetPrefab;
    public GameObject StartPoint;
    public GameObject EndPoint;
    private int CreationInt;

    void Start ()
    {
        CreationInt = Random.Range(1, 4);
        if (CreationInt == 1)
        { Instantiate(VipZepPrefab, StartPoint.transform.position, StartPoint.transform.rotation); }
        else if (CreationInt == 2)
        { Instantiate(VipHeliPrefab, StartPoint.transform.position, StartPoint.transform.rotation); }
        else if (CreationInt == 3)
        { Instantiate(VipJetPrefab, StartPoint.transform.position, StartPoint.transform.rotation); }
    }
	
	void Update ()
    {
	}
}
