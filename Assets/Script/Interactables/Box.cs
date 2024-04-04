using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactable
{
    public BoxID itemID;
   

    private void Update()
    {
        if (!isDisabled && itemID == BoxID.Debug)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            if (x != 0)
            {
                Move(x == 1 ? Direction.E : Direction.W);
            } else if (y != 0)
            {
                Move(y == 1 ? Direction.N : Direction.S);
            }
        }
    }
    
    

    // Box disappear after falling into pit
    public IEnumerator BoxFallInPit()
    {
        isDisabled = true;
        float elapsed = 0;
        while (elapsed < .5f)
        {
            transform.position -= 2 * Time.fixedDeltaTime * Vector3.up;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // TODO: will be replaced with other methods
        EventBus.Broadcast(EventTypes.ClearPlayerInteractBox);
        EventBus.Broadcast<bool>(EventTypes.DisableInteraction, false);
        yield return new WaitForSeconds(0.2f);
        //Destroy(gameObject);
    }
}
