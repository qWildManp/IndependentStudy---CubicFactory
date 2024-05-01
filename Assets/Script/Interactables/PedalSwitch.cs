using System;
using Script.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedalSwitch : Floor
{
    public ControllerObjectBase[] controlledObjects;
    public bool playerInRange;
    public bool pedalActivated;
    public Material buttonMatInActivate;
    public Material buttonMatActivate;
    public MeshRenderer _renderer;
    private void Update()
    {
        if (!isElectrified)
        {
            pedalActivated = false;
            _renderer.materials[1].CopyPropertiesFromMaterial(buttonMatInActivate);
            foreach (var controlled in controlledObjects)
            {
                controlled.Deactivate();
            }
        }

        if (Input.GetKeyDown(KeyCode.F)&& playerInRange && isElectrified)
        {
            pedalActivated = true;
            GameManager.Instance.player.SetJumpAnim();
            _renderer.materials[1].CopyPropertiesFromMaterial(buttonMatActivate);
            UIManager.Instance.UpdateInteractPadalHint(transform.position,false);
            foreach (var controlled in controlledObjects)
            {
                
                controlled.Activate();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isElectrified && other.gameObject.CompareTag("Player"))
        {
            if(!pedalActivated)
                UIManager.Instance.UpdateInteractPadalHint(transform.position,true);
            playerInRange = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        UIManager.Instance.UpdateInteractPadalHint(transform.position,false);
        playerInRange = false;
        
    }
}
