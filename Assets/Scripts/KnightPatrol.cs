using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightPatrol : MonoBehaviour
{
    private AudioManager sfxManager;
    public int randomGruntInd;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private Animator anim;
    [SerializeField] private BoxCollider2D boxCollider;
    private bool facingRight = true;
    private Rigidbody2D rb;
    public Transform waypoint_L;
    public Transform waypoint_R;

    // Start is called before the first frame update
    void Start()
    {
        sfxManager = FindObjectOfType<AudioManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(maxSpeed,0f);
    }

    // Update is called once per frame
    void Update()
    {
        randomGruntInd = Random.Range(0, 2);
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
        sfxManager.sfx[randomGruntInd].Play();
    }
}
