using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T s_instance;
    public static T Instance
    {
        get
        {
            if (s_instance != null) return s_instance;

            s_instance = FindObjectOfType<T>(true);
            
            if (s_instance == null)
            {
            #if UNITY_EDITOR
                Debug.Log("Instance AddComponent!");
            #endif
                
                new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
            }
            else s_instance.Init();

            return s_instance;

        }
    }
    private void Awake()
    {
        if (s_instance != null&& s_instance != this as T)
        {
            Destroy(gameObject);
            return;
        }
        if (s_instance != null)
        {
            return;
        }
        
        //If no other object init this singleton then init it self
        s_instance = this as T;
        //Initialize Function
        Init();
    }

    protected virtual void Init()
    {
        
    }
}
