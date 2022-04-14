using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerState {

	private float jumpGraceTimer;

	public override void Enter(PlayerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
	{
		stateInput.anim.Play("Player_Fall");
		if (transitionInfo != null) {
			PlayerFallingTransitionInfo fallingTransitionInfo = (PlayerFallingTransitionInfo) transitionInfo;
			if (fallingTransitionInfo.wasGrounded) {
				jumpGraceTimer = stateInput.playerController.jumpGraceTime;
			} else {
				jumpGraceTimer = -1f;
			}
		}
		
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

		stateInput.playerController.checkGas(stateInput.player.GetButton("Jump"));

		if (stateInput.player.GetButtonDown("Grapple")) {
			stateInput.playerController.startGrapple();
		}

		if (stateInput.player.GetButtonUp("Grapple")) {
			stateInput.playerController.stopGrapple();
		}

				if (stateInput.player.GetAxis("MoveGrapple") != 0) {
			stateInput.playerController.changeGrappleLength(stateInput.player.GetAxis("MoveGrapple"));
		}

		if (jumpGraceTimer >= 0) {

			if (stateInput.player.GetButtonDown("Jump"))
			{
				stateInput.playerController.Jump();
				character.ChangeState<PlayerJumpingState>();
			}
			
			jumpGraceTimer -= Time.deltaTime;
		}

		if (stateInput.player.GetButtonDown("Jump") && (stateInput.playerController.touchingRightWall || stateInput.playerController.touchingLeftWall))
		{
			stateInput.rb.drag = 0;
			character.ChangeState<PlayerWallJumpingState>();
		}

		if (stateInput.playerController.isGrounded)
		{
			character.ChangeState<PlayerIdleState>();
		} else if (stateInput.playerController.touchingRightWall && stateInput.lastXDir == 1)
		{
			stateInput.lastWall = Physics2D.OverlapCircle(stateInput.playerController.rightWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall).gameObject;
			character.ChangeState<PlayerWallSlidingState>();
		} else if (stateInput.playerController.touchingLeftWall && stateInput.lastXDir == -1)
		{
			stateInput.lastWall = Physics2D.OverlapCircle(stateInput.playerController.leftWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall).gameObject;
			character.ChangeState<PlayerWallSlidingState>();
		}

		// Movement animations and saving previous input
		int horizontalMovement = (int)Mathf.Sign(stateInput.player.GetAxis("MoveHorizontal"));
		if (stateInput.player.GetAxis("MoveHorizontal") > -0.01f && stateInput.player.GetAxis("MoveHorizontal") < 0.01f) {
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

public class PlayerFallingTransitionInfo : CharacterStateTransitionInfo {

	public bool wasGrounded;
	public PlayerFallingTransitionInfo(bool wasGrounded) {
		this.wasGrounded = wasGrounded;
	}

}
