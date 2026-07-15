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

    protected override void Start()
    {

        playerMouvsScript = GameObject.Find("Player").GetComponent<PlayerController>();
        platformRb = GetComponent<Rigidbody2D>();

        angle *= Mathf.Deg2Rad;
        initialPos = transform.position;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        targetPos = initialPos + (direction * range);

        currentState = PlatformState.Idle;
    }

    protected override void Update()
    {
        if (currentState == PlatformState.Idle && playerMouvsScript.isGrounded && playerMouvsScript.currentPBase == this)
        {
            currentState = PlatformState.MovingToTarget;
            actualSpeed = initialSpeed;
        }
    }

    protected override void FixedUpdate()
    {
        // On exécute le mouvement ICI pour être parfaitement synchro avec le joueur
        if (currentState == PlatformState.MovingToTarget)
        {
            Vector2 nextPos = Vector2.MoveTowards(platformRb.position, targetPos, actualSpeed * Time.fixedDeltaTime);
            platformRb.MovePosition(nextPos);

            // Vérification de l'arrivée
            if (Vector2.Distance(platformRb.position, targetPos) <= 0.001f)
            {
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
            Vector2 nextPos = Vector2.MoveTowards(platformRb.position, initialPos, actualSpeed * Time.fixedDeltaTime);
            platformRb.MovePosition(nextPos);

            if (Vector2.Distance(platformRb.position, initialPos) <= 0.001f)
            {
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
}