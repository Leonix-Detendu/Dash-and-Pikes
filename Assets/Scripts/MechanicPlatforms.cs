using System.Collections;
using UnityEngine;

public class MechanicPlatforms : PlatformsBase
{
    [Header("Paramètres de la Plateforme")]
    public float initialSpeed;
    public float accelFactor;
    public float timeStop;
    public float angle;
    public float range;

    private Vector2 initialPos;
    private Vector2 targetPos; // On va précalculer la destination
    private float actualSpeed;

    public PlayerController playerMouvsScript;
    private Rigidbody2D platformRb;

    private enum PlatformState { Idle, MovingToTarget, Paused, Returning }
    private PlatformState currentState = PlatformState.Idle;

    [SerializeField] private float delayToBoost;
    private float delayRemaining;

    [Header("Audio")]
    private AudioSource audioSource;

    [SerializeField] private AudioClip movingSound;
    [SerializeField] private AudioClip impactSound;

    private bool shouldSound;
    private float soundDelay;
    [SerializeField] private float mvToTargetDelay;
    [SerializeField] private float returningDelay;


    protected override void Start()
    {
        canBoost = true;

        angleBoost *= Mathf.Deg2Rad;

        angleBoost %= 2 * Mathf.PI;

        boostVector = new Vector2(Mathf.Cos(angleBoost), Mathf.Sin(angleBoost)).normalized * superBoost;

        delayRemaining = delayToBoost;
      
        playerMouvsScript = GameObject.Find("Player").GetComponent<PlayerController>();
        platformRb = GetComponent<Rigidbody2D>();

        angle *= Mathf.Deg2Rad;
        initialPos = transform.position;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        targetPos = initialPos + (direction * range);

        currentState = PlatformState.Idle;

        audioSource = GetComponent<AudioSource>();

        shouldSound = true;
    }

    protected override void Update()
    {
        if (currentState == PlatformState.Idle && playerMouvsScript.isGrounded && playerMouvsScript.currentPBase == this)
        {
            currentState = PlatformState.MovingToTarget;
            actualSpeed = initialSpeed;
        }
        if (currentState == PlatformState.Paused)
        {
            delayRemaining -= Time.deltaTime;
            if (delayRemaining > 0)
            {
                shouldBoost = true;
            }
            else
            {
                shouldBoost = false;
            }
        }
        else
        {
            shouldBoost = false;
            delayRemaining = delayToBoost;
        }
    }

    protected override void FixedUpdate()
    {
        // On exécute le mouvement ICI pour être parfaitement synchro avec le joueur
        if (currentState == PlatformState.MovingToTarget)
        {
            
            if (shouldSound)
            {
                StartCoroutine(movingSoundRoutine(false));
            }
           
            Vector2 nextPos = Vector2.MoveTowards(platformRb.position, targetPos, actualSpeed * Time.fixedDeltaTime);
            platformVelocity = (nextPos - platformRb.position) / Time.fixedDeltaTime;
            platformRb.MovePosition(nextPos);

            // Vérification de l'arrivée
            if (Vector2.Distance(platformRb.position, targetPos) <= 0.001f)
            {
                audioSource.PlayOneShot(impactSound);
                shouldSound = true;
                currentState = PlatformState.Paused;
                platformRb.position = targetPos; // On la snap parfaitement sur sa cible
                StartCoroutine(PauseRoutine());
            }
            else
            {
                // On accélère seulement si on n'est pas encore arrivé
                actualSpeed *= accelFactor;
            }
        }
        else if (currentState == PlatformState.Returning)
        {
            if (shouldSound)
            {
                StartCoroutine(movingSoundRoutine(true));
            }

            Vector2 nextPos = Vector2.MoveTowards(platformRb.position, initialPos, actualSpeed * Time.fixedDeltaTime);
            platformVelocity = (nextPos - platformRb.position) / Time.fixedDeltaTime;
            platformRb.MovePosition(nextPos);

            if (Vector2.Distance(platformRb.position, initialPos) <= 0.001f)
            {
                shouldSound = true;
                currentState = PlatformState.Idle;
                platformRb.position = initialPos;
            }
        }
        else
        {
            // Si on est en "Idle" ou en "Paused", on force la vélocité à 0
            platformVelocity = Vector2.zero;
        }
    }

    // La coroutine ne sert plus qu'à gérer le chronomètre de pause !
    IEnumerator PauseRoutine()
    {
        yield return new WaitForSeconds(timeStop);
        currentState = PlatformState.Returning;
        actualSpeed = initialSpeed; // On réinitialise la vitesse pour le retour
    }

    IEnumerator movingSoundRoutine( bool isReturning) 
    {
        shouldSound = false;
        soundDelay = isReturning ? returningDelay : mvToTargetDelay;
        audioSource.PlayOneShot(movingSound);
        
        yield return new WaitForSeconds(soundDelay);

        shouldSound = true;
    }
}