using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/***
 * Usage: 
 * On active listening side (method must have corresponding parameter type):
 * EventBus.StartListening<T>(EventTypes..., MethodName);
 * EventBus.StopListening<T>(EventTypes..., MethodName);
 * EventBus.TriggerEvent<T>(EventTypes..., input);
 */

public class EventBus : MonoBehaviour
{
    public static EventBus Instance;
    private Hashtable eventHash = new Hashtable();

    private void Awake()
    {
        Instance = this;
    }

    public static void StartListening<T>(EventTypes.Action action, UnityAction<T> listener)
    {
        UnityEvent<T> thisevent = null;
        string b = GetKey<T>(action);
        if (Instance.eventHash.ContainsKey(b))
        {
            thisevent = (UnityEvent<T>)Instance.eventHash[b];
            thisevent.AddListener(listener);
            Instance.eventHash[action] = thisevent;
        }
        else
        {
            thisevent = new UnityEvent<T>();
            thisevent.AddListener(listener);
            Instance.eventHash.Add(b, thisevent);
        }
    }

    private static string GetKey<T>(EventTypes.Action action)
    {
        Type type = typeof(T);
        string key = type.ToString() + action.ToString();
        return key;
    }

    public static void StopListening<T>(EventTypes.Action action, UnityAction<T> listener)
    {
        string b = GetKey<T>(action);
        if (Instance.eventHash.ContainsKey(b))
        {
            UnityEvent<T> thisevent = (UnityEvent<T>)Instance.eventHash[b];
            thisevent.RemoveListener(listener);
            Instance.eventHash[action] = thisevent;
        }
    }

    public static void TriggerEvent<T>(EventTypes.Action action, T val)
    {
        UnityEvent<T> thisevent = null;
        string b = GetKey<T>(action);
        if (Instance.eventHash.ContainsKey(b))
        {
            thisevent = (UnityEvent<T>)Instance.eventHash[b];
            thisevent.Invoke(val);
        }
    }
}
