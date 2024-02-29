using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentRot = transform.rotation.eulerAngles;
        Vector3 newRot = currentRot + new Vector3(0, rotateSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.Euler(newRot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            Destroy(gameObject);
    }
}
