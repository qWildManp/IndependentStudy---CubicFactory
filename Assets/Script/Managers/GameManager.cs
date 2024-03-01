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
        GridSystem.Instance.InitializeGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        { //ReloadScene();
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene("TutorialLevel");
    }
    protected override void Init()
    {
        base.Init();
        EventBus.AddListener<Box>(EventTypes.RegisterPlayerInteractBox,SetPlayerAttachBox);
        EventBus.AddListener(EventTypes.ClearPlayerInteractBox,ClearPlayerAttachBox);
    }

    private void SetPlayerAttachBox(Box interactBox)// set player attached Box
    {
        this.playerAttachBox = interactBox;
    }

    private void ClearPlayerAttachBox()
    {
        this.playerAttachBox = null;
    }

    public Box GetPlayerAttachedBox()// Get player attached Box
    {
        return playerAttachBox;
    }
}
