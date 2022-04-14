using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTimer; 
    private float lerper;   
    public override void Enter(PlayerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
    {
        //stateInput.canDash = false;
        //stateInput.playerController.dashing = true;
        stateInput.playerController.dashDirection = stateInput.playerController.clampTo8Directions(stateInput.player.GetAxis2D("MoveHorizontal", "MoveVertical"));
        if (stateInput.playerController.dashDirection.x == 0 && stateInput.playerController.dashDirection.y == 0) {
            if (stateInput.spriteRenderer.flipX == true) {
                stateInput.playerController.dashDirection = new Vector2(-1, 0);
            } else {
                stateInput.playerController.dashDirection = new Vector2(1, 0);
            }
        }
        stateInput.rb.velocity = stateInput.playerController.dashDirection * stateInput.playerController.dashSpeed;
        dashTimer = stateInput.playerController.dashTime;
        lerper = 0;
    }

    public override void ForceCleanUp(PlayerStateInput stateInput)
    {
        //stateInput.rb.velocity = Vector2.zero;
        stateInput.rb.drag = 0;
        stateInput.playerController.startDashCooldown();
    }

    public override void Update(PlayerStateInput stateInput)
    {

        stateInput.playerController.isGrounded = stateInput.playerController.checkIfGrounded();

        if (stateInput.playerController.tookDamage()) {
            stateInput.playerController.setDamaged(false);
            LaunchStateTransitionInfo transitionInfo = new LaunchStateTransitionInfo(stateInput.playerController.launchVelocity, stateInput.playerController.moveAfterLaunchTime, true);
            character.ChangeState<PlayerLaunchState>(transitionInfo);
            return;
        }
        
        if (stateInput.player.GetButtonDown("Jump") && stateInput.playerController.isGrounded)
        {
            stateInput.playerController.Jump();
            character.ChangeState<PlayerJumpingState>();
            return;
        }

        if (dashTimer > 0)
        { 
            dashTimer -= Time.deltaTime;
            lerper += Time.deltaTime;
            stateInput.rb.drag = Mathf.Lerp(18, 0, lerper / stateInput.playerController.dashTime);
        } else {
            character.ChangeState<PlayerFallingState>();
        }

        
    }
}
