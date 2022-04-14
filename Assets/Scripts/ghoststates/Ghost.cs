using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character<Ghost, GhostState, GhostStateInput>
{
    public float maxSpeed = 3f;
    public int attackRate = 1;
    public float attackStrength = 1.0f;
    public Transform waypoint;
    public Transform waypoint_l;
    public Transform waypoint_r;

    protected override void Init()
    {
        stateInput.ghost = this;
        stateInput.anim = GetComponent<Animator>();
        stateInput.spriteRenderer = GetComponent<SpriteRenderer>();
        stateInput.rb = GetComponent<Rigidbody2D>();
        stateInput.boxCollider = GetComponent<BoxCollider2D>();
        stateInput.stateChanged = false;
        stateInput.gameobj = gameObject;
        stateInput.enemy_controller = GetComponent<EnemyController>();
        stateInput.player = GameObject.FindGameObjectWithTag("Player");
    }
    protected override void SetInitialState()
    {
        ChangeState<GhostWanderState>();
    }

}

public abstract class GhostState : CharacterState<Ghost, GhostState, GhostStateInput>
{

}

public class GhostStateInput : CharacterStateInput
{
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public bool stateChanged;
    public GameObject lastWall;
    public int lastXDir;
    public Ghost ghost;
    public GameObject gameobj;
    public EnemyController enemy_controller;
    public GameObject player;
}
