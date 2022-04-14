using System.Runtime.CompilerServices;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaunchState : PlayerState
{
    private GameObject currentWall;
    private float launchTime = 1f;
    private float timer;


    public override void Enter(PlayerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
    {
        if (transitionInfo == null) {
            timer = launchTime;
        } else {
            LaunchStateTransitionInfo launchTransition = (LaunchStateTransitionInfo) transitionInfo;
            timer = launchTransition.moveAfterLaunchTime;
            stateInput.rb.velocity = new Vector2(stateInput.spriteRenderer.flipX ? launchTransition.launchVelocity.x : -launchTransition.launchVelocity.x, launchTransition.launchVelocity.y);
            stateInput.playerController.SetPlayerInvincibility(launchTransition.invincible);
        }
        //stateInput.anim.Play("Player_Jump");
        stateInput.anim.Play("Player_Fall");
    }

    public override void Update(PlayerStateInput stateInput)
    {
        stateInput.playerController.isGrounded = stateInput.playerController.checkIfGrounded();
        
        if (stateInput.playerController.isGrounded && timer <= 0) {
            character.ChangeState<PlayerIdleState>();  
        }
        if (timer <= 0) {
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
        timer -= Time.deltaTime;
    }
    public override void FixedUpdate(PlayerStateInput stateInput)
    {
        if (timer <= 0)
        {
            stateInput.playerController.HandleLerpMovement();
        }
        

    }

    public override void ForceCleanUp(PlayerStateInput stateInput)
    {
    }

}

public class LaunchStateTransitionInfo : CharacterStateTransitionInfo
{
    public LaunchStateTransitionInfo(Vector2 launchVelocity, float moveTime, bool invincible) {
        this.launchVelocity = launchVelocity;
        this.moveAfterLaunchTime = moveTime;
        this.invincible = invincible;
    }
    public Vector2 launchVelocity;
    public float moveAfterLaunchTime;
    public bool invincible;

}
