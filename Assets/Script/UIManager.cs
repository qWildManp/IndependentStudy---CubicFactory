using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    // Start is called before the first frame update
    public GameObject inWorldHint;

    private void Awake()
    {
        EventBus.AddListener<Vector3,bool>(EventTypes.ShowInteractHint,UpdateInteractHint);
    }

    private void OnDestroy()
    {
        EventBus.RemoveListener<Vector3,bool>(EventTypes.ShowInteractHint,UpdateInteractHint);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateInteractHint(Vector3 pos,bool show)
    {
        if (inWorldHint)
        {
            inWorldHint.transform.position = pos;
            inWorldHint.SetActive(show);
        }
        
    }
}
