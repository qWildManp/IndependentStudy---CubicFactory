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
    [SerializeField] private Box playerInteractingBox;
    
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
        //EventBus.AddListener<Box>(EventTypes.RegisterPlayerAttachedBox,SetPlayerAttachBox);
        //EventBus.AddListener<Box>(EventTypes.RegisterPlayerInteractingBox,SetPlayerAttachBox);
        EventBus.AddListener(EventTypes.ClearPlayerInteractBox,ClearPlayerAttachBox);
        GridSystem.Instance.InitializeGrid();
    }

    private void OnDestroy()
    {
        //EventBus.RemoveListener<Box>(EventTypes.RegisterPlayerAttachedBox,SetPlayerAttachBox);
        EventBus.RemoveListener(EventTypes.ClearPlayerInteractBox,ClearPlayerAttachBox);
    }

    public void SetPlayerAttachBox(Box interactBox)// set player attached Box
    {
        this.playerAttachBox = interactBox;
    }
    public void ClearPlayerAttachBox()
    {
        this.playerAttachBox = null;
        this.playerInteractingBox = null;
    }

    public Box GetPlayerAttachedBox()// Get player attached Box
    {
        return playerAttachBox;
    }
}
