using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeSpeed = 1;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float e = 1;
        while (e > 0)
        {
            canvasGroup.alpha = e;
            e -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
