using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    public Image oldMainBg;
    public Image newMainBg;
    public float fadeDuration = 1f; // Duration of fade effect in seconds
    public float moveSpeed = 1f; // Speed at which the camera moves towards the object
    public Transform targetObject; // The object the camera moves towards

    private Camera oldMainCamera;
    private Camera newMainCamera;

    void Start()
    {
        oldMainCamera = Camera.main;
        newMainCamera = GetComponent<Camera>();
        StartCoroutine(SwitchCamera());
    }


    IEnumerator SwitchCamera()
    {
        yield return new WaitForSeconds(1);
        // Fade in
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float clamp = Mathf.Clamp01(timer / fadeDuration);
            oldMainBg.color = new Color(0f, 0f, 0f, clamp);
            yield return null;
        }
        oldMainCamera.enabled = false;
        newMainCamera.enabled = true;
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float clamp = Mathf.Clamp01(timer / fadeDuration);
            newMainBg.color = new Color(0f, 0f, 0f, 1 - clamp);
            yield return null;
        }

        // Move camera towards object
        while (Vector3.Distance(newMainCamera.transform.position, targetObject.position) > 0.1f)
        {
            newMainCamera.transform.position = Vector3.MoveTowards(newMainCamera.transform.position, targetObject.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1);

        // Fade out
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float clamp = Mathf.Clamp01(timer / fadeDuration);
            newMainBg.color = new Color(0f, 0f, 0f, clamp);
            yield return null;
        }
        newMainCamera.enabled = false;
        oldMainCamera.enabled = true;
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float clamp = Mathf.Clamp01(timer / fadeDuration);
            oldMainBg.color = new Color(0f, 0f, 0f, 1 - clamp);
            yield return null;
        }
    }
}
