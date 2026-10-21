using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal.Internal;

public class LifeAndDeathSystem : MonoBehaviour
{
    public int initialHealth;
    private int healthBar;

    public Vector2 respawnPos;
    private bool isImmune;

    private Rigidbody2D rb;

    private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    [SerializeField] private GameObject Canvas;
    private UIManager UIManager;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UIManager = Canvas.GetComponent<UIManager>();

        rb = GetComponent<Rigidbody2D>();
        healthBar = initialHealth;
        UIManager.ActualizeHealth(healthBar);
        isImmune = false;
        Debug.Log(healthBar);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isImmune)
        {
            return;
        }

        if (collision.CompareTag("DamageEntity"))
        {
            TakeDamage();

            StartCoroutine(ResetInvulnerability());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            CheckPointPlatforms currentCheckpoint = collision.gameObject.GetComponent<CheckPointPlatforms>();

            if (currentCheckpoint != null)
            {
                ContactPoint2D contact = collision.GetContact(0);

                if (contact.normal.y > 0.5f)
                {
                    newCheckPoint(currentCheckpoint);
                }
            }
        }
    }

    IEnumerator ResetInvulnerability()
    {
        yield return new WaitForSeconds(0.2f);

        isImmune = false;

    }

    private void TakeDamage()
    {

        audioSource.PlayOneShot(deathSound);

        healthBar--;
        isImmune = true;

        UIManager.ActualizeHealth(healthBar);

        rb.position = respawnPos;
        rb.linearVelocity = Vector2.zero;

        if (healthBar <= 0)
        {
            GameManager.instance.gameOver = true;
            Debug.Log(GameManager.instance.gameOver);
        }
    }

    private void newCheckPoint(CheckPointPlatforms CheckPoint)
    {
        respawnPos = CheckPoint.respawnPos;

        if (CheckPoint.isFinal)
        {
            CheckPoint.levelManager.shouldChangeLevel = true;
        }

         // Debug.Log("Nouveau point de respawn enregistré : " + respawnPos);
    }
}
