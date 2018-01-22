using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObserverObject : MonoBehaviour
{
    class ObserverData
    {
        public ObserverData(GameObject obj, string functionToCall)
        {
            this.obj = obj;
            this.functionToCall = functionToCall;
        }
        
        public GameObject obj;
        public string functionToCall;
    };

    Dictionary<string, List<ObserverData>> subscribers = new Dictionary<string, List<ObserverData>>();

    public void AddSubscriber(string eventName, GameObject obj, string functionToCall)
    {
        if (!subscribers.ContainsKey(eventName)) subscribers[eventName] = new List<ObserverData>();

        subscribers[eventName].Add(new ObserverData(obj, functionToCall));
    }

    public void RemoveSubscriber(GameObject obj)
    {
        foreach (var ev in subscribers)
        {
            ev.Value.RemoveAll(data => (data.obj == obj));
        }
    }

    public void DoEvent(string eventName, object passedData)
    {
        if (!subscribers.ContainsKey(eventName)) subscribers[eventName] = new List<ObserverData>();

        foreach (var data in subscribers[eventName])
        {
            data.obj.SendMessage(data.functionToCall, passedData, SendMessageOptions.DontRequireReceiver);
        }
    }
}