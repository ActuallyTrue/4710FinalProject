using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerWallJumpingState : PlayerState
{
	private bool canMove;
	private GameObject currentWall;

	private float wallTouchTime = 0.1f;
	private float wallTouchTimer;

	public override void Enter(PlayerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
	{
		stateInput.playerController.touchingLeftWall = false;
		stateInput.playerController.touchingRightWall = false;
		stateInput.playerController.WallJump(new Vector2((stateInput.spriteRenderer.flipX ? 1 : -1) * stateInput.playerController.wallJumpOffVelocity.x, stateInput.playerController.wallJumpOffVelocity.y));
		stateInput.spriteRenderer.flipX = !stateInput.spriteRenderer.flipX;
		wallTouchTimer = wallTouchTime;
		stateInput.anim.Play("Player_Jump");
	}

	public override void Update(PlayerStateInput stateInput)
	{
		stateInput.playerController.isGrounded = stateInput.playerController.checkIfGrounded();

		stateInput.playerController.touchingRightWall = Physics2D.OverlapCircle(stateInput.playerController.rightWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall);

		stateInput.playerController.touchingLeftWall = Physics2D.OverlapCircle(stateInput.playerController.leftWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall);

		wallTouchTimer -= Time.deltaTime;
		
		if (stateInput.playerController.tookDamage()) {
			stateInput.playerController.setDamaged(false);
			LaunchStateTransitionInfo transitionInfo = new LaunchStateTransitionInfo(stateInput.playerController.launchVelocity, stateInput.playerController.moveAfterLaunchTime, true);
			character.ChangeState<PlayerLaunchState>(transitionInfo);
			return;
		}

		if (wallTouchTimer <= 0 && (stateInput.playerController.touchingRightWall || stateInput.playerController.touchingLeftWall)) {
			stateInput.rb.velocity = new Vector2(0, stateInput.rb.velocity.y);
		}
		if (stateInput.player.GetButtonDown("Jump") && (stateInput.playerController.touchingRightWall || stateInput.playerController.touchingLeftWall))
		{
			stateInput.rb.drag = 0;
			character.ChangeState<PlayerWallJumpingState>();
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

		if (stateInput.playerController.touchingLeftWall)
		{
			currentWall = Physics2D.OverlapCircle(stateInput.playerController.leftWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall).gameObject;
		}
		else if (stateInput.playerController.touchingRightWall)
		{
			currentWall = Physics2D.OverlapCircle(stateInput.playerController.rightWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall).gameObject;
		}

		if ((stateInput.playerController.touchingRightWall && stateInput.lastXDir == 1) && !GameObject.ReferenceEquals(currentWall, stateInput.lastWall))
		{
			stateInput.lastWall = Physics2D.OverlapCircle(stateInput.playerController.rightWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall).gameObject;
			currentWall = null;
			character.ChangeState<PlayerWallSlidingState>();
		}
		else if (stateInput.playerController.touchingLeftWall && stateInput.lastXDir == -1 && !GameObject.ReferenceEquals(currentWall, stateInput.lastWall))
		{

			stateInput.lastWall = Physics2D.OverlapCircle(stateInput.playerController.leftWallCheck.position, stateInput.playerController.checkRadius, stateInput.playerController.whatIsWall).gameObject;
			currentWall = null;
			character.ChangeState<PlayerWallSlidingState>();
		} else if (stateInput.playerController.isGrounded)
		{
			character.ChangeState<PlayerIdleState>();
		} else if (stateInput.rb.velocity.y <= 0)
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
				//stateInput.spriteRenderer.flipX = horizontalMovement == -1;
			}
		}
		//stateInput.lastXDir = horizontalMovement;
	}

	public override void ForceCleanUp(PlayerStateInput stateInput)
	{
		base.ForceCleanUp(stateInput);
		stateInput.lastXDir = 0;
	}
	public override void FixedUpdate(PlayerStateInput stateInput)
	{
			stateInput.playerController.HandleLerpMovement();
		
	}
}