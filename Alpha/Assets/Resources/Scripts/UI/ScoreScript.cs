using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PlaneMan = Singleton<PlaneManager>;

public class ScoreScript : MonoBehaviour
{
    private float count = 0;
    public float m_Timer;
    public GameObject Ring;

    [Header("Time Gates")]
    public float EarlyTimeGate;
    public float LateTimeGate;
    public float OverTime;

    [Header("Scores")]
    public float EarlyScore;
    public float LateScore;
    public float OverTimeScore;

    void Update()
    {
        // ===== Timer Count ===== //
        if (m_Timer > 0)
        {   m_Timer -= 1 * Time.deltaTime; }
        else
        {   m_Timer = 0; }
        // ==== Selection Ring ==== //
        if (m_Timer <= LateTimeGate && m_Timer > OverTime && count == 0)
        {
            count += 1;
            Ring.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (m_Timer <= OverTime && count == 1)
        {
            count += 1;
            Ring.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ExitGate"))
        {
            if (m_Timer >= EarlyTimeGate)
            {
                PlaneMan.Instance.m_Score += EarlyScore;
                PlayerPrefs.SetFloat("Score", PlaneMan.Instance.m_Score);
                PlayerPrefs.Save();
            }
            else if (m_Timer >= LateTimeGate)
            {
                PlaneMan.Instance.m_Score += LateScore;
                PlayerPrefs.SetFloat("Score", PlaneMan.Instance.m_Score);
                PlayerPrefs.Save();
            }
            else if (m_Timer >= OverTime)
            {
                PlaneMan.Instance.m_Score += OverTimeScore;
                PlayerPrefs.SetFloat("Score", PlaneMan.Instance.m_Score);
                PlayerPrefs.Save();
            }
        }
    }

    public void InitTime(PlaneController currentPlane)
    {
        if (currentPlane.planeName == "Commercial")
        {
            EarlyTimeGate = currentPlane.Path.commercialAircraftTraversalTime;
            LateTimeGate = currentPlane.Path.commercialAircraftTraversalTime - 5;
            OverTime = currentPlane.Path.commercialAircraftTraversalTime - 13;
            m_Timer = EarlyTimeGate * 2;
        }
        else if (currentPlane.planeName == "LightAircraft")
        {
            EarlyTimeGate = currentPlane.Path.lightAircraftTraversalTime;
            LateTimeGate = currentPlane.Path.lightAircraftTraversalTime - 5;
            OverTime = currentPlane.Path.lightAircraftTraversalTime - 13;
            m_Timer = EarlyTimeGate * 2;
        }
        else if (currentPlane.planeName == "Priority") 
        {
            EarlyTimeGate = currentPlane.Path.priorityAircraftTraversalTime;
            LateTimeGate = currentPlane.Path.priorityAircraftTraversalTime - 5;
            OverTime = currentPlane.Path.priorityAircraftTraversalTime - 13;
            m_Timer = EarlyTimeGate * 2;
        }
    }
}