using UnityEngine;
//using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public delegate void Action(params object[] args);
    private Dictionary<string, Action> _eventMap;

    void Awake()
    {
        _eventMap = new Dictionary<string, Action>();
    }

    public void AddListener(string eventName, Action handler)
    {
        if (!_eventMap.ContainsKey(eventName))
        {
            _eventMap.Add(eventName, handler);
        }
        else
        {
            _eventMap[eventName] += handler;
        }
    }

    public void RemoveListener(string eventName, Action handler)
    {
        if (_eventMap.ContainsKey(eventName))
        {
            _eventMap[eventName] -= handler;
        }
    }

    public void DoEvent(string eventName, params object[] args)
    {
        if (_eventMap.ContainsKey(eventName))
        {
            _eventMap[eventName](args);
        }
    }
}