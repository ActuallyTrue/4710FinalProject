using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostWanderState : GhostState
{
    Transform targetedWaypoint;
    // Start is called before the first frame update
    public override void Enter(GhostStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
    {
        //base.Enter(stateInput, transitionInfo);
        stateInput.anim.Play("GhostIdle");
        stateInput.rb.velocity = new Vector2(-stateInput.ghost.maxSpeed, 0f);
        targetedWaypoint = stateInput.ghost.waypoint_l;
    }

    // Update is called once per frame
    public override void Update(GhostStateInput stateInput)
    {
    
            if (targetedWaypoint == stateInput.ghost.waypoint_r)
            {
                stateInput.gameobj.transform.rotation = Quaternion.Euler(0,180,0);
                float distance = Vector2.Distance(stateInput.ghost.transform.position, targetedWaypoint.position);
                Vector2 moveDirection = targetedWaypoint.position - stateInput.ghost.transform.position;
                moveDirection.Normalize();
                moveDirection *= stateInput.ghost.maxSpeed;
                stateInput.rb.velocity = moveDirection;
                if (distance < 0.05f) {
                    targetedWaypoint = stateInput.ghost.waypoint_l;
                }
            }
            if (targetedWaypoint == stateInput.ghost.waypoint_l)
            {      
                stateInput.gameobj.transform.rotation = Quaternion.Euler(0,0,0);
                float distance = Vector2.Distance(stateInput.ghost.transform.position, targetedWaypoint.position);
                Vector2 moveDirection = targetedWaypoint.position - stateInput.ghost.transform.position;
                moveDirection.Normalize();
                moveDirection *= stateInput.ghost.maxSpeed;
                stateInput.rb.velocity = moveDirection;
               if (distance < 0.05f) {
                    targetedWaypoint = stateInput.ghost.waypoint_r;
                }
            }
        
        if (stateInput.enemy_controller.spotPlayerByDistance(stateInput.player.transform.position)) {
             stateInput.rb.velocity = Vector2.zero;
             character.ChangeState<GhostChaseState>();
             return;
        } 
    }
}
