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
    [SerializeField] public bool gamePause;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePause)
            {
                EventBus.Broadcast(EventTypes.PauseGame,false);
                SoundManager.Instance.PlayBtnClick();
                UIManager.Instance.Resume();
            }
            else
            {
                EventBus.Broadcast(EventTypes.PauseGame,true);
                SoundManager.Instance.PlayBtnClick();
                UIManager.Instance.ShowPausePanel();
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        EventBus.AddListener(EventTypes.ClearPlayerInteractBox,ClearPlayerAttachBox);
        EventBus.AddListener<Box>(EventTypes.RemovePlayerInteractingBox,RemoveInteractingBox);
        EventBus.AddListener<bool>(EventTypes.PauseGame,PauseGame);
        GridSystem.Instance.InitializeGrid();
    }

    private void OnDestroy()
    {
        EventBus.RemoveListener(EventTypes.ClearPlayerInteractBox,ClearPlayerAttachBox);
        EventBus.RemoveListener<Box>(EventTypes.RemovePlayerInteractingBox,RemoveInteractingBox);
        EventBus.RemoveListener<bool>(EventTypes.PauseGame,PauseGame);
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

    private void PauseGame(bool result)
    {
        if (result)
        {
            Time.timeScale = 0;
            gamePause = true;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            gamePause = false;
            Cursor.visible = false;
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

    public void ReTryLevel()
    {
        StartCoroutine(DelayLoadLevel(SceneManager.GetActiveScene().name));
    }

    public void BackToMain()
    {
        StartCoroutine(DelayLoadLevel("StartScene"));
    }
    
    IEnumerator DelayLoadLevel(string LevelName)
    {
        yield return new WaitForSecondsRealtime(0.15f);
        Time.timeScale = 1;
        SceneManager.LoadScene(LevelName);
    }
}
