using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class MenuUIManager : MonoSingleton<MenuUIManager>
{
    public CanvasGroup startGamePanel;
    public CanvasGroup levelSelectPanel;
    public CanvasGroup coverImg;

    private Tween fadeTween;
    public void StartLevel(string LevelName)
    {
        StartCoroutine(DelayLoadLevel(LevelName));
    }

    IEnumerator DelayLoadLevel(string LevelName)
    {
        yield return new WaitForSeconds(0.15f);
        SceneManager.LoadScene(LevelName);
    }
    public void ExitGame()
    {
        DOVirtual.DelayedCall(0.15f, Application.Quit);
    }

    public void StartGame()
    {
        startGamePanel.DOFade(0, 0.5f).OnComplete(() =>
        {
            startGamePanel.interactable = false;
            startGamePanel.blocksRaycasts = false;
            levelSelectPanel.interactable = true;
            levelSelectPanel.blocksRaycasts = true;
            levelSelectPanel.transform.DOMoveX(350f, 0.5f);
        });
    }

    public void BackToMainMenu()
    {
        levelSelectPanel.transform.DOMoveX(-500, 0.5f).OnComplete(() =>
        {
            levelSelectPanel.interactable = false;
            levelSelectPanel.blocksRaycasts = false;
            startGamePanel.interactable = true;
            startGamePanel.blocksRaycasts = true;
            startGamePanel.DOFade(1, 0.5f);
        });
    }

    public void BackToStartScene()
    {
        StartCoroutine(FadeIn());
        StartCoroutine(DelayLoadStartScene());
    }

    IEnumerator DelayLoadStartScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("StartScene");
    }

    IEnumerator FadeIn()
    {
        for(float i = 0; i < 1f; i += Time.deltaTime)
        {
            coverImg.alpha += i;
            yield return new WaitForSeconds(0.02f);
        }
    }

}

