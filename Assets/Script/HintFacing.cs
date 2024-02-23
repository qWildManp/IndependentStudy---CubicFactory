using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintFacing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 camForward = GameManager.Instance.camRoot.transform.forward.normalized;
        camForward.y = 0;
        transform.forward = camForward;
        transform.position = GameManager.Instance.player.transform.position + new Vector3(0, 2, 0);
    }
}
