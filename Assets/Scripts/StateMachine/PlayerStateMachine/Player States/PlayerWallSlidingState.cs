using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlidingState : PlayerState {
	public override void Enter(PlayerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
	{
		stateInput.rb.drag = stateInput.playerController.wallSlideDrag;
		stateInput.anim.Play("Player_Fall");
	}

	public override void ForceCleanUp(PlayerStateInput stateInput) {
		stateInput.rb.drag = 0;
		stateInput.playerController.touchingLeftWall = false;
		stateInput.playerController.touchingRightWall = false;
	}

	public override void Update(PlayerStateInput stateInput)
	{
		stateInput.playerController.isGrounded = stateInput.playerController.checkIfGrounded();

		stateInput.playerController.touchingRightWall = Physics2D.OverlapCircle(stateInput.playerController.rightWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall);

		stateInput.playerController.touchingLeftWall = Physics2D.OverlapCircle(stateInput.playerController.leftWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall);
		
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

		if (stateInput.player.GetButtonDown("Jump"))
		{

			if (stateInput.lastXDir > 0 && stateInput.playerController.touchingRightWall)
			{
				stateInput.rb.drag = 0;
				character.ChangeState<PlayerWallJumpingState>();
			}
			else if (stateInput.lastXDir < 0 && stateInput.playerController.touchingLeftWall)
			{
				stateInput.rb.drag = 0;
				character.ChangeState<PlayerWallJumpingState>();
			}
		} else if ((stateInput.playerController.touchingRightWall && stateInput.lastXDir == -1) || (stateInput.playerController.touchingLeftWall && stateInput.lastXDir == 1) || stateInput.lastXDir == 0)
		{
			character.ChangeState<PlayerFallingState>();
		}
		else if (stateInput.playerController.isGrounded)
		{
			stateInput.rb.drag = 0;
			character.ChangeState<PlayerIdleState>();
		} else if(!stateInput.playerController.touchingRightWall && !stateInput.playerController.touchingLeftWall)
		{
			 character.ChangeState<PlayerFallingState>();
		}

		// Movement animations and saving previous input
		int horizontalMovement = (int)Mathf.Sign(stateInput.player.GetAxis("MoveHorizontal"));
		if (stateInput.player.GetAxis("MoveHorizontal") > -0.1f && stateInput.player.GetAxis("MoveHorizontal") < 0.1f) {
			horizontalMovement = 0;
		}
		if (stateInput.lastXDir != horizontalMovement)
		{
			if (horizontalMovement != 0)
			{
				stateInput.spriteRenderer.flipX = horizontalMovement == -1;
			}
		}
		
		stateInput.lastXDir = horizontalMovement;
	}


	public override void FixedUpdate(PlayerStateInput stateInput)
	{
		stateInput.playerController.HandleMovement();
	}
}
