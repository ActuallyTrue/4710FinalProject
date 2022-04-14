using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissirdPatrol : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private Animator anim;
    [SerializeField] private BoxCollider2D boxCollider;
    private bool facingRight = true;
    private Rigidbody2D rb;
    public Transform waypoint_L;
    public Transform waypoint_R;

    public float grappleCutRadius = 0.5f;

    StatePlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(maxSpeed,0f);
        player = FindObjectOfType<StatePlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > waypoint_R.position.x)
        {
            transform.position = waypoint_R.position;
            facingRight = !facingRight;
            transform.rotation = Quaternion.Euler(0,180,0);
            rb.velocity = -rb.velocity;
        }
        if (transform.position.x < waypoint_L.position.x)
        {
            transform.position = waypoint_L.position;
            facingRight = !facingRight;
            transform.rotation = Quaternion.Euler(0,0,0);
            rb.velocity = -rb.velocity;
        }

        if (player.joint2D) {
            Vector2 playerPos = player.transform.position;
            Vector2 vectorToScissird = (Vector2)transform.position - playerPos;
            Vector2 vectorToAnchor = player.joint2D.connectedAnchor - playerPos;
            vectorToAnchor.Normalize();
            float dotProduct = Vector2.Dot(vectorToScissird, vectorToAnchor);
            dotProduct /= Vector2.SqrMagnitude(vectorToAnchor);
            Vector2 projectedVector = dotProduct * vectorToAnchor;
            if (Vector2.Distance(projectedVector, vectorToScissird) < grappleCutRadius) {
                player.stopGrapple();
            }
        }
    }
}
