using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] public ThirdPersonController player;

    [SerializeField] private Box playerAttachBox;
    
    [SerializeField] public CamRootRotation camRoot;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    protected override void Init()
    {
        base.Init();
        EventBus.AddListener(EventTypes.ClearPlayerInteractBox,ClearPlayerAttachBox);
        EventBus.AddListener<Box>(EventTypes.RemovePlayerInteractingBox,RemoveInteractingBox);
        GridSystem.Instance.InitializeGrid();
    }

    private void OnDestroy()
    {
        EventBus.RemoveListener(EventTypes.ClearPlayerInteractBox,ClearPlayerAttachBox);
        EventBus.RemoveListener<Box>(EventTypes.RemovePlayerInteractingBox,RemoveInteractingBox);
    }

    public bool SetPlayerAttachBox(Box interactBox)// set player attached Box
    {
        if (interactBox == this.playerAttachBox)
        {
            return false;
        }
        else
        {
            this.playerAttachBox = interactBox;
            return true;
        }
        
    }
    public void ClearPlayerAttachBox()
    {
        
            this.playerAttachBox = null;
            EventBus.Broadcast(EventTypes.ShowInteractHint,transform.position + new Vector3(0,2,0),false);
        
    }

    public void RemoveInteractingBox(Box interactingBox)
    {
        if (playerAttachBox == interactingBox)
        {
            this.playerAttachBox = null;
            EventBus.Broadcast(EventTypes.ShowInteractHint,transform.position + new Vector3(0,2,0),false);
            EventBus.Broadcast<bool>(EventTypes.DisableInteraction, false);
        }
    }
    public Box GetPlayerAttachedBox()// Get player attached Box
    {
        return playerAttachBox;
    }
}
