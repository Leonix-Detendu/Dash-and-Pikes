using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement et Dash")]
    public float moveSpeed;
    public float acceleration;
    public float decceleration;
    [Range(1, 2)] public float velPower;
    public float friction;
    private bool isDashing;
    public float dashRange;
    private float dashStack;
    private Vector2 dashVelocity;

    [Header("Detection du Sol")]
    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private float moveInput;
    public bool isGrounded;
    private bool wasGrounded;

    [Header("Reglages du saut et de la gravite")]
    private bool isJumping;
    public float jumpForce;
    public float defaultGS;
    public float jumpMultiplier;
    public float fallMultiplier;
    public float jumpCutMultiplier;
    public float maxFallSpeed;
    private float lastGroundedTimer;
    public float coyoteTime;
    private float jumpBufferTimer;
    public float jBMaxTime;

    [Header("Parametres de plateforme sur laquelle se tient le joueur")]
    public PlatformsBase currentPBase;
    private Vector2 lastPlatformVelocity;

    public float maxVelocityY;
    public float maxVelocityX;

    [HideInInspector] public Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip superJumpSound;
    [SerializeField] private AudioClip landingSound;
    private AudioSource audioSource;
    

    // Debug 
    private bool shouldDebug = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.position = new Vector3(0, 1.5f, 0);
        rb.gravityScale = defaultGS;
        isDashing = false;
        dashStack = 1;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
        // Récupère les entrées de déplacement horizontal
        moveInput = Input.GetAxisRaw("Horizontal");

        // Récupère la commande de saut
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jBMaxTime;
        }

        if (isGrounded && !isJumping)
        {
            lastGroundedTimer = coyoteTime;
        }

        lastGroundedTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;

        // Redonne toutes les stacks de dash lorsque le personnage touche le sol et n'est pas en train de dash
        if (isGrounded && !isDashing)
        {
            dashStack = 1;
        }

        // Lance la coroutine de dash lorsque l'on appuie sur Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashStack > 0)
        {
            StartCoroutine(dashRoutine());
        }

        // Commande de debug
        if (Input.GetKeyDown(KeyCode.B))
        {
            shouldDebug = true;
        }
    }

    void FixedUpdate()
    {

        Vector2 currentPlatformVelocity = platformCalculations();

        if (!isDashing)
        {
            animator.SetFloat("speed", Mathf.Abs(Run()));
            GroundFriction();
            Jump();
            GravityManagement();

            if ( Mathf.Abs(rb.linearVelocity.x) > maxVelocityX )
            {
                rb.linearVelocity = new Vector2( maxVelocityX * Mathf.Sign(rb.linearVelocity.x), rb.linearVelocity.y);
            }
            if ( rb.linearVelocityY > maxVelocityY ) 
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxVelocityY);
            }
        }
        else
        {
            rb.linearVelocity = dashVelocity + currentPlatformVelocity;
        }
    }

    private void TurnCharacter(bool canTurn)
    {
        if (canTurn)
        {
            if (moveInput > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (moveInput < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    // Coroutine réalisant l'action de dash
    IEnumerator dashRoutine()
    {
        isJumping = false;
        rb.gravityScale = defaultGS;

        TurnCharacter(true);
        animator.SetBool("isDashing", true);
        animator.SetBool("isGrounded", false);

        audioSource.PlayOneShot(dashSound);

        // Action de Dash
        Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (dashDirection.x == 0 && dashDirection.y == 0)
        {
            yield break;
        }
        isDashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = dashRange * dashDirection;
        dashVelocity = rb.linearVelocity;
        dashStack--;

        // Délai jusqu'à la fin du dash
        yield return new WaitForSeconds(0.2f);

        // Fin du dash
        rb.linearVelocity *= 0.5f;
        rb.gravityScale = defaultGS;
        isDashing = false;
        dashVelocity = Vector2.zero;
        animator.SetBool("isDashing", false);
    }

    private float Run()
    {
        float platformVelocityX = (currentPBase == null) ? 0f : currentPBase.platformVelocity.x;

        float targetVelocityX = (moveSpeed * moveInput) + platformVelocityX;

        float velocityDiff = targetVelocityX - rb.linearVelocity.x;

        float accelRate = (Mathf.Abs(moveSpeed * moveInput) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(velocityDiff) * accelRate, velPower) * Mathf.Sign(velocityDiff);

        rb.AddForce(movement * Vector2.right);

        return rb.linearVelocityX - platformVelocityX;
    }

    private void GroundFriction()
    {
        float targetVelocityX = (currentPBase == null) ? 0f : currentPBase.platformVelocity.x;

        float velocityDiff = targetVelocityX - rb.linearVelocity.x;

        if (isGrounded && moveInput == 0)
        {
            if (Mathf.Abs(velocityDiff) <= friction)
            {
                rb.AddForce(Vector2.right * velocityDiff * rb.mass, ForceMode2D.Impulse);
            }
        }
    }

    private void Jump()
    {

        // Action de saut
        if (jumpBufferTimer > 0 && lastGroundedTimer > 0 && !isJumping)
        {
            animator.SetTrigger("Jump");

            rb.gravityScale = defaultGS;

            float platformVelX = currentPBase != null ? currentPBase.platformVelocity.x : 0;
            float purePlayerSpeedX = rb.linearVelocity.x - platformVelX;

            rb.linearVelocity = new Vector2(purePlayerSpeedX, 0);

            if (currentPBase != null && currentPBase.canBoost && currentPBase.shouldBoost)
            {
                audioSource.PlayOneShot(superJumpSound);
                bool shouldBoostOnXAxis = (currentPBase.angleBoost <= Mathf.Abs(Mathf.PI / 2) && moveInput >= 0) || (currentPBase.angleBoost >= Mathf.Abs(Mathf.PI / 2) && moveInput <= 0 );
                if ( shouldBoostOnXAxis )
                {
                    rb.AddForce(currentPBase.boostVector, ForceMode2D.Impulse);
                    Debug.Log("BoostTotal : " + currentPBase.boostVector);
                }
                else
                {
                    Vector2 boostVectorOnlyOnYAxis = new Vector2(0, 1) * currentPBase.boostVector.y;
                    rb.AddForce(boostVectorOnlyOnYAxis, ForceMode2D.Impulse);
                    Debug.Log("Boost sur Y uniquement : " + boostVectorOnlyOnYAxis);
                }
            }
            else
            {
                audioSource.PlayOneShot(jumpSound);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
                
            isJumping = true;
            lastGroundedTimer = 0;
            jumpBufferTimer = 0;
        }
    }

    private void GravityManagement()
    {
        if (isJumping)
        {
            if (rb.linearVelocity.y > 0f && Input.GetButton("Jump"))
            {
                rb.gravityScale = jumpMultiplier;
            }
            else if (rb.linearVelocity.y > 0f)
            {
                rb.gravityScale = jumpCutMultiplier;
            }
        }

        if (rb.linearVelocity.y < 0f) rb.gravityScale = fallMultiplier;

        if (isGrounded) rb.gravityScale = defaultGS;

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    private Vector2 platformCalculations()
    {
        Collider2D groundCollider = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        isGrounded = groundCollider != null && !groundCollider.isTrigger;

        if (!wasGrounded && isGrounded)
        {
            audioSource.PlayOneShot(landingSound, 0.4f);
        }

        wasGrounded = isGrounded;

        animator.SetBool("isGrounded", isGrounded);

        TurnCharacter(isGrounded);

        if (isGrounded) isJumping = false;

        PlatformsBase newPBase = null;

        if (groundCollider != null) newPBase = groundCollider.GetComponent<PlatformsBase>();

        if (newPBase != null)
        {
            if (newPBase != currentPBase)
            {
                currentPBase = newPBase;
                lastPlatformVelocity = Vector2.zero;
            }
        }
        else
        {
            currentPBase = null;
            lastPlatformVelocity = Vector2.zero;
        }

        Vector2 currentPlatformVelocity = (currentPBase != null) ? currentPBase.platformVelocity : Vector2.zero;

        if (isGrounded && currentPBase != null && !isDashing )
        {
            Vector2 velocityChange = currentPlatformVelocity - lastPlatformVelocity;

            if (velocityChange.sqrMagnitude > 0.001f)
            {
                rb.linearVelocity += velocityChange;
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentPlatformVelocity.y);

        }

        lastPlatformVelocity = currentPlatformVelocity;

        return currentPlatformVelocity;
    }
}