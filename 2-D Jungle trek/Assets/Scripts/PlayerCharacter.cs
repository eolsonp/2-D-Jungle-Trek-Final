﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private float accelerationForce = 5;
    [SerializeField]
    private float maxSpeed = 5;
    [SerializeField]
    private float jumpForce = 10;
    [SerializeField]
    private Rigidbody2D rb2d;
    [SerializeField]
    private Collider2D playerGroundCollider;
    [SerializeField]
    private PhysicsMaterial2D playerMovingPhysicsMaterial, playerStoppingPhysicsMaterial;
    [SerializeField]
    private Collider2D groundDetectTrigger;
    [SerializeField]
    private ContactFilter2D groundContactFilter;
    private float horizontalInput;
    private bool isOnGround;
    private bool canDoubleJump = false;
    bool facingRight = true;
    private Transform groundCheck;
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    private Collider2D[] groundHitDetectionResults = new Collider2D[16];
    private Checkpoint currentCheckpoint;
    Animator anim;
	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateIsOnGround();
        UpdateHorizontalInput();
        HandleJumpInput();
    }
    private void FixedUpdate()
    {
        UpdatePhysicsMaterial();
        Move();
        if (isOnGround)
            canDoubleJump = false;
        anim.SetFloat("vSpeed", rb2d.velocity.y);
    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    private void UpdatePhysicsMaterial()
    {
        if (Mathf.Abs(horizontalInput) > 0)
        {
            playerGroundCollider.sharedMaterial = playerMovingPhysicsMaterial;
        }
        else
        {
            playerGroundCollider.sharedMaterial = playerStoppingPhysicsMaterial;
        }
    }
    private void UpdateIsOnGround()
    {
       isOnGround = groundDetectTrigger.OverlapCollider(groundContactFilter, groundHitDetectionResults) > 0;
        anim.SetBool("Ground", isOnGround);
       //Debug.Log("IsOnGround?: " + isOnGround);
    }
    private void UpdateHorizontalInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }
    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump")  && (isOnGround || !canDoubleJump))
         {
             rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (!canDoubleJump && !isOnGround)
                canDoubleJump = true;
         }    
    }
    private void Move()
    {
        rb2d.AddForce(Vector2.right * horizontalInput * accelerationForce);
        Vector2 clampedVelocity = rb2d.velocity;
        clampedVelocity.x = Mathf.Clamp(rb2d.velocity.x, -maxSpeed, maxSpeed);
        rb2d.velocity = clampedVelocity;
        if (horizontalInput > 0 && !facingRight)
            Flip();
        else if (horizontalInput < 0 && facingRight)
            Flip();
    }
    public void Respawn()
    {
        if (currentCheckpoint == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            rb2d.velocity = Vector2.zero;
            transform.position = currentCheckpoint.transform.position;
        }
    }
    public void SetCurrentCheckpoint(Checkpoint newCurrentCheckpoint)
    {
        if(currentCheckpoint != null)
            currentCheckpoint.SetIsActivated(false);

        currentCheckpoint = newCurrentCheckpoint;
        currentCheckpoint.SetIsActivated(true);
    }
}
