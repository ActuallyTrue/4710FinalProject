using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerManager : Character<PlayerManager, PlayerState, PlayerStateInput> {

	override protected void Init()
    {
        //gameObject.name = GetComponent<PhotonView>().Owner.NickName;
        stateInput.stateMachine = this;
        stateInput.playerController = GetComponent<StatePlayerController>();
        stateInput.anim = stateInput.playerController.anim;
        stateInput.spriteRenderer = stateInput.playerController.spriteRenderer;
        stateInput.rb = GetComponent<Rigidbody2D>();
        stateInput.boxCollider = GetComponent<BoxCollider2D>();
        stateInput.stateChanged = false;
        stateInput.player = ReInput.players.GetPlayer(0);
        stateInput.playerController.player = stateInput.player;
        stateInput.gameManager = FindObjectOfType<GameManager>();
    }

    override protected void SetInitialState()
    {
        ChangeState<PlayerIdleState>();
    }

    public PlayerStateInput GetStateInput() {
        return stateInput;
    }

    public PlayerState GetState() {
        return state;
    }

}

public abstract class PlayerState : CharacterState<PlayerManager, PlayerState, PlayerStateInput>
{

}

public class PlayerStateInput : CharacterStateInput
{
    
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public StatePlayerController playerController;
    public bool stateChanged;
    public GameObject lastWall;
    public int lastXDir;
    public Player player;
    public PlayerManager stateMachine;

    public GameManager gameManager;
}
