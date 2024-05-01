using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    // Start is called before the first frame update
    public GameObject inWorldHint;
    public GameObject inWorldHintInteractPedal;
    public CanvasGroup pausePanel;
    public TextMeshProUGUI collectCheeseResult;
    private void Awake()
    {
        EventBus.AddListener<Vector3,bool>(EventTypes.ShowInteractHint,UpdateInteractHint);
    }

    private void OnDestroy()
    {
        EventBus.RemoveListener<Vector3,bool>(EventTypes.ShowInteractHint,UpdateInteractHint);
    }

    void UpdateInteractHint(Vector3 pos,bool show)
    {
        if (inWorldHint)
        {
            inWorldHint.transform.position = pos;
            inWorldHint.SetActive(show);
        }
        
    }

    public void UpdateInteractPadalHint(Vector3 pos, bool show)
    {
        if (inWorldHintInteractPedal)
        {
            inWorldHintInteractPedal.transform.position = pos;
            inWorldHintInteractPedal.SetActive(show);
        }
    }
    public void ShowPausePanel()
    {
        pausePanel.gameObject.SetActive(true);
    }

    public void Resume()
    {
        pausePanel.gameObject.SetActive(false);
        EventBus.Broadcast(EventTypes.PauseGame,false);
    }

    public void UpdateCollectedCheese()
    {
        collectCheeseResult.text = GameManager.Instance.levelCollect + " / " + GameManager.Instance.maxCollect;
    }
}
