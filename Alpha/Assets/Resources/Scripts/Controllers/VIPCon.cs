using UnityEngine;
//using System.Collections;
using Jarvis = Singleton<CoPilot>;
using PlaneMan = Singleton<PlaneManager>;
using VipMan = Singleton<VipManager>;

public class VIPCon : MonoBehaviour
{
    public bool isHeli;
    public float Speed;
    // ==== PUBLIC ==== //
    public PlaneController Manager;
    public bool RayHit = false;
    // ==== PRIVATE ==== //
    private float count = 0;
    private GameObject newGameEvent;
    private CrashEvent newCrashEvent;

	void Update ()
    {
        PlaneRay(RayHit);
        float step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, VipMan.Instance.EndPoint.transform.position, step);
        transform.LookAt(VipMan.Instance.EndPoint.transform);
	}

    private void PlaneRay(bool Hit)
    {
        RaycastHit objectHit;
        Vector3 forwardPos = this.transform.TransformDirection(Vector3.forward);
        Vector3 rightPos = this.transform.TransformDirection(Vector3.right);
        Vector3 leftPos = this.transform.TransformDirection(Vector3.left);

        Debug.DrawRay(this.transform.position, forwardPos * 25, Color.red);
        Debug.DrawRay(this.transform.position, rightPos * 15, Color.red);
        Debug.DrawRay(this.transform.position, leftPos * 15, Color.red);

        if (Physics.Raycast(this.transform.position, forwardPos, out objectHit, 25) ||
            Physics.Raycast(this.transform.position, rightPos, out objectHit, 15) ||
            Physics.Raycast(this.transform.position, leftPos, out objectHit, 15))
        {
            if (objectHit.transform.tag == "Plane" && Hit == false)
            {
                Manager = objectHit.transform.GetComponent<PlaneController>();
                Hit = true;
            }
        }

        if (Hit && count == 0)
        {
            PlaneMan.Instance.AudioPlay(PlaneMan.Instance.AlertNotify);
            if (isHeli == false) { newGameEvent = Instantiate(Jarvis.Instance.vipEventPrefab) as GameObject; }
            else if (isHeli == true) { newGameEvent = Instantiate(Jarvis.Instance.heliEventPrefab) as GameObject; }
            CrashEvent(this.gameObject, Manager, Hit);
            count += 1;
        }
        else if (!Hit)
        {
            CrashEvent(this.gameObject, Manager, Hit);
            Manager = null;
            count = 0;
        }
    }

    public void CrashEvent(GameObject _a, PlaneController _b, bool hit)
    {
        if (hit)
        {
            newCrashEvent = newGameEvent.GetComponent<CrashEvent>();
            newCrashEvent.InitializeNewThreat(_b, _a);
            newGameEvent.transform.SetParent(Jarvis.Instance.EventTransform);
            Jarvis.Instance.Events.Add(newCrashEvent);
            Jarvis.Instance.EventGameObjects.Add(newGameEvent);

            newGameEvent.transform.position = Jarvis.Instance.alertButton.transform.position;
            float newY = newGameEvent.transform.position.y - (Jarvis.Instance.EventGameObjects.Count * Jarvis.Instance.m_notificationOffset);
            TransformExtensions.SetYPos(newGameEvent.transform, newY);
            newGameEvent.transform.localScale = new Vector3(1f, 1f, 1f);

            Jarvis.Instance.alertNotification.SetActive(true);
            Jarvis.Instance.alertNumber.text = Jarvis.Instance.Events.Count.ToString();
        }
        else if (!hit)
        {
            if (newCrashEvent != null)
            {
                Jarvis.Instance.Events.Remove(newCrashEvent);
                Jarvis.Instance.alertNumber.text = Jarvis.Instance.Events.Count.ToString();
                Jarvis.Instance.EventGameObjects.Remove(newGameEvent);
                Jarvis.Instance.ReAssessEvents(_b);
                Destroy(newGameEvent);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "ExitGate")
        {
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Plane")
        {
            CrashEvent(this.gameObject, Manager, RayHit);
            Destroy(this.gameObject);
            Instantiate(PlaneMan.Instance.OnDeathParticles, this.transform.position, this.transform.rotation);
        }
    }
}