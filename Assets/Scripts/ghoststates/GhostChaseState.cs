using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostChaseState : GhostState
{
    //float timer = 0f
    // Start is called before the first frame update
    public override void Enter(GhostStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
    {
        //base.Enter(stateInput, transitionInfo);
        stateInput.anim.Play("GhostIdle");        
    }

    // Update is called once per frame
    public override void Update(GhostStateInput stateInput)
    {



        if (stateInput.enemy_controller.spotPlayerByDistance(stateInput.player.transform.position) == false)
        {
            character.ChangeState<GhostWanderState>();
        }
        else
        {
            stateInput.enemy_controller.TurnToFacePlayer(stateInput.player.transform.position);
            //stateInput.gameobj.transform.position = stateInput.player.transform.position;
            float distance = Vector2.Distance(stateInput.ghost.transform.position, stateInput.player.transform.position);
            Vector2 moveDirection = stateInput.player.transform.position - stateInput.ghost.transform.position;
            moveDirection.Normalize();
            moveDirection *= stateInput.ghost.maxSpeed;
            stateInput.rb.velocity = moveDirection;
            //Vector2.Lerp(stateInput.ghost.transform.position, stateInput.player.transform.position, );
             

        }

    }
}
