using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rewired;

public class StatePlayerController : MonoBehaviour
{
	//reference to gas bar in canvas
	public Image gasBar;

	private AudioManager sfxmanager;
	public float trueMoveSpeed = 6f;
	public float moveSpeed = 6f;

	public float maxGasTime = 10f;

	private float gasTimer;

	public float boostScale = 2f;
	public float accelerationTimeAirborne;
	public float accelerationTimeGrounded;
	private float velocityXSmoothing;
	public float moveAfterLaunchTime;
	private float moveAfterLaunchTimer;
	public Vector2 launchVelocity;

	[HideInInspector]
	public Vector2 moveInput;
	public Player player;
	public int playerId;

	//variables for variable jump height
	public float maxJumpVelocity;
	public float minJumpVelocity;
	public float jumpGraceTime = 5f/60f;

	//the player's rigidbody
	public Rigidbody2D rb;

	//Everything for being grounded
	[HideInInspector]
	public bool isGrounded;
	public float checkRadius;
	public LayerMask whatIsGround;
	public Transform groundCheck;

	
	//Everything for Wall Jumping
	public bool touchingRightWall;
	public bool touchingLeftWall;
	public LayerMask whatIsWall;
	public Transform rightWallCheck;
	public Transform leftWallCheck;
	private Transform wallCheckChanger;
	public float wallSlideDrag;
	public Vector2 wallJumpOffVelocity;

	//Everything for Grappling
	private LineRenderer lineRenderer;
	private Vector2 grapplePoint;

	[SerializeField]
	private float maxGrappleDistance;

	public SpringJoint2D joint2D;

	public int playerIndex;
	public float dashTime;
	public float dashSpeed;
	public float dashCooldownTime;
	private float dashCooldownTimer;
	public PlayerManager playerManager;

	public BoxCollider2D boxCollider;
	public GameObject playerCamera;

	private GameObject mainCam;

	public Vector2 dashDirection;

	public GameManager gameManager;

	public Animator anim;
	public SpriteRenderer spriteRenderer;
	[HideInInspector]
	public bool invincible = false;
	[HideInInspector]
	private bool damaged = false;
	public float invincibilityTime;

	public bool professorMode = false;

	void Start()
	{
		playerId = 0;
		player = ReInput.players.GetPlayer(playerId);
		player.controllers.hasKeyboard = true;
		gameManager = FindObjectOfType<GameManager>();
		rb = GetComponent<Rigidbody2D>();
		sfxmanager = FindObjectOfType<AudioManager>();
		playerManager = GetComponent<PlayerManager>();
		boxCollider = GetComponent<BoxCollider2D>();
		PlayerCamera camera = Instantiate(playerCamera).GetComponent<PlayerCamera>();
		mainCam = camera.gameObject;
		lineRenderer = GetComponent<LineRenderer>();
		camera.gameObject.SetActive(true);
		camera.makeFocusArea(this);
		gasTimer = maxGasTime;

		//setting reference to gas bar image in canvas
		gasBar = GameObject.Find("gas").GetComponent<Image>();

	}

	public void Update() {
		
		drawRope();
		updateDashCooldown();

		if (player.GetButtonDown("ProfessorMode")) {
			professorMode = !professorMode;
			Debug.Log("Professor Mode Engaged!");
		}
		Debug.Log(gasTimer);

	}

	public float CalculatePlayerVelocity(float RBvelocity, Vector2 input, float moveSpeed, float velocityXSmoothing, float accelerationTimeGrounded, float accelerationTimeAirborne, bool isGrounded)
	{
		float targetVelocityx = input.x * moveSpeed;
		return Mathf.SmoothDamp(RBvelocity, targetVelocityx, ref velocityXSmoothing, isGrounded ? accelerationTimeGrounded : accelerationTimeAirborne);
	}

	public bool checkIfGrounded() {
		Vector2 startingPoint1 = new Vector2(groundCheck.position.x - 0.5f, groundCheck.position.y);
		Vector2 startingPoint2 = new Vector2(groundCheck.position.x + 0.5f, groundCheck.position.y);
		RaycastHit2D hit1 = Physics2D.Raycast(startingPoint1, Vector2.down, 0.2f, whatIsGround);
		RaycastHit2D hit2 = Physics2D.Raycast(startingPoint2, Vector2.down, 0.2f, whatIsGround);
		if (hit1 || hit2) {
			return true;
		} else {
			return false;
		}
	}

	private void updateDashCooldown() {
		if (dashCooldownTimer > 0) {
			dashCooldownTimer -= Time.deltaTime;
		}
	}

	public void startDashCooldown() {
		dashCooldownTimer = dashCooldownTime;
	}

	public bool canDash() {
		return dashCooldownTimer <= 0;
	}

	public bool tookDamage() {
		return damaged;
	}

	public void setDamaged(bool enable) {
		damaged = enable;
	}

	//if you jump it changes your y velocity to the maxJumpVelocity
	public void Jump()
	{
		sfxmanager.sfx[4].Play();
		rb.velocity = new Vector2(rb.velocity.x, maxJumpVelocity);

	}

	public void JumpRelease()
	{
		if (rb.velocity.y > minJumpVelocity)
		{
			rb.velocity = new Vector2(rb.velocity.x, minJumpVelocity);
		}
	}

	public void checkGas(bool button)
	{
		if (button && gasTimer >= 0)
		{
			moveSpeed = trueMoveSpeed * boostScale;
			gasTimer -= Time.deltaTime;
			checkGasBarValid();
		}
		else {
			moveSpeed = trueMoveSpeed;
		}
	}

	public void addGas()
	{
		gasTimer = maxGasTime;
		checkGasBarValid();
	}

	public void WallJump(Vector2 jumpVelocity)
	{
		sfxmanager.sfx[4].Play();
		rb.velocity = jumpVelocity;
	}


	public void HandleMovement()
	{
		moveInput = player.GetAxis2D("MoveHorizontal", "MoveVertical");
		if (rb != null) {
			float xVelocity = CalculatePlayerVelocity(rb.velocity.x, moveInput, moveSpeed, velocityXSmoothing, accelerationTimeGrounded, accelerationTimeAirborne, isGrounded);
			rb.velocity = new Vector2(xVelocity, rb.velocity.y);
		}
	}

	public void startGrapple() {
		RaycastHit2D hit;
		Vector3 dir;
		// if (player.GetAxis("MoveHorizontal") >= 0) {
		// 	dir = 1;
		// } else
		// {
		// 	dir = -1;
		// }
		sfxmanager.sfx[3].Play();
		Vector3 mousePos = mainCam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.GetComponent<Camera>().nearClipPlane + 10));
		dir = mousePos - gameObject.transform.position;
		dir.z = 0;
		hit = Physics2D.Raycast(gameObject.transform.position, dir.normalized, maxGrappleDistance, whatIsGround);
		if (hit)
		{
			grapplePoint = hit.point;
			joint2D = gameObject.AddComponent<SpringJoint2D>();
			joint2D.autoConfigureConnectedAnchor = false;
			joint2D.connectedAnchor = grapplePoint;

			float distanceFromPoint = Vector2.Distance(gameObject.transform.position, grapplePoint);

			joint2D.distance = distanceFromPoint * 0.8f;

			joint2D.enableCollision = true;

			lineRenderer.positionCount = 2;
		}
	}

	public void stopGrapple() {
		lineRenderer.positionCount = 0;
		Destroy(joint2D);
	}

	void drawRope() {
		if (!joint2D) {
			return;
		}
		lineRenderer.SetPosition(0, gameObject.transform.position);
		lineRenderer.SetPosition(1, grapplePoint);
	}

	public void changeGrappleLength(float dir) {
		if (!joint2D) {
			return;
		}

		joint2D.distance +=  0.2f * dir;
	}

	public void HandleLerpMovement()
	{
		moveInput = player.GetAxis2D("MoveHorizontal", "MoveVertical");
		rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(moveInput.x * moveSpeed, rb.velocity.y)), 1f * Time.deltaTime);
	}

	public Vector2 clampTo8Directions(Vector2 vectorToClamp) {
		if (vectorToClamp.x > 0.1f && (vectorToClamp.y < 0.1f && vectorToClamp.y > -0.1f))
		{
			//right
			return new Vector2(1,0);
		}
		if (vectorToClamp.x > 0.1f && vectorToClamp.y < -0.1f)
		{
			//down right
			return new Vector2(1,-1).normalized;
		}
		if ((vectorToClamp.x < 0.1f && vectorToClamp.x > -0.1) && vectorToClamp.y < -0.1f)
		{
			//down
			return new Vector2(0,-1);
		}
		if (vectorToClamp.x < -0.1f && vectorToClamp.y < -0.1f)
		{
			//down left
			return new Vector2(-1,-1).normalized;
		}
		if (vectorToClamp.x < -0.1f && (vectorToClamp.y < 0.1f && vectorToClamp.y > -0.1f))
		{
			//left
			return new Vector2(-1,0);
		}
		if (vectorToClamp.x < -0.1f && vectorToClamp.y > 0.1f)
		{
			//up left
			return new Vector2(-1,1).normalized;
		}
		if ((vectorToClamp.x < 0.1f && vectorToClamp.x > -0.1) && vectorToClamp.y > 0.1f)
		{
			//up
			return new Vector2(0,1);
		}
		if (vectorToClamp.x > 0.1f && vectorToClamp.y > 0.1f)
		{
			//up right
			return new Vector2(1,1).normalized;
		}

		return Vector2.zero;
	}

	 //void OnCollisionEnter2D(Collision2D collision)
	 //{
	 //    if (collision.gameObject.CompareTag("Platform"))
	 //    {
	 //        transform.parent = collision.transform;
	 //    }
	 //}

	//private void OnCollisionExit2D(Collision2D collision)
	//{
	//    transform.parent = null;
	//}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == 12) { //if it's a hazard
			if (invincible == false) {
				setDamaged(true);
				SetPlayerInvincibility(true);
			}
		}
	}

	public void SetPlayerInvincibility(bool enable) {
		invincible = enable;
		if (enable) {
			StartCoroutine(InvincibleTimer());
		}
	}

	IEnumerator InvincibleTimer() {
		yield return new WaitForSeconds(invincibilityTime);
		SetPlayerInvincibility(false);
	} 


	//Updates the gas bar image
	public void updateGasBar()
    {
		gasBar.fillAmount = gasTimer / 10f;
    }

	/*
	This method checks to see if gas bar UI has been added to scene. 
	If it has it will call the function that updates the gas bar
	If it hasnt it will just print a message instead.
	 */
	public void checkGasBarValid() { 
		if (gasBar == null)
        {
			Debug.Log("WARNING: No gas bar canvas element added to level");
        } else
        {
			updateGasBar();
        }
	}
}
