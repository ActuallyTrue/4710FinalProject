using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState {

	public override void Enter(PlayerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
	{
		stateInput.lastXDir = 0;
		stateInput.anim.Play("Player_Idle");
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

		if (stateInput.player.GetButtonDown("Grapple")) {
			stateInput.playerController.startGrapple();
		}

		if (stateInput.player.GetButtonUp("Grapple")) {
			stateInput.playerController.stopGrapple();
		}

		if (stateInput.player.GetAxis("MoveGrapple") != 0) {
			stateInput.playerController.changeGrappleLength(stateInput.player.GetAxis("MoveGrapple"));
		}
		
		if (stateInput.player.GetButtonDown("Jump") && stateInput.playerController.isGrounded)
		{
			stateInput.playerController.Jump();
			character.ChangeState<PlayerJumpingState>();
		} else if((stateInput.rb.velocity.y < 0) && !stateInput.playerController.isGrounded)
		{
			character.ChangeState<PlayerFallingState>(new PlayerFallingTransitionInfo(true));
		} else {
			// // Movement animations and saving previous input
			 int horizontalMovement = (int)Mathf.Sign(stateInput.player.GetAxis("MoveHorizontal"));
			 if (stateInput.player.GetAxis("MoveHorizontal") > -0.01f && stateInput.player.GetAxis("MoveHorizontal") < 0.01f) {
				 horizontalMovement = 0;
			 }

			if (stateInput.lastXDir != horizontalMovement)
			{
			    if (horizontalMovement != 0)
			    {
			        stateInput.anim.Play("Player_Run");
			        stateInput.spriteRenderer.flipX = horizontalMovement == -1;
			    }
			    else
			    {
			        stateInput.anim.Play("Player_Idle");
			    }
			}
			stateInput.lastXDir = horizontalMovement;
		}
	}


	public override void FixedUpdate(PlayerStateInput stateInput) {
		stateInput.playerController.HandleMovement();
	}

}
