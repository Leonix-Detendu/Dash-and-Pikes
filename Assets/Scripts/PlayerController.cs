using System;
using System.Collections;
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

    [Header("Detection du Sol")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private float moveInput;
    public bool isGrounded;

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
    private float inertiaJumpTimer;
    public float inertiaJumpMaxTime;
    private bool inertiaChange;
    private Vector2 platformBoost;
    

    [Header("Parametres de plateforme sur laquelle se tient le joueur")]
    public PlatformsBase currentPBase;
    private Vector2 lastPlatformVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = defaultGS;
        isDashing = false;
        dashStack = 1;
        platformBoost = Vector2.zero;
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
    }

    void FixedUpdate()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        isGrounded = groundCollider != null;

        if (isGrounded) isJumping = false;

        PlatformsBase newPlatform = null;

        if (isGrounded && groundCollider.CompareTag("Platform"))
        {
            newPlatform = groundCollider.GetComponent<PlatformsBase>();
        }

        if (newPlatform != null && currentPBase != newPlatform)
        {
            currentPBase = newPlatform;
            lastPlatformVelocity = currentPBase.platformVelocity;
        }

        else if (newPlatform == null)
        {
            currentPBase = null;
            lastPlatformVelocity = Vector2.zero;
        }

        Vector2 currentPlatformVelocity = (currentPBase != null) ? currentPBase.platformVelocity : Vector2.zero;

        if (isGrounded && currentPBase != null)
        {
            Vector2 platformVelocityChange = currentPlatformVelocity - lastPlatformVelocity;

            if (platformVelocityChange.sqrMagnitude > 0.001f)
            {
                rb.linearVelocity += platformVelocityChange;
            }
        }

        lastPlatformVelocity = currentPlatformVelocity;

        if (!isDashing)
        {

            Run();

            GroundFriction();

            Jump();

            GravityManagement();
            
        }
    }

    // Coroutine réalisant l'action de dash
    IEnumerator dashRoutine()
    {
        isJumping = false;
        rb.gravityScale = defaultGS;

        // Action de Dash
        Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (dashDirection.x == 0 && dashDirection.y == 0)
        {
            yield break;
        }
        isDashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = dashRange * dashDirection;
        dashStack--;

        // Délai jusqu'à la fin du dash
        yield return new WaitForSeconds(0.2f);

        // Fin du dash
        rb.linearVelocity *= 0.5f;
        rb.gravityScale = defaultGS;
        isDashing = false;
    }

    private void Run()
    {
        float platformVelocityX = (currentPBase == null) ? 0f : currentPBase.platformVelocity.x;

        float targetVelocityX = (moveSpeed * moveInput) + platformVelocityX;

        float velocityDiff = targetVelocityX - rb.linearVelocity.x;

        float accelRate = (Mathf.Abs(moveSpeed * moveInput) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(velocityDiff) * accelRate, velPower) * Mathf.Sign(velocityDiff);

        rb.AddForce(movement * Vector2.right);
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
        // Gestion de la mémoire de l'inertie de la plateforme et du timer avant expiration de l'inertie, si inertie prise en compte
        if ( currentPBase?.takeInertia == true)
        {
            Vector2 currentVel = currentPBase.platformVelocity;

            bool changedDirX = (currentVel.x > 0.1f && platformBoost.x < -0.1f) || (currentVel.x < -0.1f && platformBoost.x > 0.1f);

            bool changedDirY = (currentVel.y > 0.1f && platformBoost.y < -0.1f) || (currentVel.y < -0.1f && platformBoost.y > 0.1f);

            bool isStopped = currentVel.sqrMagnitude < 0.01f;

            if (!isStopped && !changedDirX && !changedDirY)
            {
                inertiaJumpTimer = inertiaJumpMaxTime;

                platformBoost = currentVel;
            }
            else
            {
                inertiaJumpTimer -= Time.fixedDeltaTime;

                if (inertiaJumpTimer <= 0)
                {
                    platformBoost = currentVel;

                    if (!isStopped) inertiaJumpTimer = inertiaJumpMaxTime;
                }
            }
        }
        else
        {
            inertiaJumpTimer -= Time.fixedDeltaTime;

            if (inertiaJumpTimer <= 0) platformBoost = Vector2.zero;
        }

        // Action de saut avec prise en compte de l'inertie, selon si le joueur le veut ou non
        if (jumpBufferTimer > 0 && lastGroundedTimer > 0 && !isJumping)
        {
            rb.gravityScale = defaultGS;

            if (moveInput != 0 && Mathf.Sign(moveInput) != Mathf.Sign(platformBoost.x) && Mathf.Abs(platformBoost.x) > 0.1f)
            {
                platformBoost.x = 0f;
            }

            if (inertiaJumpTimer < 0)
            {
                platformBoost = Vector2.zero;
            }

            float limitY = currentPBase?.inertiaSpeedMaxY ?? 0f;
            float limitX = currentPBase?.inertiaSpeedMaxX ?? 0f;

            float newBoostY = platformBoost.y > 0 ? platformBoost.y : 0f;

            if (limitY > 0f && newBoostY > limitY)
            {
                newBoostY = limitY;
            }

            float platformVelX = currentPBase != null ? currentPBase.platformVelocity.x : 0f;
            float purePlayerSpeedX = rb.linearVelocity.x - platformVelX;

            if (limitX > 0f)
            {
                platformBoost.x = Mathf.Clamp(platformBoost.x, -limitX, limitX);
            }

            rb.linearVelocity = new Vector2(purePlayerSpeedX + platformBoost.x, newBoostY);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);


            isJumping = true;

            lastGroundedTimer = 0;

            jumpBufferTimer = 0;

            platformBoost = Vector2.zero;
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
}